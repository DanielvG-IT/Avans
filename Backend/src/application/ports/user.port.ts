import { Result } from '@/result';
import { User } from '@/domain/user/user.model';

export interface IUserService {
  findById(id: string): Promise<Result<User>>;
}
