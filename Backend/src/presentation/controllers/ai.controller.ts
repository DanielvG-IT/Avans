import { RequireAuth } from '../decorators/auth.decorator';
import { AuthenticatedSession } from '@/types/session.types';
import { SessionGuard } from '../guards/session.guard';
import {
  BadRequestException,
  Controller,
  HttpStatus,
  UseGuards,
  HttpCode,
  Session,
  Inject,
  Body,
  Post,
} from '@nestjs/common';
import { Throttle } from '@nestjs/throttler';

// -- imports for ai --
import { PredictionDto, PredictionResponseDto } from '../dtos/ai.dto';
import { type IAiService } from '@/application/ports/ai.port';

@Controller('ai')
@UseGuards(SessionGuard)
export class AiController {
  constructor(@Inject('SERVICE.AI') private readonly aiService: IAiService) {}

  @Post('predict')
  @RequireAuth('STUDENT')
  @HttpCode(HttpStatus.OK)
  @Throttle({ default: { limit: 5, ttl: 300000 } }) // 5 requests per 5 minutes **LESS STRICT FOR PRESENTATION PURPOSES
  async createPrediction(
    @Session() session: AuthenticatedSession,
    @Body() prediction: PredictionDto,
  ): Promise<PredictionResponseDto> {
    const result = await this.aiService.getPrediction(prediction);
    if (result._tag === 'Failure') {
      const errorMessage = result.error?.message || 'Failed to get predictions';
      throw new BadRequestException({
        message: 'Prediction generation failed',
        details: errorMessage,
      });
    }

    return result.data;
  }
}
