import { AuthService } from './application/services/auth.service';
import { UserService } from './application/services/user.service';
import { Module, NestModule, MiddlewareConsumer } from '@nestjs/common';
import { HttpModule } from '@nestjs/axios';
import { ConfigModule } from '@nestjs/config';
import { AuthController } from './presentation/controllers/auth.controller';
import { UserController } from './presentation/controllers/user.controller';
import { LoggerService } from './common/logger.service';
import { RequestLoggingMiddleware } from './infrastructure/middleware/request-logging.middleware';
import { PrismaService } from './infrastructure/database/prisma';
import { AppController } from './presentation/controllers/app.controller';
import { AiHttpClient } from './infrastructure/ai-service/ai-client';

import { ChoiceModulesRepository } from './infrastructure/database/repositories/choicemodules.repository';
import { UserFavoritesService } from './application/services/userfavorites.service';
import { UserFavoritesRepository } from './infrastructure/database/repositories/userfavorites.repository';
import { UserFavoritesController } from './presentation/controllers/userfavorites.controller';

import { LocationRepository } from './infrastructure/database/repositories/location.repository';
import { LocationService } from './application/services/location.service';
import { LocationController } from './presentation/controllers/location.controller';
import { ModuleTagRepository } from './infrastructure/database/repositories/moduletag.repository';
import { ModuleTagService } from './application/services/moduletag.service';
import { ModuleTagController } from './presentation/controllers/moduletag.controller';

import { UserRepository } from './infrastructure/database/repositories/user.repository';
import { ModuleService } from './application/services/module.service';
import { ModulesController } from './presentation/controllers/modules.controller';

class SessionActivityMiddleware {
  use(req: any, res: any, next: () => void) {
    // no-op middleware to track session activity (placeholder implementation)
    next();
  }
}

@Module({
  imports: [
    HttpModule,
    ConfigModule.forRoot({
      isGlobal: true,
    }),
  ],
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
    {
      provide: 'REPO.LOCATION',
      useClass: LocationRepository,
    },
    {
      provide: 'SERVICE.LOCATION',
      useClass: LocationService,
    },
    {
      provide: 'REPO.MODULETAG',
      useClass: ModuleTagRepository,
    },
    {
      provide: 'SERVICE.MODULETAG',
      useClass: ModuleTagService,
    },
    AiHttpClient,
  ],
  controllers: [
    AuthController,
    UserController,
    ModulesController,
    AppController,
    UserFavoritesController,
    LocationController,
    ModuleTagController,
  ],
})
export class AppModule implements NestModule {
  configure(consumer: MiddlewareConsumer) {
    consumer
      .apply(RequestLoggingMiddleware, SessionActivityMiddleware)
      .forRoutes('*');
  }
}
