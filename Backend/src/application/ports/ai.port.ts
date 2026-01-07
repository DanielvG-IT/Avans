import {
  PredictionPayload,
  PredictionResponse,
} from '@/domain/predictions/prediction.model';
import { Result } from '@/result';

export interface IAiService {
  getPrediction(
    payload: PredictionPayload,
  ): Promise<Result<PredictionResponse>>;
}
