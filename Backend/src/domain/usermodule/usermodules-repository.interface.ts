import { UserFavorite } from '@/domain/usermodule/userfavorite.model';
import { UserRecommended } from '@/domain/usermodule/userrecommended.model';

/**
 * Unified repository interface for UserModules table
 */
export interface IUserModulesRepository {
  // Favorites methods
  findFavoritesByUserId(userId: string): Promise<UserFavorite[]>;
  isFavorited(userId: string, moduleId: number): Promise<boolean>;
  addFavorite(userId: string, moduleId: number): Promise<void>;
  removeFavorite(userId: string, moduleId: number): Promise<void>;

  // Recommended methods
  findRecommendedByUserId(userId: string): Promise<UserRecommended[]>;
  setRecommendedModules(userId: string, moduleIds: number[]): Promise<void>;
}
