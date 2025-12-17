import { UserFavorite } from '@/domain/userfavorites/userfavorites.model';

export interface IUserFavoritesService {
  getFavoritesForUser(userId: string): Promise<UserFavorite[]>;
  addFavorite(userId: string, moduleId: string): Promise<void>;
  removeFavorite(userId: string, moduleId: string): Promise<void>;
  toggleFavorite(userId: string, moduleId: string): Promise<void>;
}
