import { SessionData } from '@/types/session.types';
import {
  BadRequestException,
  ConflictException,
  Controller,
  HttpStatus,
  HttpCode,
  Session,
  Inject,
  Body,
  Post,
} from '@nestjs/common';

// -- imports for auth --
import { type IAuthService } from '@/application/ports/auth.port';
import { LoginDto } from '@/presentation/dtos/auth.dto';
import { UserDTO } from '@/presentation/dtos/user.dto';

@Controller('auth')
export class AuthController {
  constructor(
    @Inject('SERVICE.AUTH') private readonly authService: IAuthService,
  ) {}

  @Post('login')
  @HttpCode(HttpStatus.OK)
  async login(
    @Body() dto: LoginDto,
    @Session() session: SessionData,
  ): Promise<{ user: UserDTO }> {
    if (session && session.user) {
      throw new ConflictException('Already authenticated');
    }

    const result = await this.authService.login(dto.email, dto.password);
    if (result._tag === 'Failure') {
      throw new BadRequestException('Failed to login: ' + result.error.message);
    }

    session.user = result.data.user;
    session.lastActivity = Date.now();
    return { user: result.data.user };
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
