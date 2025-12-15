import { User } from '@/domain/user/user.model';
import { Result } from '@/result';

export interface IAuthService {
  login(email: string, password: string): Promise<Result<{ user: User }>>;
}
