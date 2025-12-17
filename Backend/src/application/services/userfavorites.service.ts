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

  getFavoritesForUser(userId: string): Promise<UserFavorite[]> {
    return this.favoritesRepository.findByUserId(userId);
  }

  async addFavorite(userId: string, moduleId: string): Promise<void> {
    const exists = await this.favoritesRepository.exists(userId, moduleId);
    if (!exists) {
      await this.favoritesRepository.add(userId, moduleId);
    }
  }

  async removeFavorite(userId: string, moduleId: string): Promise<void> {
    await this.favoritesRepository.remove(userId, moduleId);
  }

  async toggleFavorite(userId: string, moduleId: string): Promise<void> {
    const exists = await this.favoritesRepository.exists(userId, moduleId);

    if (exists) {
      await this.favoritesRepository.remove(userId, moduleId);
    } else {
      await this.favoritesRepository.add(userId, moduleId);
    }
  }
}
