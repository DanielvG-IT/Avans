// General Imports
import { SessionData } from '@/types/session.types';
import {
  UnauthorizedException,
  NotFoundException,
  ParseIntPipe,
  Controller,
  Session,
  Delete,
  Inject,
  Param,
  Body,
  Post,
  Get,
} from '@nestjs/common';

// Users
import { IUserService } from '@/application/ports/user.port';
import { UserDTO } from '@/presentation/dtos/user.dto';
import { User } from '@/domain/user/user.model';

// User Favorites
import { IUserFavoritesService } from '@/application/ports/userfavorites.port';

// User Recommended
import { IUserRecommendedService } from '@/application/ports/userrecommended.port';
import { SubmitRecommendedDto } from '../dtos/userrecommended.dto';

// ==========================
// Controller
// ==========================
@Controller('user')
export class UserController {
  private readonly userService: IUserService;
  private readonly favoritesService: IUserFavoritesService;
  private readonly recommendedService: IUserRecommendedService;

  constructor(
    @Inject('SERVICE.USER') _userService: IUserService,
    @Inject('SERVICE.USER_FAVORITES') _favoritesService: IUserFavoritesService,
    @Inject('SERVICE.USER_RECOMMENDED')
    _userRecommendedService: IUserRecommendedService,
  ) {
    this.userService = _userService;
    this.favoritesService = _favoritesService;
    this.recommendedService = _userRecommendedService;
  }

  // GET /user/me
  @Get('profile')
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

  // GET /user/favorites
  @Get()
  async findFavorites(@Session() session: SessionData) {
    if (!session?.user) {
      throw new Error('No active session');
    }

    return {
      favorites: await this.favoritesService.findFavorites(session.user.id),
    };
  }

  // GET /user/favorites/:moduleId
  @Get(':moduleId')
  async isModuleFavorited(
    @Param('moduleId', ParseIntPipe) moduleId: number,
    @Session() session: SessionData,
  ) {
    if (!session?.user) {
      throw new Error('No active session');
    }

    return {
      isFavorited: await this.favoritesService.isModuleFavorited(
        session.user.id,
        moduleId,
      ),
    };
  }

  // POST /user/favorites/:moduleId
  @Post(':moduleId')
  async favoriteModule(
    @Param('moduleId', ParseIntPipe) moduleId: number,
    @Session() session: SessionData,
  ) {
    if (!session?.user) {
      throw new Error('No active session');
    }

    await this.favoritesService.favoriteModule(session.user.id, moduleId);
    return { success: true };
  }

  // DELETE /user/favorites/:moduleId
  @Delete(':moduleId')
  async unfavoriteModule(
    @Param('moduleId', ParseIntPipe) moduleId: number,
    @Session() session: SessionData,
  ) {
    if (!session?.user) {
      throw new Error('No active session');
    }

    await this.favoritesService.unfavoriteModule(session.user.id, moduleId);
    return { success: true };
  }

  // GET /user/recommended
  @Get('recommended')
  async getRecommended(@Session() session: SessionData) {
    if (!session?.user) {
      throw new Error('No active session');
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

  // POST /user/recommended
  @Post('recommended')
  async submitRecommended(
    @Body() body: SubmitRecommendedDto,
    @Session() session: SessionData,
  ) {
    if (!session?.user) {
      throw new Error('No active session');
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
