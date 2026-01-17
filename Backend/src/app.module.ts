import { Module, NestModule, MiddlewareConsumer } from '@nestjs/common';
import { HttpModule } from '@nestjs/axios';
import { ConfigModule } from '@nestjs/config';
import { TerminusModule } from '@nestjs/terminus';
import { ThrottlerModule, ThrottlerGuard } from '@nestjs/throttler';
import { APP_GUARD } from '@nestjs/core';

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
import { UserModulesRepository } from './infrastructure/database/repositories/usermodules.repository';
import { LocationRepository } from './infrastructure/database/repositories/location.repository';
import { ModuleTagRepository } from './infrastructure/database/repositories/moduletag.repository';

// -- imports for infrastructure --
import { LoggerService } from './logger.service';
import { PrismaService } from './infrastructure/database/prisma';
import { PredictionClient } from './infrastructure/prediction/prediction-client';
import { RequestLoggingMiddleware } from './infrastructure/middleware/request-logging.middleware';

@Module({
  imports: [
    HttpModule,
    TerminusModule,
    ConfigModule.forRoot({
      isGlobal: true,
    }),
    ThrottlerModule.forRoot([{
      ttl: 60000,  // 60 seconds
      limit: 30,   // 30 requests per minute (default for all endpoints)
    }]),
  ],
  providers: [
    // Global Guards
    {
      provide: APP_GUARD,
      useClass: ThrottlerGuard,
    },
    // Common
    LoggerService,
    PrismaService,
    // Services
    { provide: 'SERVICE.AUTH', useClass: AuthService },
    { provide: 'SERVICE.USER', useClass: UserService },
    { provide: 'SERVICE.MODULE', useClass: ModuleService },
    { provide: 'SERVICE.AI', useClass: AiService },
    // Repositories
    { provide: 'REPO.USER', useClass: UserRepository },
    { provide: 'REPO.USER_MODULES', useClass: UserModulesRepository },
    { provide: 'REPO.MODULE', useClass: ModuleRepository },
    { provide: 'REPO.LOCATION', useClass: LocationRepository },
    { provide: 'REPO.MODULETAG', useClass: ModuleTagRepository },
    // HTTP Clients
    { provide: 'CLIENT.PREDICTION', useClass: PredictionClient },
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
    consumer.apply(RequestLoggingMiddleware).forRoutes('*');
  }
}
