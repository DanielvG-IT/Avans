import { Injectable, NestMiddleware } from '@nestjs/common';
import { Request, Response, NextFunction } from 'express';
import { LoggerService } from '../../common/logger.service';

@Injectable()
export class RequestLoggingMiddleware implements NestMiddleware {
  constructor(private readonly logger: LoggerService) {
    this.logger.setContext('RequestLoggingMiddleware');
  }

  use(req: Request, res: Response, next: NextFunction) {
    const start = Date.now();
    const { method, originalUrl } = req;

    res.on('finish', () => {
      const duration = Date.now() - start;
      const message = `${method} ${originalUrl} ${res.statusCode} ${duration}ms`;
      this.logger.log(message, 'HTTP');
    });

    next();
  }
}

export default RequestLoggingMiddleware;
