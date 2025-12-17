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
import { UserFavoritesService } from './application/services/userfavorites.service';
import { UserFavoritesRepository } from './infrastructure/repositories/userfavorites.repository';
import { UserFavoritesController } from './presentation/controllers/userfavorites.controller';
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
    {
      provide: 'SERVICE.USER_FAVORITES',
      useClass: UserFavoritesService,
    },
    {
      provide: 'REPO.USER_FAVORITES',
      useClass: UserFavoritesRepository,
    },
  ],
  controllers: [
    AuthController,
    UserController,
    ModulesController,
    AppController,
    UserFavoritesController,
  ],
})
export class AppModule implements NestModule {
  configure(consumer: MiddlewareConsumer) {
    consumer.apply(SessionActivityMiddleware).forRoutes('*');
  }
}
