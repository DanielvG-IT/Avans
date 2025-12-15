import cookieParser from "cookie-parser";
import { AppModule } from "./app.module";
import { NestFactory } from "@nestjs/core";
import { corsOrigin, logLevel, nodeEnv } from "./constants";
import { Logger, VersioningType, INestApplication } from "@nestjs/common";

// Immediately Invoked Function Expression (IIFE) to allow use of async/await at the top level
// and to encapsulate the application bootstrap logic
// This pattern also helps in managing the application lifecycle and error handling
// by allowing us to use try/catch and ensure proper cleanup on failure.
void (async () => {
  let app: INestApplication | undefined;
  const logger = new Logger("Bootstrap");

  try {
    app = await NestFactory.create(AppModule);

    // Enable CORS with environment-based origin
    app.enableCors({
      origin: process.env.CORS_ORIGIN ?? "*",
      credentials: true,
      methods: ["GET", "HEAD", "PUT", "PATCH", "POST", "DELETE"],
      preflightContinue: false,
      optionsSuccessStatus: 204,
    });

    // Use cookie parser middleware
    app.use(cookieParser());

    // Set global API prefix and versioning
    app.setGlobalPrefix("api");
    app.enableVersioning({
      type: VersioningType.HEADER,
      header: "X-API-Version",
      defaultVersion: "1",
    });

    // Load environment variables from .env file
    logger.debug(`Current Environment: ${nodeEnv}`);
    logger.debug(`Log Level: ${logLevel}`);
    logger.debug(`CORS Origin: ${corsOrigin}`);

    // Swagger setup in development environment
    if (nodeEnv === "development") {
      const { SwaggerModule, DocumentBuilder } = await import("@nestjs/swagger");

      const config = new DocumentBuilder()
        .setTitle("API Documentation")
        .setDescription("API documentation for Avans Keuzekompas Backend")
        .addCookieAuth("ACCESSTOKEN")
        .setVersion("0.1.0")
        .build();
      SwaggerModule.setup("api/docs", app, SwaggerModule.createDocument(app, config));
      logger.log("Swagger documentation available at /api/docs");
    }

    // Start the application
    const port = parseInt(process.env.PORT ?? "3001", 10);
    await app.listen(port);
    logger.log(`Application is running on http://localhost:${port}`);
  } catch (error) {
    // Safely normalize the caught value to an Error for logging
    const err = error instanceof Error ? error : new Error(String(error));
    logger.error("Application failed to start", err.stack ?? err.message);

    if (app) {
      try {
        await app.close();
        logger.log("Application closed due to startup error");
      } catch (closeError) {
        const cerr = closeError instanceof Error ? closeError : new Error(String(closeError));
        logger.error("Failed to close application gracefully", cerr.stack ?? cerr.message);
      }
    }

    process.exit(1); // Exit with failure
  }
})();
