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

@Controller('user/favorites')
export class UserFavoritesController {
  constructor(
    @Inject('SERVICE.USER_FAVORITES')
    private readonly favoritesService: UserFavoritesService,
  ) {}

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
    @Param('moduleId') moduleId: string,
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
    @Param('moduleId') moduleId: string,
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
    @Param('moduleId') moduleId: string,
    @Session() session: SessionData,
  ) {
    if (!session?.user) {
      throw new Error('No active session');
    }

    await this.favoritesService.unfavoriteModule(session.user.id, moduleId);
    return { success: true };
  }
}
