import { Injectable, NestMiddleware } from '@nestjs/common';
import { Request, Response, NextFunction } from 'express';
import { SessionData } from '@/types/session.types';

/**
 * Middleware that tracks session activity by updating the lastActivity timestamp
 * on each request. This enables automatic session expiration after 30 minutes of inactivity.
 */
@Injectable()
export class SessionActivityMiddleware implements NestMiddleware {
  private readonly INACTIVITY_TIMEOUT = 30 * 60 * 1000; // 30 minutes in milliseconds

  use(req: Request, res: Response, next: NextFunction) {
    const session = req.session as SessionData;

    if (session && session.user) {
      const now = Date.now();
      const lastActivity = session.lastActivity;

      // Check if session has expired due to inactivity
      if (lastActivity && now - lastActivity > this.INACTIVITY_TIMEOUT) {
        // Session expired - destroy it
        session.destroy((err?: Error) => {
          if (err) {
            console.error('Error destroying expired session:', err);
          }
        });
        // Clear the session cookie
        res.clearCookie('connect.sid');
        return next();
      }

      // Update last activity timestamp
      session.lastActivity = now;
    }

    next();
  }
}
