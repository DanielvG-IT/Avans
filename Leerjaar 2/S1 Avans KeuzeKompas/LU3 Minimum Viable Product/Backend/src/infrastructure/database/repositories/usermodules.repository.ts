import { IUserModulesRepository } from '@/domain/usermodule/usermodules-repository.interface';
import { UserRecommended } from '@/domain/usermodule/userrecommended.model';
import { UserFavorite } from '@/domain/usermodule/userfavorite.model';
import { Injectable } from '@nestjs/common';
import { PrismaService } from '../prisma';

/**
 * Unified repository for UserModules table
 * Handles both favorites and recommended modules since they share the same table
 */
@Injectable()
export class UserModulesRepository implements IUserModulesRepository {
  constructor(private readonly prisma: PrismaService) {}

  // ==========================================
  // Favorites Methods
  // ==========================================

  async findFavoritesByUserId(userId: string): Promise<UserFavorite[]> {
    const favorites = await this.prisma.userModules.findMany({
      where: { userId, favorited: true },
      orderBy: { updatedAt: 'desc' },
    });

    return favorites.map((fav) => ({
      id: fav.id,
      userId: fav.userId,
      moduleId: fav.moduleId,
      createdAt: fav.createdAt,
    }));
  }

  async isFavorited(userId: string, moduleId: number): Promise<boolean> {
    const record = await this.prisma.userModules.findUnique({
      where: {
        userId_moduleId: {
          userId,
          moduleId: moduleId,
        },
      },
      select: { favorited: true },
    });

    return record?.favorited ?? false;
  }

  async addFavorite(userId: string, moduleId: number): Promise<void> {
    await this.prisma.userModules.upsert({
      where: {
        userId_moduleId: {
          userId,
          moduleId: moduleId,
        },
      },
      create: {
        userId,
        moduleId: moduleId,
        favorited: true,
        recommended: false,
      },
      update: {
        favorited: true,
      },
    });
  }

  async removeFavorite(userId: string, moduleId: number): Promise<void> {
    const existing = await this.prisma.userModules.findUnique({
      where: {
        userId_moduleId: {
          userId,
          moduleId: moduleId,
        },
      },
    });

    if (!existing) return;

    // If only favorited is false, update the row to set favorited to false
    if (existing.recommended) {
      await this.prisma.userModules.update({
        where: {
          userId_moduleId: {
            userId,
            moduleId: moduleId,
          },
        },
        data: { favorited: false },
      });
    }
    // If recommended is also false, delete the row entirely
    else {
      await this.prisma.userModules.delete({
        where: {
          userId_moduleId: {
            userId,
            moduleId: moduleId,
          },
        },
      });
    }
  }

  // ==========================================
  // Recommended Methods
  // ==========================================

  async findRecommendedByUserId(userId: string): Promise<UserRecommended[]> {
    const rows = await this.prisma.userModules.findMany({
      where: { userId, recommended: true },
      orderBy: { updatedAt: 'desc' },
    });

    return rows.map((r) => ({
      id: r.id,
      userId: r.userId,
      moduleId: r.moduleId,
      recommendationReason: r.recommendationReason,
      createdAt: r.createdAt,
    }));
  }

  async setRecommendedModules(
    userId: string,
    modules: Array<{ moduleId: number; motivation?: string }>,
  ): Promise<void> {
    const uniqueModules = Array.from(
      new Map(
        modules
          .filter(
            (m) =>
              typeof m.moduleId === 'number' &&
              Number.isInteger(m.moduleId) &&
              m.moduleId > 0,
          )
          .map((m) => [m.moduleId, m]),
      ).values(),
    );

    await this.prisma.$transaction(async (tx) => {
      // 1) First, set all existing recommended modules to false and clear their motivation
      await tx.userModules.updateMany({
        where: { userId, recommended: true },
        data: { recommended: false, recommendationReason: null },
      });

      // 2) Delete rows where both favorited and recommended are false
      await tx.userModules.deleteMany({
        where: {
          userId,
          favorited: false,
          recommended: false,
        },
      });

      // 3) Create or update the new recommended modules with their motivations
      if (uniqueModules.length > 0) {
        for (const module of uniqueModules) {
          await tx.userModules.upsert({
            where: {
              userId_moduleId: {
                userId,
                moduleId: module.moduleId,
              },
            },
            create: {
              userId,
              moduleId: module.moduleId,
              recommended: true,
              recommendationReason: module.motivation || null,
              favorited: false,
            },
            update: {
              recommended: true,
              recommendationReason: module.motivation || null,
            },
          });
        }
      }
    });
  }
}
