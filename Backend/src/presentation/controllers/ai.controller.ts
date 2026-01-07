import { type IAiService } from '@/application/ports/ai.port';
import { SessionData } from '@/types/session.types';
import { PredictionDto } from '../dtos/ai.dto';
import {
  UnauthorizedException,
  BadRequestException,
  Controller,
  Session,
  Inject,
  Body,
  Post,
} from '@nestjs/common';

@Controller('ai')
export class AiController {
  private readonly aiService: IAiService;

  constructor(@Inject('SERVICE.AI') _aiService: IAiService) {
    this.aiService = _aiService;
  }

  @Post('predict')
  async createPrediction(
    @Session() session: SessionData,
    @Body() prediction: PredictionDto,
  ): Promise<{ prediction: { id: number; similarity_score: number }[] }> {
    if (!session) {
      throw new UnauthorizedException('No active session');
    }

    const result = await this.aiService.getPrediction(prediction);
    if (result._tag === 'Failure') {
      throw new BadRequestException(
        result.error?.message || 'Prediction failed',
      );
    }

    return { prediction: result.data.filtered_top_5_matches };
  }
}
