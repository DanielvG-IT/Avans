import {
  Controller,
  Get,
  Post,
  Delete,
  Param,
  Session,
  Inject,
} from '@nestjs/common';
import { SessionData } from '@/types/session.types';
import { UserFavoritesService } from '@/application/services/userfavorites.service';

@Controller('favorites')
export class UserFavoritesController {
  private readonly favoritesService: UserFavoritesService;

  constructor(
    @Inject('SERVICE.USER_FAVORITES')
    favoritesService: UserFavoritesService,
  ) {
    this.favoritesService = favoritesService;
  }

  @Get()
  async getMyFavorites(@Session() session: SessionData) {
    const userId = session.user?.id;
    if (!userId) {
      throw new Error('No active session');
    }

    return {
      favorites: await this.favoritesService.getFavoritesForUser(userId),
    };
  }

  @Post(':moduleId')
  async addFavorite(
    @Param('moduleId') moduleId: string,
    @Session() session: SessionData,
  ) {
    const userId = session.user?.id;
    if (!userId) {
      throw new Error('No active session');
    }

    await this.favoritesService.addFavorite(userId, moduleId);
    return { success: true };
  }

  @Delete(':moduleId')
  async removeFavorite(
    @Param('moduleId') moduleId: string,
    @Session() session: SessionData,
  ) {
    const userId = session.user?.id;
    if (!userId) {
      throw new Error('No active session');
    }

    await this.favoritesService.removeFavorite(userId, moduleId);
    return { success: true };
  }
}
