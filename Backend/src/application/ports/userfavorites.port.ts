import { UserFavorite } from '@/domain/userfavorites/userfavorites.model';

export interface IUserFavoritesService {
  findFavorites(userId: string): Promise<UserFavorite[]>;
  isModuleFavorited(userId: string, moduleId: number): Promise<boolean>;
  favoriteModule(userId: string, moduleId: number): Promise<void>;
  unfavoriteModule(userId: string, moduleId: number): Promise<void>;
}
