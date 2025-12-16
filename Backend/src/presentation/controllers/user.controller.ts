import { SessionData } from '@/types/session.types';
import { UserDTO } from '@/presentation/dtos/user.dto';
import {
  Controller,
  Get,
  Session,
  UnauthorizedException,
} from '@nestjs/common';

@Controller('user')
export class UserController {
  @Get('profile')
  async getProfile(@Session() session: SessionData): Promise<{ user: UserDTO }> {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    return { user: session.user };
  }
}
