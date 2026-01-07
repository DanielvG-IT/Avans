import { IPredictionClient } from '@/domain/predictions/prediction-client.interface';
import { IAiService } from '@/application/ports/ai.port';
import { LoggerService } from '@/common/logger.service';
import { Inject, Injectable } from '@nestjs/common';
import { fail, Result, succeed } from '@/result';
import {
  PredictionPayload,
  PredictionResponse,
} from '@/domain/predictions/prediction.model';

@Injectable()
export class AiService implements IAiService {
  private readonly predictionClient: IPredictionClient;

  constructor(
    @Inject('HTTP.AI') _predictionClient: IPredictionClient,
    private readonly logger?: LoggerService,
  ) {
    this.logger?.setContext('AiService');
    this.predictionClient = _predictionClient;
  }

  getPrediction(
    payload: PredictionPayload,
  ): Promise<Result<PredictionResponse>> {
    return this.predictionClient
      .getPrediction(payload)
      .then((matches) => {
        const response: PredictionResponse = {
          filtered_top_5_matches: matches,
        };
        return succeed(response);
      })
      .catch((error: Error) => {
        this.logger?.error(
          `Failed to get prediction: ${error.message}`,
          'AiService',
        );
        return fail(error);
      });
  }
}
