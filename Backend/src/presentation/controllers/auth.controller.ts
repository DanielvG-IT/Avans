import { type IAuthService } from '@/application/ports/auth.port';
import { LoginDto } from '@/presentation/dtos/auth.dto';
import { UserDTO } from '@/presentation/dtos/user.dto';
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

@Controller('auth')
export class AuthController {
  private readonly authService: IAuthService;

  constructor(@Inject('SERVICE.AUTH') _authService: IAuthService) {
    this.authService = _authService;
  }

  @Post('login')
  @HttpCode(HttpStatus.OK)
  async login(
    @Body() dto: LoginDto,
    @Session() session: Record<string, any>,
  ): Promise<{ user: UserDTO }> {
    if (session && session.user) {
      throw new ConflictException('Already authenticated');
    }

    if (!dto.email || !dto.password) {
      throw new BadRequestException('Missing email or password');
    }

    const result = await this.authService.login(dto.email, dto.password);
    if (result._tag === 'Failure') {
      throw new BadRequestException('Failed to login: ' + result.error.message);
    }

    session.user = result.data.user;
    return { user: result.data.user };
  }
}
