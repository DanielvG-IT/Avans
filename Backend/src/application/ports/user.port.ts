import { Result } from '@/result';
import { User } from '@/domain/user/user.model';
import { UserFavorite } from '@/domain/usermodule/userfavorite.model';

export interface IUserService {
  // User profile methods
  findById(id: string): Promise<Result<User>>;

  // User favorites methods
  findFavorites(userId: string): Promise<UserFavorite[]>;
  isModuleFavorited(userId: string, moduleId: number): Promise<boolean>;
  favoriteModule(userId: string, moduleId: number): Promise<void>;
  unfavoriteModule(userId: string, moduleId: number): Promise<void>;

  // User recommended methods
  setRecommendedModules(
    userId: string,
    modules: Array<{ moduleId: number; motivation?: string }>,
  ): Promise<void>;
  getRecommendedModuleIds(userId: string): Promise<number[]>;
  getRecommendedModules(
    userId: string,
  ): Promise<Array<{ moduleId: number; motivation?: string | null }>>;
}
