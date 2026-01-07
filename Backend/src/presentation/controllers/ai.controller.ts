import { type IAiService } from '@/application/ports/ai.port';
import { SessionData } from '@/types/session.types';
import { PredictionDto } from '../dtos/ai.dto';
import {
  UnauthorizedException,
  BadRequestException,
  Controller,
  HttpStatus,
  HttpCode,
  Session,
  Inject,
  Body,
  Post,
} from '@nestjs/common';

@Controller('ai')
export class AuthController {
  private readonly aiService: IAiService;

  constructor(@Inject('SERVICE.AI') _aiService: IAiService) {
    this.aiService = _aiService;
  }

  @Post()
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

  @Post('logout')
  @HttpCode(HttpStatus.OK)
  async logout(@Session() session: SessionData): Promise<{ message: string }> {
    if (!session || !session.user) {
      throw new BadRequestException('No active session to logout');
    }

    return new Promise((resolve, reject) => {
      session.destroy((err?: Error) => {
        if (err) {
          reject(new BadRequestException('Failed to logout'));
        } else {
          resolve({ message: 'Successfully logged out' });
        }
      });
    });
  }
}
