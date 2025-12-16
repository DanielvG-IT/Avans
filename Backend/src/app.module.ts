import { Module, NestModule, MiddlewareConsumer } from '@nestjs/common';
import { AuthService } from './application/services/auth.service';
import { UserService } from './application/services/user.service';
import { AuthController } from './presentation/controllers/auth.controller';
import { UserController } from './presentation/controllers/user.controller';
import { UserRepository } from './infrastructure/repositories/user.repository';
import { PrismaService } from './infrastructure/database/prisma';
import { SessionActivityMiddleware } from './infrastructure/middleware/session-activity.middleware';
import { AppController } from './presentation/controllers/app.controller';
import { ChoiceModulesRepository } from './infrastructure/repositories/choicemodules.repository';
import { ModuleService } from './application/services/module.service';
import { ModulesController } from './presentation/controllers/modules.controller';
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
    {
      provide: 'REPO.MODULE',
      useClass: ChoiceModulesRepository,
    },
    {
      provide: 'SERVICE.MODULE',
      useClass: ModuleService,
    },
  ],
  controllers: [
    AuthController,
    UserController,
    ModulesController,
    AppController,
  ],
})
export class AppModule implements NestModule {
  configure(consumer: MiddlewareConsumer) {
    consumer.apply(SessionActivityMiddleware).forRoutes('*');
  }
}
