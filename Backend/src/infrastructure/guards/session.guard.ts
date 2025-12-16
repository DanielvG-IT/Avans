import {
  Injectable,
  CanActivate,
  ExecutionContext,
  UnauthorizedException,
} from '@nestjs/common';
import { Request } from 'express';
import { SessionData } from '@/types/session.types';

/**
 * Guard that ensures a valid, non-expired session exists.
 * Checks if the user is authenticated and the session hasn't expired due to inactivity.
 */
@Injectable()
export class SessionGuard implements CanActivate {
  private readonly INACTIVITY_TIMEOUT = 30 * 60 * 1000; // 30 minutes in milliseconds

  canActivate(context: ExecutionContext): boolean {
    const request = context.switchToHttp().getRequest<Request>();
    const session = request.session as SessionData;

    // Check if user is authenticated
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session found');
    }

    // Check for session inactivity
    const lastActivity = session.lastActivity;
    if (lastActivity) {
      const now = Date.now();
      if (now - lastActivity > this.INACTIVITY_TIMEOUT) {
        // Session expired due to inactivity
        throw new UnauthorizedException('Session expired due to inactivity');
      }
    }

    return true;
  }
}
