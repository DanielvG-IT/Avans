import { PrismaService } from './infrastructure/database/prisma';
import { AuthService } from './application/services/auth.service';
import { UserService } from './application/services/user.service';
import { Module, NestModule, MiddlewareConsumer } from '@nestjs/common';
import { AuthController } from './presentation/controllers/auth.controller';
import { UserController } from './presentation/controllers/user.controller';
import { UserRepository } from './infrastructure/database/repositories/user.repository';
import { SessionActivityMiddleware } from './presentation/middleware/session-activity.middleware';

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
export class AppModule implements NestModule {
  configure(consumer: MiddlewareConsumer) {
    consumer.apply(SessionActivityMiddleware).forRoutes('*');
  }
}
