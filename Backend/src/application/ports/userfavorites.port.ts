import { UserFavorite } from '@/domain/userfavorites/userfavorites.model';

export interface IUserFavoritesService {
  findFavorites(userId: string): Promise<UserFavorite[]>;
  isModuleFavorited(userId: string, moduleId: string): Promise<boolean>;
  favoriteModule(userId: string, moduleId: string): Promise<void>;
  unfavoriteModule(userId: string, moduleId: string): Promise<void>;
}
