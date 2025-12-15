import { User } from '@/domain/user/user.model';
import { Session as ExpressSession } from 'express-session';

/**
 * Extended session interface that includes our custom properties
 */
export interface SessionData extends ExpressSession {
  user?: User;
  lastActivity?: number;
}
