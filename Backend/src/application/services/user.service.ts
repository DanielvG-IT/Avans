import { type IUserRepository } from '@/domain/user/user-repository.interface';
import { IUserService } from '@/application/ports/user.port';
import { Inject, Injectable } from '@nestjs/common';

@Injectable()
export class UserService implements IUserService {
  private readonly userRepository: IUserRepository;

  constructor(@Inject('REPO.USER') _userRepository: IUserRepository) {
    this.userRepository = _userRepository;
  }
}
