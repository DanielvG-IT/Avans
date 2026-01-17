import { SessionData, AuthenticatedSession } from '@/types/session.types';
import {
  BadRequestException,
  ConflictException,
  Controller,
  HttpStatus,
  UseGuards,
  HttpCode,
  Session,
  Inject,
  Body,
  Post,
  Res,
} from '@nestjs/common';
import { Response } from 'express';
import { Throttle } from '@nestjs/throttler';

// -- imports for auth --
import { IAuthService } from '@/application/ports/auth.port';
import { LoginDto } from '@/presentation/dtos/auth.dto';
import { UserDTO } from '@/presentation/dtos/user.dto';
import { LoginResponseDto } from '@/presentation/dtos/auth.response.dto';
import { SessionGuard } from '../guards/session.guard';

@Controller('auth')
export class AuthController {
  constructor(
    @Inject('SERVICE.AUTH') private readonly authService: IAuthService,
  ) {}

  @Post('login')
  @Throttle({ default: { limit: 5, ttl: 60000*3 } }) // 10 login attempts per minute
  @HttpCode(HttpStatus.OK)
  async login(
    @Body() dto: LoginDto,
    @Session() session: SessionData,
  ): Promise<{ user: LoginResponseDto }> {
    if (session?.user) {
      throw new ConflictException('Already authenticated');
    }

    const result = await this.authService.login(dto.email, dto.password);
    if (result._tag === 'Failure') {
      throw new BadRequestException('Failed to login: ' + result.error.message);
    }

    session.user = result.data.user;
    session.lastActivity = Date.now();
    
    // Only return name and role in the response
    return { 
      user: {
        name: result.data.user.name,
        role: result.data.user.role,
      }
    };
  }

  @Post('logout')
  @UseGuards(SessionGuard)
  @HttpCode(HttpStatus.OK)
  async logout(
    @Session() session: AuthenticatedSession,
    @Res({ passthrough: true }) res: Response,
  ): Promise<{ message: string }> {
    return new Promise((resolve, reject) => {
      session.destroy((err?: Error) => {
        if (err) {
          reject(new BadRequestException('Failed to logout'));
        } else {
          res.clearCookie('connect.sid');
          resolve({ message: 'Successfully logged out' });
        }
      });
    });
  }
}
