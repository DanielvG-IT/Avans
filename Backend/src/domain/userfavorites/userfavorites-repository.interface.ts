import { UserFavorite } from './userfavorites.model';

export interface IUserFavoritesRepository {
  findByUserId(userId: string): Promise<UserFavorite[]>;
  add(userId: string, itemId: number): Promise<void>;
  remove(userId: string, itemId: number): Promise<void>;
  exists(userId: string, itemId: number): Promise<boolean>;
}
