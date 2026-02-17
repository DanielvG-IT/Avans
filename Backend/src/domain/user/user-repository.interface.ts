import { Result } from '@/result';
import { User } from './user.model';

export interface IUserRepository {
  findById(id: string): Promise<Result<User>>;
  findByEmail(email: string): Promise<Result<User>>;
  create(user: User): Promise<Result<User>>;
  update(id: string, user: Partial<User>): Promise<Result<User>>;
  delete(id: string): Promise<Result>;
}
