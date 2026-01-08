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
        );

      const response = await firstValueFrom(request$);
      return response.data.filtered_top_5_matches;
    } catch (error: unknown) {
      // Transform and throw appropriate HttpException
      throw this.handleError(error);
    }
  }

  /**
   * Private helper that logs and transforms errors into HttpExceptions.
   * Returns the exception instead of throwing to make error flow explicit.
   */
  private handleError(error: unknown): HttpException {
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

      return new HttpException(message, status);
    }

    // Handle generic system errors (e.g., DNS, Timeout)
    const errorMessage =
      error instanceof Error ? error.message : 'Unknown error';
    this.logger.error(`Connection Error: ${errorMessage}`);

    return new HttpException(
      'AI Service Unavailable',
      HttpStatus.GATEWAY_TIMEOUT,
    );
  }
}
