import { IUserFavoritesRepository } from '@/domain/userfavorites/userfavorites-repository.interface';
import { IUserRecommendedRepository } from '@/domain/userrecommended/userrecommended-repository.interface';
import { UserFavorite } from '@/domain/userfavorites/userfavorites.model';
import { Injectable } from '@nestjs/common';
import { PrismaService } from '../prisma';

/**
 * Unified repository for UserModules table
 * Handles both favorites and recommended modules since they share the same table
 */
@Injectable()
export class UserModulesRepository
  implements IUserFavoritesRepository, IUserRecommendedRepository
{
  constructor(private readonly prisma: PrismaService) {}

  // ==========================================
  // Favorites Methods
  // ==========================================

  async findByUserId(userId: string): Promise<UserFavorite[]> {
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

  async exists(userId: string, moduleId: number): Promise<boolean> {
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

  async add(userId: string, moduleId: number): Promise<void> {
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

  async remove(userId: string, moduleId: number): Promise<void> {
    const existing = await this.prisma.userModules.findUnique({
      where: {
        userId_moduleId: {
          userId,
          moduleId: moduleId,
        },
      },
    });

    if (!existing) return;

    // If recommended is also false, delete the row entirely
    if (!existing.recommended) {
      await this.prisma.userModules.delete({
        where: {
          userId_moduleId: {
            userId,
            moduleId: moduleId,
          },
        },
      });
    } else {
      // Otherwise just unset favorited
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
  }

  // ==========================================
  // Recommended Methods
  // ==========================================

  async getRecommendedModuleIds(userId: string): Promise<number[]> {
    const rows = await this.prisma.userModules.findMany({
      where: { userId, recommended: true },
      select: { moduleId: true },
      orderBy: { updatedAt: 'desc' },
    });

    return rows.map((r) => r.moduleId);
  }

  async setRecommendedModules(
    userId: string,
    moduleIds: number[],
  ): Promise<void> {
    const uniqueIds = Array.from(new Set(moduleIds)).filter(
      (id) => typeof id === 'number' && Number.isInteger(id) && id > 0,
    );

    await this.prisma.$transaction(async () => {
      // 1) Ensure rows exist + set recommended=true for selected ids
      if (uniqueIds.length > 0) {
        await this.prisma.userModules.updateMany({
          where: { userId, moduleId: { in: uniqueIds } },
          data: { recommended: true },
        });

        await this.prisma.userModules.createMany({
          data: uniqueIds.map((moduleId) => ({
            userId,
            moduleId,
            recommended: true,
          })),
          skipDuplicates: true,
        });
      }

      // 2) Unset recommended for anything NOT in the submitted list
      if (uniqueIds.length === 0) {
        await this.prisma.userModules.updateMany({
          where: { userId, recommended: true },
          data: { recommended: false },
        });
      } else {
        await this.prisma.userModules.updateMany({
          where: {
            userId,
            recommended: true,
            moduleId: { notIn: uniqueIds },
          },
          data: { recommended: false },
        });
      }

      // 3) Cleanup rows where both flags are false
      await this.prisma.userModules.deleteMany({
        where: {
          userId,
          favorited: false,
          recommended: false,
        },
      });
    });
  }
}
