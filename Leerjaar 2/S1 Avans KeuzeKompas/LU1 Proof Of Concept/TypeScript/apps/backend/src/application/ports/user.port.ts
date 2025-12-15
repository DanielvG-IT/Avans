import { User } from "../../domain/user/user";
import { Result } from "../../domain/result";

//* User Service Interface
export interface IUserService {
  getUserById(id: string): Promise<Result<User>>;
  getUserByEmail(email: string): Promise<Result<User>>;
  getAllUsers(): Promise<Result<User[]>>;
  createUser(data: User): Promise<Result<User>>;
  updateUser(id: string, data: User | Partial<User>): Promise<Result<User>>;
  deleteUser(id: string): Promise<Result<boolean>>;
}
