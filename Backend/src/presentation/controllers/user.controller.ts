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
import { IUserService } from '@/application/ports/user.port';
// -- imports for user favorites --
import { IUserFavoritesService } from '@/application/ports/userfavorites.port';
// -- imports for user recommended --
import { IUserRecommendedService } from '@/application/ports/userrecommended.port';
import { SubmitRecommendedDto } from '../dtos/userrecommended.dto';

@Controller('user')
export class UserController {
  constructor(
    @Inject('SERVICE.USER') private readonly userService: IUserService,
    @Inject('SERVICE.USER_FAVORITES')
    private readonly favoritesService: IUserFavoritesService,
    @Inject('SERVICE.USER_RECOMMENDED')
    private readonly recommendedService: IUserRecommendedService,
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
      favorites: await this.favoritesService.findFavorites(session.user.id),
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
      isFavorited: await this.favoritesService.isModuleFavorited(
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

    await this.favoritesService.favoriteModule(session.user.id, moduleId);
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

    await this.favoritesService.unfavoriteModule(session.user.id, moduleId);
    return { success: true };
  }

  @Get('recommended')
  @HttpCode(HttpStatus.OK)
  async getRecommended(@Session() session: SessionData) {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    const MAX_RECENT_RECOMMENDED = 5;
    const moduleIds = await this.recommendedService.getRecommendedModuleIds(
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

    await this.recommendedService.setRecommendedModules(
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
