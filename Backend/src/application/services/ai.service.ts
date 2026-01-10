import { IPredictionClient } from '@/domain/prediction/prediction-client.interface';
import { PredictionPayload } from '@/domain/prediction/prediction.model';
import { IModuleService } from '@/application/ports/module.port';
import { PredictionDto } from '@/presentation/dtos/ai.dto';
import { IAiService } from '@/application/ports/ai.port';
import { LoggerService } from '@/logger.service';
import { Module } from '@/domain/module/module.model';
import { Inject, Injectable } from '@nestjs/common';
import { fail, Result, succeed } from '@/result';

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
    @Inject('CLIENT.PREDICTION') predictionClient: IPredictionClient,
    @Inject('SERVICE.MODULE') moduleService: IModuleService,
    private readonly logger?: LoggerService,
  ) {
    this.logger?.setContext('AiService');
    this.predictionClient = predictionClient;
    this.moduleService = moduleService;
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
          try {
            const module = await this.moduleService.findById(match.id);
            if (!module) {
              throw new Error(
                `Kan de aanbevolen module (ID: ${match.id}) niet vinden in de database`,
              );
            }
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
          } catch (moduleError: unknown) {
            const msg =
              moduleError instanceof Error
                ? moduleError.message
                : 'Onbekende fout bij ophalen module';
            throw new Error(
              `Kan de aanbevolen module (ID: ${match.id}) niet verwerken: ${msg}`,
            );
          }
        }),
      );

      return succeed({ predictions });
    } catch (error: unknown) {
      let errorMessage = 'Kan de voorstellen niet verwerken';

      if (error instanceof Error) {
        errorMessage = error.message;
      } else if (typeof error === 'string') {
        errorMessage = error;
      } else if (typeof error === 'object' && error !== null) {
        errorMessage = JSON.stringify(error);
      }

      this.logger?.error(
        `Failed to get prediction: ${errorMessage}`,
        error instanceof Error ? error.stack : undefined,
      );

      return fail(new Error(errorMessage));
    }
  }

  /**
   * Transform TypeScript DTO (camelCase) to Python API payload (snake_case)
   * Maps frontend camelCase properties to backend snake_case requirements
   * @param dto Prediction DTO with camelCase properties
   * @returns Payload object with snake_case properties
   */
  private transformDtoToPayload(dto: PredictionDto): PredictionPayload {
    return {
      current_study: dto.currentStudy,
      interests: dto.interests,
      wanted_study_credit_range: dto.wantedStudyCreditRange,
      location_preference: dto.locationPreference,
      learning_goals: dto.learningGoals,
      level_preference: dto.levelPreference,
      preferred_language: dto.preferredLanguage,
      preferred_period: dto.preferredPeriod,
    };
  }
}
