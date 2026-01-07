import { Injectable, Logger, HttpException, HttpStatus } from '@nestjs/common';
import { firstValueFrom, catchError, timeout, Observable } from 'rxjs';
import { ConfigService } from '@nestjs/config';
import { HttpService } from '@nestjs/axios';
import { isAxiosError } from 'axios';

/**
 * Interface representing the input from your prompt
 */
export interface PredictionPayload {
  current_study: string;
  interests: string[];
  wanted_study_credit_range: [number, number];
  location_preference: string[];
  learning_goals: string[];
  level_preference: string[];
  preferred_language: string;
}

/**
 * Interface representing the AI service output
 */
export interface PredictionMatch {
  id: number;
  similarity_score: number;
}

export interface PredictionResponse {
  filtered_top_5_matches: PredictionMatch[];
}

@Injectable()
export class AiHttpClient {
  private readonly logger = new Logger(AiHttpClient.name);
  private readonly aiServiceUrl: string;

  constructor(
    private readonly httpService: HttpService,
    private readonly configService: ConfigService,
  ) {
    // Use the config service with a fallback string to satisfy ESLint
    this.aiServiceUrl = this.configService.get<string>(
      'AI_SERVICE_URL',
      'http://localhost:8000',
    );
  }

  async getPrediction(payload: PredictionPayload): Promise<PredictionMatch[]> {
    const url = `${this.aiServiceUrl}/predict`;

    try {
      const request$: Observable<{ data: PredictionResponse }> =
        this.httpService.post<PredictionResponse>(url, payload).pipe(
          timeout(10000), // AI services need more time (10s)
          catchError((error: unknown) => {
            this.handleError(error);
            throw error;
          }),
        );

      const response = await firstValueFrom(request$);
      return response.data.filtered_top_5_matches;
    } catch (error: unknown) {
      if (error instanceof HttpException) {
        throw error;
      }

      this.logger.error(`Connection Error: ${(error as Error).message}`);
      throw new HttpException(
        'AI Service Unavailable',
        HttpStatus.GATEWAY_TIMEOUT,
      );
    }
  }

  /**
   * Private helper designed to satisfy @typescript-eslint/no-unsafe rules
   */
  private handleError(error: unknown): void {
    if (isAxiosError(error)) {
      const status = error.response?.status ?? HttpStatus.INTERNAL_SERVER_ERROR;

      // Cast the response data to a safe record type
      const errorData = error.response?.data as
        | Record<string, unknown>
        | undefined;
      const message = errorData?.message ?? 'AI Service encountered an error';

      this.logger.error(
        `[AI Service Error] ${status} - ${JSON.stringify(message)}`,
      );

      throw new HttpException(message, status);
    }

    // Handle generic system errors (e.g., DNS, Timeout)
    throw new HttpException(
      'Internal Network Error',
      HttpStatus.INTERNAL_SERVER_ERROR,
    );
  }
}
