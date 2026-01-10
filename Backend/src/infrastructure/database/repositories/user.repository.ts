import { type IUserRepository } from '@/domain/user/user-repository.interface';
import { PrismaService } from '@/infrastructure/database/prisma';
import type { User } from '@/domain/user/user.model';
import { Role as PrismaRole } from '@/infrastructure/database/generated/prisma/enums';
import { Injectable } from '@nestjs/common';
import { Result, succeed, fail } from '@/result';

@Injectable()
export class UserRepository implements IUserRepository {
  constructor(private readonly prisma: PrismaService) {}

  async findById(id: string): Promise<Result<User>> {
    try {
      const user = await this.prisma.user.findUnique({
        where: { id },
      });

      if (!user) return fail(new Error('User not found'));

      const domainUser: User = {
        id: user.id,
        name: user.name,
        email: user.email,
        hashedPassword: user.hashedPassword,
        role: String(user.role),
        createdAt: user.createdAt,
        updatedAt: user.updatedAt,
      };

      return succeed(domainUser);
    } catch (error) {
      return fail(error instanceof Error ? error : new Error('Unknown error'));
    }
  }
  async findByEmail(email: string): Promise<Result<User>> {
    try {
      const user = await this.prisma.user.findUnique({
        where: { email },
      });

      if (!user) return fail(new Error('User not found'));

      const domainUser: User = {
        id: user.id,
        name: user.name,
        email: user.email,
        hashedPassword: user.hashedPassword,
        role: String(user.role),
        createdAt: user.createdAt,
        updatedAt: user.updatedAt,
      };

      return succeed(domainUser);
    } catch (error) {
      return fail(error instanceof Error ? error : new Error('Unknown error'));
    }
  }

  async create(user: User): Promise<Result<User>> {
    try {
      const createdUser = await this.prisma.user.create({
        data: {
          id: user.id,
          email: user.email,
          hashedPassword: user.hashedPassword,
          name: user.name,
          role: user.role as unknown as PrismaRole,
          createdAt: user.createdAt,
          updatedAt: user.updatedAt,
        },
      });

      const domainUser: User = {
        id: createdUser.id,
        name: createdUser.name,
        email: createdUser.email,
        hashedPassword: createdUser.hashedPassword,
        role: String(createdUser.role),
        createdAt: createdUser.createdAt,
        updatedAt: createdUser.updatedAt,
      };

      return succeed(domainUser);
    } catch (error) {
      return fail(error instanceof Error ? error : new Error('Unknown error'));
    }
  }

  async update(id: string, user: Partial<User>): Promise<Result<User>> {
    try {
      const data: any = { updatedAt: new Date() };
      if (user.name !== undefined) data.name = user.name;
      if (user.email !== undefined) data.email = user.email;
      if (user.hashedPassword !== undefined)
        data.hashedPassword = user.hashedPassword;
      if (user.role !== undefined)
        data.role = user.role as unknown as PrismaRole;

      const updatedUser = await this.prisma.user.update({
        where: { id },
        data,
      });

      const domainUser: User = {
        id: updatedUser.id,
        name: updatedUser.name,
        email: updatedUser.email,
        hashedPassword: updatedUser.hashedPassword,
        role: String(updatedUser.role),
        createdAt: updatedUser.createdAt,
        updatedAt: updatedUser.updatedAt,
      };

      return succeed(domainUser);
    } catch (error) {
      return fail(error instanceof Error ? error : new Error('Unknown error'));
    }
  }

  async delete(id: string): Promise<Result> {
    try {
      await this.prisma.user.delete({
        where: { id },
      });
      return succeed(void 0);
    } catch (error) {
      return fail(error instanceof Error ? error : new Error('Unknown error'));
    }
  }
}
