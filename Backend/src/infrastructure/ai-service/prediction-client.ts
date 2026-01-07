import { IPredictionClient } from '@/domain/predictions/prediction-client.interface';
import { Injectable, HttpException, HttpStatus } from '@nestjs/common';
import { firstValueFrom, catchError, timeout, Observable } from 'rxjs';
import { LoggerService } from '@/common/logger.service';
import { ConfigService } from '@nestjs/config';
import { HttpService } from '@nestjs/axios';
import { isAxiosError } from 'axios';
import {
  PredictionMatch,
  PredictionPayload,
  PredictionResponse,
} from '@/domain/predictions/prediction.model';

@Injectable()
export class AiHttpClient implements IPredictionClient {
  private readonly aiServiceUrl: string;

  constructor(
    private readonly httpService: HttpService,
    private readonly configService: ConfigService,
    private readonly logger: LoggerService,
  ) {
    this.logger.setContext('AiHttpClient');
    // Use the config service with a fallback string to satisfy ESLint
    this.aiServiceUrl = this.configService.get<string>(
      'AI_SERVICE_URL',
      'http://localhost:8000',
    );
  }

  public async getPrediction(
    payload: PredictionPayload,
  ): Promise<PredictionMatch[]> {
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
