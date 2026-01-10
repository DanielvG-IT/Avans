import { Module, NestModule, MiddlewareConsumer } from '@nestjs/common';
import { HttpModule } from '@nestjs/axios';
import { ConfigModule } from '@nestjs/config';

// -- imports for controllers --
import { AppController } from './presentation/controllers/app.controller';
import { AuthController } from './presentation/controllers/auth.controller';
import { UserController } from './presentation/controllers/user.controller';
import { ModulesController } from './presentation/controllers/modules.controller';
import { AiController } from './presentation/controllers/ai.controller';

// -- imports for services --
import { AuthService } from './application/services/auth.service';
import { UserService } from './application/services/user.service';
import { ModuleService } from './application/services/module.service';
import { AiService } from './application/services/ai.service';

// -- imports for repositories --
import { UserRepository } from './infrastructure/database/repositories/user.repository';
import { ModuleRepository } from './infrastructure/database/repositories/module.repository';
import { UserFavoritesRepository } from './infrastructure/database/repositories/userfavorites.repository';
import { UserRecommendedRepository } from './infrastructure/database/repositories/userrecommended.repository';
import { LocationRepository } from './infrastructure/database/repositories/location.repository';
import { ModuleTagRepository } from './infrastructure/database/repositories/moduletag.repository';

// -- imports for infrastructure --
import { PrismaService } from './infrastructure/database/prisma';
import { AiHttpClient } from './infrastructure/ai-service/prediction-client';
import { LoggerService } from './common/logger.service';
import { RequestLoggingMiddleware } from './infrastructure/middleware/request-logging.middleware';

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
    // Auth Service - handles authentication
    {
      provide: 'SERVICE.AUTH',
      useClass: AuthService,
    },
    // User Service - handles profile, favorites, and recommendations
    {
      provide: 'SERVICE.USER',
      useClass: UserService,
    },
    {
      provide: 'SERVICE.USER_FAVORITES',
      useClass: UserService,
    },
    {
      provide: 'SERVICE.USER_RECOMMENDED',
      useClass: UserService,
    },
    // Module Service - handles modules, locations, and tags
    {
      provide: 'SERVICE.MODULE',
      useClass: ModuleService,
    },
    {
      provide: 'SERVICE.LOCATION',
      useClass: ModuleService,
    },
    {
      provide: 'SERVICE.MODULETAG',
      useClass: ModuleService,
    },
    // AI Service
    {
      provide: 'SERVICE.AI',
      useClass: AiService,
    },
    // Repositories
    {
      provide: 'REPO.USER',
      useClass: UserRepository,
    },
    {
      provide: 'REPO.USER_FAVORITES',
      useClass: UserFavoritesRepository,
    },
    {
      provide: 'REPO.USER_RECOMMENDED',
      useClass: UserRecommendedRepository,
    },
    {
      provide: 'REPO.MODULE',
      useClass: ModuleRepository,
    },
    {
      provide: 'REPO.LOCATION',
      useClass: LocationRepository,
    },
    {
      provide: 'REPO.MODULETAG',
      useClass: ModuleTagRepository,
    },
    // HTTP Clients
    {
      provide: 'HTTP.AI',
      useClass: AiHttpClient,
    },
  ],
  controllers: [
    AppController,
    AuthController,
    UserController,
    ModulesController,
    AiController,
  ],
})
export class AppModule implements NestModule {
  configure(consumer: MiddlewareConsumer) {
    consumer
      .apply(RequestLoggingMiddleware, SessionActivityMiddleware)
      .forRoutes('*');
  }
}
