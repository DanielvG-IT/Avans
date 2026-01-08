import { Injectable, Inject } from '@nestjs/common';
import { IUserFavoritesService } from '@/application/ports/userfavorites.port';
import { IUserFavoritesRepository } from '@/domain/userfavorites/userfavorites-repository.interface';
import { UserFavorite } from '@/domain/userfavorites/userfavorites.model';

@Injectable()
export class UserFavoritesService implements IUserFavoritesService {
  private readonly favoritesRepository: IUserFavoritesRepository;

  constructor(
    @Inject('REPO.USER_FAVORITES')
    _favoritesRepository: IUserFavoritesRepository,
  ) {
    this.favoritesRepository = _favoritesRepository;
  }

  findFavorites(userId: string): Promise<UserFavorite[]> {
    return this.favoritesRepository.findByUserId(userId);
  }

  isModuleFavorited(userId: string, moduleId: number): Promise<boolean> {
    return this.favoritesRepository.exists(userId, moduleId);
  }

  async favoriteModule(userId: string, moduleId: number): Promise<void> {
    const exists = await this.favoritesRepository.exists(userId, moduleId);
    if (!exists) {
      await this.favoritesRepository.add(userId, moduleId);
    }
  }

  async unfavoriteModule(userId: string, moduleId: number): Promise<void> {
    await this.favoritesRepository.remove(userId, moduleId);
  }
}
