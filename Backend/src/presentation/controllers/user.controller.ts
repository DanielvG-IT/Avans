import { SessionData } from '@/types/session.types';
import { UserDTO } from '@/presentation/dtos/user.dto';
import { IUserService } from '@/application/ports/user.port';
import { User } from '@/domain/user/user.model';
import { Result } from '@/result';
import {
  Controller,
  Get,
  Inject,
  NotFoundException,
  Session,
  UnauthorizedException,
} from '@nestjs/common';

@Controller('user')
export class UserController {
  private readonly userService: IUserService;

  constructor(@Inject('SERVICE.USER') _userService: IUserService) {
    this.userService = _userService;
  }

  @Get('profile')
  async getProfile(
    @Session() session: SessionData,
  ): Promise<{ user: UserDTO }> {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    const result: Result<User> = await this.userService.findById(
      session.user.id,
    );
    if (result._tag === 'Failure' || !result.data) {
      throw new NotFoundException('User not found');
    }

    const freshUser = result.data;
    session.user = freshUser;

    return { user: this.toUserDto(freshUser) };
  }

  private toUserDto(user: User): UserDTO {
    const dto = new UserDTO();
    dto.id = user.id;
    dto.name = user.name;
    dto.email = user.email;
    dto.role = user.role;
    dto.createdAt = user.createdAt;
    dto.updatedAt = user.updatedAt;
    return dto;
  }
}
