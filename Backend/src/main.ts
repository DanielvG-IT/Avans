import { Logger, ValidationPipe, VersioningType } from '@nestjs/common';
import { DocumentBuilder, SwaggerModule } from '@nestjs/swagger';
import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { LoggerService } from './logger.service';
import cookieParser from 'cookie-parser';
import session from 'express-session';
import helmet from 'helmet';
import { configDotenv } from 'dotenv';

configDotenv();

async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  const logger = app.get(LoggerService);
  // Let Nest use our LoggerService for framework logs and exceptions
  app.useLogger(logger);
  const isProduction = process.env.NODE_ENV === 'production';

  // Global API prefix - must be set before middleware
  app.setGlobalPrefix('api');

  // Security: Set various HTTP headers
  app.use(helmet());

  // Enable CORS
  app.enableCors({
    origin: process.env.ALLOWED_ORIGINS?.split(','),
    credentials: true,
    methods: ['GET', 'HEAD', 'PUT', 'PATCH', 'POST', 'DELETE', 'OPTIONS'],
  });

  // Parse cookies
  app.use(cookieParser());

  // Session Management
  if (process.env.SESSION_SECRET == null) {
    throw new Error('SESSION_SECRET environment variable is not set');
  }
  app.use(
    session({
      secret: process.env.SESSION_SECRET,
      resave: true,
      saveUninitialized: false,
      rolling: true, // Reset maxAge on every request
      cookie: {
        // For cross-site requests (frontend != backend) browsers require SameSite='None' and Secure=true
        sameSite: isProduction ? 'none' : 'lax',
        secure: isProduction,
        httpOnly: true,
        maxAge: 1000 * 60 * 60 * 24, // 24 hours
      },
    }),
  );

  // Global Validation: Whitelist and transform payloads
  app.useGlobalPipes(
    new ValidationPipe({
      whitelist: true, // Strip properties not in the DTO
      transform: true, // Auto-transform payloads to DTO instances
      forbidNonWhitelisted: true, // Throw error if non-whitelisted properties are present
      transformOptions: {
        enableImplicitConversion: true,
      },
    }),
  );

  // Ensure clients that omit the API version header still hit versioned routes
  // If a request doesn't include `X-API-Version`, default it to '1'
  app.use((req: any, _res: any, next: any) => {
    if (!req.headers || !req.headers['x-api-version']) {
      req.headers = req.headers || {};
      req.headers['x-api-version'] = '1';
    }
    next();
  });

  // API Versioning
  app.enableVersioning({
    type: VersioningType.HEADER,
    header: 'X-API-Version',
    defaultVersion: '1',
  });

  // Swagger Documentation (Development only)
  if (isProduction === false) {
    const config = new DocumentBuilder()
      .setTitle('CompassGPT API')
      .setDescription('The CompassGPT API description')
      .setVersion('1.0')
      .addCookieAuth()
      .build();

    const document = SwaggerModule.createDocument(app, config);
    SwaggerModule.setup('docs', app, document);
    logger.log('Swagger UI initialized at /docs', 'Bootstrap');
  }

  // Graceful Shutdown
  app.enableShutdownHooks();

  const port = process.env.PUBLIC_PORT ?? 4000;
  await app.listen(port);
  logger.log(`Application is running on: ${await app.getUrl()}`, 'Bootstrap');
}

bootstrap().catch((err) => {
  const logger = new Logger('Bootstrap');
  logger.error('Fatal error during application bootstrap', err);
  process.exit(1);
});
