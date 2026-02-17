import { PredictionDto } from '@/presentation/dtos/ai.dto';
import { PredictionWithModules } from '@/application/services/ai.service';
import { Result } from '@/result';

/**
 * Application service port for AI prediction functionality
 */
export interface IAiService {
  /**
   * Get module predictions based on student preferences
   * @param dto Prediction DTO with camelCase properties (TypeScript convention)
   * @returns Result containing predicted modules with full details or error
   */
  getPrediction(dto: PredictionDto): Promise<Result<PredictionWithModules>>;
}
