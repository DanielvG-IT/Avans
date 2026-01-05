import { UserFavorite } from './userfavorites.model';

export interface IUserFavoritesRepository {
  findByUserId(userId: string): Promise<UserFavorite[]>;
  add(userId: string, itemId: string): Promise<void>;
  remove(userId: string, itemId: string): Promise<void>;
  exists(userId: string, itemId: string): Promise<boolean>;
}
