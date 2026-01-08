import { PredictionMatch, PredictionPayload } from './prediction.model';

export interface IPredictionClient {
  getPrediction(payload: PredictionPayload): Promise<PredictionMatch[]>;
}
