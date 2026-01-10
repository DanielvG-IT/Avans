import { SessionData } from '@/types/session.types';
import {
  UnauthorizedException,
  NotFoundException,
  ParseIntPipe,
  Controller,
  HttpStatus,
  HttpCode,
  Session,
  Delete,
  Inject,
  Param,
  Body,
  Post,
  Get,
} from '@nestjs/common';

// -- imports for users --
import { User } from '@/domain/user/user.model';
import { UserDTO } from '@/presentation/dtos/user.dto';
import { UserService } from '@/application/services/user.service';
import { SubmitRecommendedDto } from '../dtos/userrecommended.dto';

@Controller('user')
export class UserController {
  constructor(
    @Inject('SERVICE.USER') private readonly userService: UserService,
  ) {}

  @Get('profile')
  @HttpCode(HttpStatus.OK)
  async getProfile(
    @Session() session: SessionData,
  ): Promise<{ user: UserDTO }> {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    const result = await this.userService.findById(session.user.id);
    if (result._tag === 'Failure' || !result.data) {
      throw new NotFoundException('User not found');
    }

    const freshUser = result.data;
    session.user = freshUser;

    return { user: this.toUserDto(freshUser) };
  }

  @Get()
  @HttpCode(HttpStatus.OK)
  async findFavorites(@Session() session: SessionData) {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    return {
      favorites: await this.userService.findFavorites(session.user.id),
    };
  }

  @Get(':moduleId')
  @HttpCode(HttpStatus.OK)
  async isModuleFavorited(
    @Param('moduleId', ParseIntPipe) moduleId: number,
    @Session() session: SessionData,
  ) {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    return {
      isFavorited: await this.userService.isModuleFavorited(
        session.user.id,
        moduleId,
      ),
    };
  }

  @Post(':moduleId')
  @HttpCode(HttpStatus.CREATED)
  async favoriteModule(
    @Param('moduleId', ParseIntPipe) moduleId: number,
    @Session() session: SessionData,
  ) {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    await this.userService.favoriteModule(session.user.id, moduleId);
    return { success: true };
  }

  @Delete(':moduleId')
  @HttpCode(HttpStatus.OK)
  async unfavoriteModule(
    @Param('moduleId', ParseIntPipe) moduleId: number,
    @Session() session: SessionData,
  ) {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    await this.userService.unfavoriteModule(session.user.id, moduleId);
    return { success: true };
  }

  @Get('recommended')
  @HttpCode(HttpStatus.OK)
  async getRecommended(@Session() session: SessionData) {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    const MAX_RECENT_RECOMMENDED = 5;
    const moduleIds = await this.userService.getRecommendedModuleIds(
      session.user.id,
    );
    const recentIds = moduleIds.slice(0, MAX_RECENT_RECOMMENDED);

    return {
      recommended: recentIds.map((choiceModuleId) => ({ choiceModuleId })),
    };
  }

  @Post('recommended')
  @HttpCode(HttpStatus.CREATED)
  async submitRecommended(
    @Body() body: SubmitRecommendedDto,
    @Session() session: SessionData,
  ) {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    await this.userService.setRecommendedModules(
      session.user.id,
      body.moduleIds,
    );
    return { success: true };
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
