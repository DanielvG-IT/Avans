export interface IUserService {
  findById(id: string): Promise<any>;
}
