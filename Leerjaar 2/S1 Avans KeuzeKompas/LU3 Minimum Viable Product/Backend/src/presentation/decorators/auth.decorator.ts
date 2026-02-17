import { SetMetadata } from '@nestjs/common';

export const RequireAuth = (...roles: string[]) => SetMetadata('roles', roles);
