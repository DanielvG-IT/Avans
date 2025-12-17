import { AuthService } from './application/services/auth.service';
import { UserService } from './application/services/user.service';
import { Module, NestModule, MiddlewareConsumer } from '@nestjs/common';
import { AuthController } from './presentation/controllers/auth.controller';
import { UserController } from './presentation/controllers/user.controller';
import { LoggerService } from './common/logger.service';
import { RequestLoggingMiddleware } from './infrastructure/middleware/request-logging.middleware';
import { PrismaService } from './infrastructure/database/prisma';
import { AppController } from './presentation/controllers/app.controller';

class SessionActivityMiddleware {
  use(req: any, res: any, next: () => void) {
    // no-op middleware to track session activity (placeholder implementation)
    next();
  }
}
import { ChoiceModulesRepository } from './infrastructure/database/repositories/choicemodules.repository';
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
    consumer
      .apply(RequestLoggingMiddleware, SessionActivityMiddleware)
      .forRoutes('*');
  }
}
