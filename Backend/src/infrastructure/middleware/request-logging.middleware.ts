import { Injectable, NestMiddleware } from '@nestjs/common';
import { Request, Response, NextFunction } from 'express';
import { SessionData } from '@/types/session.types';
import { LoggerService } from '../../logger.service';

@Injectable()
export class RequestLoggingMiddleware implements NestMiddleware {
  constructor(private readonly logger: LoggerService) {
    this.logger.setContext('RequestLoggingMiddleware');
  }

  use(req: Request, res: Response, next: NextFunction) {
    const start = Date.now();
    const { method, originalUrl } = req;

    // Update session activity for authenticated users
    const session = req.session as SessionData;
    if (session?.user) {
      session.lastActivity = Date.now();
    }

    res.on('finish', () => {
      const duration = Date.now() - start;
      const message = `${method} ${originalUrl} ${res.statusCode} ${duration}ms`;
      this.logger.log(message, 'HTTP');
    });

    next();
  }
}

export default RequestLoggingMiddleware;
