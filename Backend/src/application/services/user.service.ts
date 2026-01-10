import { Inject, Injectable } from '@nestjs/common';
import { Result } from '@/result';
import { LoggerService } from '@/common/logger.service';

// -- imports for user domain --
import { User } from '@/domain/user/user.model';
import { type IUserRepository } from '@/domain/user/user-repository.interface';
import { IUserService } from '@/application/ports/user.port';

// -- imports for user favorites --
import { IUserFavoritesService } from '@/application/ports/userfavorites.port';
import { IUserFavoritesRepository } from '@/domain/userfavorites/userfavorites-repository.interface';
import { UserFavorite } from '@/domain/userfavorites/userfavorites.model';

// -- imports for user recommended --
import { IUserRecommendedService } from '@/application/ports/userrecommended.port';
import { IUserRecommendedRepository } from '@/domain/userrecommended/userrecommended-repository.interface';

@Injectable()
export class UserService
  implements IUserService, IUserFavoritesService, IUserRecommendedService
{
  constructor(
    @Inject('REPO.USER') private readonly userRepository: IUserRepository,
    @Inject('REPO.USER_FAVORITES')
    private readonly favoritesRepository: IUserFavoritesRepository,
    @Inject('REPO.USER_RECOMMENDED')
    private readonly recommendedRepository: IUserRecommendedRepository,
    private readonly logger?: LoggerService,
  ) {
    this.logger?.setContext('UserService');
  }

  // ==========================================
  // User Profile Methods
  // ==========================================

  async findById(id: string): Promise<Result<User>> {
    return await this.userRepository.findById(id);
  }

  // ==========================================
  // User Favorites Methods
  // ==========================================

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

  // ==========================================
  // User Recommended Methods
  // ==========================================

  setRecommendedModules(userId: string, moduleIds: number[]): Promise<void> {
    return this.recommendedRepository.setRecommendedModules(userId, moduleIds);
  }

  getRecommendedModuleIds(userId: string): Promise<number[]> {
    return this.recommendedRepository.getRecommendedModuleIds(userId);
  }
}
