import { Module } from '@nestjs/common';
import { AuthService } from './application/services/auth.service';
import { UserService } from './application/services/user.service';
import { AuthController } from './presentation/controllers/auth.controller';
import { UserController } from './presentation/controllers/user.controller';
import { UserRepository } from './infrastructure/repositories/user.repository';
import { PrismaService } from './infrastructure/database/prisma';

@Module({
  imports: [],
  providers: [
    PrismaService,
    {
      provide: 'SERVICE.AUTH',
      useClass: AuthService,
    },
    {
      provide: 'SERVICE.USER',
      useClass: UserService,
    },
    {
      provide: 'REPO.USER',
      useClass: UserRepository,
    },
  ],
  controllers: [AuthController, UserController],
})
export class AppModule {}
