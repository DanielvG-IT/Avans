import { fail, Result, succeed } from '@/result';
import { Inject, Injectable } from '@nestjs/common';
import { verify, type Options } from '@node-rs/argon2';
import { LoggerService } from '@/logger.service';

// -- imports for auth --
import { type IUserRepository } from '@/domain/user/user-repository.interface';
import { IAuthService } from '@/application/ports/auth.port';
import { User } from '@/domain/user/user.model';

@Injectable()
export class AuthService implements IAuthService {
  private readonly argonOptions: Options = {
    memoryCost: 19456,
    parallelism: 1,
    outputLen: 32,
    timeCost: 2,
  };

  constructor(
    @Inject('REPO.USER') private readonly userRepository: IUserRepository,
    private readonly logger?: LoggerService,
  ) {
    this.logger?.setContext('AuthService');
  }

  async login(
    email: string,
    password: string,
  ): Promise<Result<{ user: User }>> {
    const result = await this.userRepository.findByEmail(email);
    if (result._tag === 'Failure') {
      const repoMsg = result.error?.message ?? 'Unknown error';
      const logMsg =
        repoMsg === 'User not found' ? 'Credentials not correct' : repoMsg;
      this.logger?.warn(`Failed login attempt for ${email}: ${logMsg}`);
      // Always return generic "Invalid credentials" error to prevent user enumeration
      return fail(new Error('Invalid credentials'));
    }

    const user = result.data;
    const isMatch = await verify(
      user.hashedPassword,
      password,
      this.argonOptions,
    );
    if (!isMatch) {
      this.logger?.warn(`Invalid credentials for ${email}`);
      return fail(new Error('Invalid credentials'));
    }

    return succeed({ user });
  }

  logout(): Result<void> {
    return succeed(void 0);
  }
}
