import { type IUserRepository } from '@/domain/user/user-repository.interface';
import { IAuthService } from '@/application/ports/auth.port';
import { verify, type Options } from '@node-rs/argon2';
import { Inject, Injectable } from '@nestjs/common';
import { LoggerService } from '@/common/logger.service';
import { fail, succeed } from '@/result';

@Injectable()
export class AuthService implements IAuthService {
  private readonly userRepository: IUserRepository;
  private readonly opts: Options = {
    memoryCost: 19456,
    parallelism: 1,
    outputLen: 32,
    timeCost: 2,
  };

  constructor(
    @Inject('REPO.USER') _userRepository: IUserRepository,
    private readonly logger?: LoggerService,
  ) {
    this.userRepository = _userRepository;
  }

  async login(email: string, password: string) {
    const result = await this.userRepository.findByEmail(email);
    if (result._tag === 'Failure') {
      // Don't log repository-specific "User not found" to avoid exposing user existence.
      const repoMsg = result.error?.message ?? 'Unknown error';
      const logMsg =
        repoMsg === 'User not found' ? 'Credentials not correct' : repoMsg;
      this.logger?.warn(
        `Failed login attempt for ${email}: ${logMsg}`,
        'AuthService',
      );
      return fail(result.error);
    }

    const user = result.data;

    const isMatch = await verify(user.hashedPassword, password);
    if (!isMatch) {
      this.logger?.warn(`Invalid credentials for ${email}`, 'AuthService');
      return fail(new Error('Invalid credentials'));
    }

    return succeed({ user });
  }

  logout() {
    return succeed(void 0);
  }
}
