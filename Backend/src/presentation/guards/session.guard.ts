import { SessionData } from '@/types/session.types';
import { Reflector } from '@nestjs/core';
import { ConfigService } from '@nestjs/config';
import { Request } from 'express';
import {
  UnauthorizedException,
  ForbiddenException,
  ExecutionContext,
  CanActivate,
  Injectable,
} from '@nestjs/common';

@Injectable()
export class SessionGuard implements CanActivate {
  private readonly INACTIVITY_TIMEOUT: number;

  constructor(
    private readonly reflector: Reflector,
    private readonly configService: ConfigService,
  ) {
    // Parse INACTIVITY_TIMEOUT once during construction
    this.INACTIVITY_TIMEOUT = this.configService.get<number>(
      'INACTIVITY_TIMEOUT',
      1800000, // Default: 30 minutes
    );
  }

  canActivate(context: ExecutionContext): boolean {
    // 1. Get required roles from the decorator BOTH the method and the class level
    const requiredRoles = this.reflector.getAllAndOverride<string[]>('roles', [
      context.getHandler(),
      context.getClass(),
    ]);

    const request = context.switchToHttp().getRequest<Request>();
    const session = request.session as SessionData;

    // 2. Check Authentication
    if (!session?.user) {
      throw new UnauthorizedException('No active session found');
    }

    // 3. Check for Inactivity
    const lastActivity = session.lastActivity;
    if (lastActivity) {
      const now = Date.now();
      if (now - lastActivity > this.INACTIVITY_TIMEOUT) {
        throw new UnauthorizedException('Session expired due to inactivity');
      }
    }

    // 4. Check Authorization (Roles)
    if (requiredRoles && requiredRoles.length > 0) {
      const userRole = session.user.role;
      const hasRole = requiredRoles.includes(userRole);

      if (!hasRole) {
        throw new ForbiddenException('Insufficient permissions');
      }
    }

    return true;
  }
}
