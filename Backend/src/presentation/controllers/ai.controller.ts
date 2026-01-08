import { PredictionDto, PredictionResponseDto } from '../dtos/ai.dto';
import { type IAiService } from '@/application/ports/ai.port';
import { SessionGuard } from '../guards/session.guard';
import { SessionData } from '@/types/session.types';
import {
  UnauthorizedException,
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

@Controller('ai')
@UseGuards(SessionGuard)
export class AiController {
  private readonly aiService: IAiService;

  constructor(@Inject('SERVICE.AI') _aiService: IAiService) {
    this.aiService = _aiService;
  }

  @Post('predict')
  @HttpCode(HttpStatus.OK)
  async createPrediction(
    @Session() session: SessionData,
    @Body() prediction: PredictionDto,
  ): Promise<PredictionResponseDto> {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    const result = await this.aiService.getPrediction(prediction);
    if (result._tag === 'Failure') {
      throw new BadRequestException(
        result.error?.message || 'Prediction failed',
      );
    }

    return result.data;
  }
}
