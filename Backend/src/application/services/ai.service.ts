import { IPredictionClient } from '@/domain/predictions/prediction-client.interface';
import { IAiService } from '@/application/ports/ai.port';
import { IModuleService } from '@/application/ports/module.port';
import { LoggerService } from '@/common/logger.service';
import { Inject, Injectable } from '@nestjs/common';
import { fail, Result, succeed } from '@/result';
import { PredictionPayload } from '@/domain/predictions/prediction.model';
import { PredictionDto } from '@/presentation/dtos/ai.dto';
import { instanceToPlain } from 'class-transformer';
import { Module } from '@/domain/modules/module.model';

/**
 * Enhanced prediction response with full module details
 */
export interface PredictionWithModules {
  predictions: Array<{
    module: Module;
    score: number;
  }>;
}

@Injectable()
export class AiService implements IAiService {
  private readonly predictionClient: IPredictionClient;
  private readonly moduleService: IModuleService;

  constructor(
    @Inject('HTTP.AI') _predictionClient: IPredictionClient,
    @Inject('SERVICE.MODULE') _moduleService: IModuleService,
    private readonly logger?: LoggerService,
  ) {
    this.logger?.setContext('AiService');
    this.predictionClient = _predictionClient;
    this.moduleService = _moduleService;
  }

  /**
   * Get module predictions from AI service based on student preferences
   * Fetches full module details for each prediction
   * @param dto Validated prediction DTO with student preferences (camelCase)
   * @returns Result containing predictions with full module details or error
   */
  async getPrediction(
    dto: PredictionDto,
  ): Promise<Result<PredictionWithModules>> {
    // Transform camelCase DTO to snake_case payload for Python API
    const payload = this.transformDtoToPayload(dto);

    this.logger?.log(
      `Requesting prediction for study: ${payload.current_study}`,
    );

    try {
      const matches = await this.predictionClient.getPrediction(payload);

      this.logger?.log(`Received ${matches.length} prediction matches`);

      // Fetch full module details for each prediction
      const predictions = await Promise.all(
        matches.map(async (match) => {
          const module = await this.moduleService.findById(String(match.id));
          return {
            module: {
              id: module.id,
              name: module.name,
              shortdescription: module.description,
              studyCredits: module.studyCredits,
              level: module.level,
              location: module.location,
              startDate: module.startDate,
            },
            score: match.similarity_score,
          };
        }),
      );

      return succeed({ predictions });
    } catch (error: unknown) {
      const errorMessage =
        error instanceof Error ? error.message : 'Unknown error';

      this.logger?.error(
        `Failed to get prediction: ${errorMessage}`,
        error instanceof Error ? error.stack : undefined,
      );

      return fail(
        error instanceof Error ? error : new Error('Prediction service failed'),
      );
    }
  }

  /**
   * Transform TypeScript DTO (camelCase) to Python API payload (snake_case)
   * Uses class-transformer to handle the @Expose() decorators
   * @param dto Prediction DTO with camelCase properties
   * @returns Payload object with snake_case properties
   */
  private transformDtoToPayload(dto: PredictionDto): PredictionPayload {
    // Use class-transformer to properly handle @Expose() mappings
    const plain = instanceToPlain(dto) as PredictionPayload;
    return plain;
  }
}
