import { AuthService } from './application/services/auth.service';
import { UserService } from './application/services/user.service';
import { Module, NestModule, MiddlewareConsumer } from '@nestjs/common';
import { AuthController } from './presentation/controllers/auth.controller';
import { UserController } from './presentation/controllers/user.controller';
import { LoggerService } from './common/logger.service';
import { RequestLoggingMiddleware } from './infrastructure/middleware/request-logging.middleware';
import { PrismaService } from './infrastructure/database/prisma';
import { AppController } from './presentation/controllers/app.controller';
import { ChoiceModulesRepository } from './infrastructure/database/repositories/choicemodules.repository';
import { UserFavoritesService } from './application/services/userfavorites.service';
import { UserFavoritesRepository } from './infrastructure/database/repositories/userfavorites.repository';
import { UserFavoritesController } from './presentation/controllers/userfavorites.controller';

class SessionActivityMiddleware {
  use(req: any, res: any, next: () => void) {
    // no-op middleware to track session activity (placeholder implementation)
    next();
  }
}
import { UserRepository } from './infrastructure/database/repositories/user.repository';
import { ModuleService } from './application/services/module.service';
import { ModulesController } from './presentation/controllers/modules.controller';
@Module({
  imports: [],
  providers: [
    LoggerService,
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
    consumer
      .apply(RequestLoggingMiddleware, SessionActivityMiddleware)
      .forRoutes('*');
  }
}
