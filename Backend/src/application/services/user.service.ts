import { Inject, Injectable } from '@nestjs/common';
import { Result } from '@/result';
import { LoggerService } from '@/logger.service';

// -- imports for user domain --
import { User } from '@/domain/user/user.model';
import { type IUserRepository } from '@/domain/user/user-repository.interface';
import { IUserService } from '@/application/ports/user.port';

// -- imports for user modules (favorites & recommended) --
import { IUserModulesRepository } from '@/domain/usermodule/usermodules-repository.interface';
import { UserFavorite } from '@/domain/usermodule/userfavorite.model';

@Injectable()
export class UserService implements IUserService {
  constructor(
    @Inject('REPO.USER') private readonly userRepository: IUserRepository,
    @Inject('REPO.USER_MODULES')
    private readonly userModulesRepository: IUserModulesRepository,
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
    return this.userModulesRepository.findFavoritesByUserId(userId);
  }

  isModuleFavorited(userId: string, moduleId: number): Promise<boolean> {
    return this.userModulesRepository.isFavorited(userId, moduleId);
  }

  async favoriteModule(userId: string, moduleId: number): Promise<void> {
    const exists = await this.userModulesRepository.isFavorited(
      userId,
      moduleId,
    );
    if (!exists) {
      await this.userModulesRepository.addFavorite(userId, moduleId);
    }
  }

  async unfavoriteModule(userId: string, moduleId: number): Promise<void> {
    await this.userModulesRepository.removeFavorite(userId, moduleId);
  }

  // ==========================================
  // User Recommended Methods
  // ==========================================

  setRecommendedModules(userId: string, moduleIds: number[]): Promise<void> {
    return this.userModulesRepository.setRecommendedModules(userId, moduleIds);
  }

  async getRecommendedModuleIds(userId: string): Promise<number[]> {
    const recommended =
      await this.userModulesRepository.findRecommendedByUserId(userId);
    return recommended.map((r) => r.moduleId);
  }
}
