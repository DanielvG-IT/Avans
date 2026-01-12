import { IPredictionClient } from '@/domain/prediction/prediction-client.interface';
import { Injectable, HttpException, HttpStatus } from '@nestjs/common';
import { firstValueFrom, timeout, Observable } from 'rxjs';
import { LoggerService } from '@/logger.service';
import { ConfigService } from '@nestjs/config';
import { HttpService } from '@nestjs/axios';
import { isAxiosError } from 'axios';
import {
  PredictionMatch,
  PredictionPayload,
  PredictionResponse,
} from '@/domain/prediction/prediction.model';

@Injectable()
export class PredictionClient implements IPredictionClient {
  private readonly aiServiceUrl: string;

  constructor(
    private readonly httpService: HttpService,
    private readonly configService: ConfigService,
    private readonly logger: LoggerService,
  ) {
    this.logger.setContext('PredictionClient');
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

    this.logger.log(
      `Sending payload to AI service: ${JSON.stringify(payload)}`,
    );

    try {
      const request$: Observable<{ data: PredictionResponse }> =
        this.httpService.post<PredictionResponse>(url, payload).pipe(
          timeout(60000), // AI services need more time (60s)
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
   * Extracts detailed error information from various response formats.
   */
  private handleError(error: unknown): HttpException {
    if (isAxiosError(error)) {
      const status = error.response?.status ?? HttpStatus.INTERNAL_SERVER_ERROR;
      const responseData: unknown = error.response?.data;

      // Log full response for debugging
      this.logger.error(
        `[AI Service Error] ${status} - Full response: ${JSON.stringify(responseData)}`,
      );

      // Try to extract a meaningful error message from various formats
      const message = this.extractErrorMessage(responseData, status);

      this.logger.error(`[AI Service Error] Extracted message: ${message}`);

      return new HttpException(message, status);
    }

    // Handle generic system errors (e.g., DNS, Timeout)
    const errorMessage =
      error instanceof Error ? error.message : 'Unknown error';
    this.logger.error(`[Connection Error] ${errorMessage}`);

    return new HttpException(
      `AI Service Unavailable: ${errorMessage}`,
      HttpStatus.GATEWAY_TIMEOUT,
    );
  }

  /**
   * Extract error message from various response formats
   */
  private extractErrorMessage(responseData: unknown, status: number): string {
    // Helper to check for object property
    const hasProp = (obj: unknown, prop: string): boolean =>
      !!obj && typeof obj === 'object' && prop in obj;

    // Handle Pydantic validation errors (FastAPI format)
    if (hasProp(responseData, 'detail')) {
      const detail = (responseData as Record<string, unknown>).detail;
      if (Array.isArray(detail)) {
        const validationErrors = detail
          .map((err) => {
            if (typeof err === 'object' && err !== null) {
              const e = err as Record<string, unknown>;
              const loc = Array.isArray(e.loc)
                ? e.loc.join('.')
                : 'unknown field';
              const msg =
                typeof e.msg === 'string' ? e.msg : 'validation failed';
              return `${loc}: ${msg}`;
            }
            return String(err);
          })
          .join('; ');
        return `Validation error: ${validationErrors}`;
      }
      if (typeof detail === 'string') return detail;
      if (typeof detail === 'object') return JSON.stringify(detail);
    }

    // Handle generic message field
    if (hasProp(responseData, 'message')) {
      const msg = (responseData as Record<string, unknown>).message;
      if (typeof msg === 'string') return msg;
    }

    // Handle error field
    if (hasProp(responseData, 'error')) {
      const err = (responseData as Record<string, unknown>).error;
      if (typeof err === 'string') return err;
    }

    // Handle plain string response
    if (typeof responseData === 'string') return responseData;

    // Fallback with HTTP status code hint
    const statusText: Record<number, string> = {
      400: 'Bad Request - Invalid input data',
      401: 'Unauthorized - Authentication failed',
      403: 'Forbidden - Access denied',
      404: 'Not found - Resource does not exist',
      422: 'Validation failed - Check your input data',
      500: 'AI Service internal error',
      503: 'AI Service unavailable',
    };

    return statusText[status] || `AI Service error (HTTP ${status})`;
  }
}
