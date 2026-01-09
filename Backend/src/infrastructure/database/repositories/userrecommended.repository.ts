import { Injectable } from '@nestjs/common';
import { PrismaService } from '../prisma';
import { IUserRecommendedRepository } from '@/domain/userrecommended/userrecommended-repository.interface';

@Injectable()
export class UserRecommendedRepository implements IUserRecommendedRepository {
  constructor(private readonly prisma: PrismaService) {}

  private get userModulesModel() {
    const client = this.prisma as unknown as {
      userModules?: any;
      userRecommended?: any;
    };

    return client.userModules ?? client.userRecommended;
  }

  async getRecommendedModuleIds(userId: string): Promise<number[]> {
    const rows = await this.userModulesModel.findMany({
      where: { userId, recommended: true },
      select: { choiceModuleId: true },
      orderBy: { updatedAt: 'desc' },
    });

    return rows.map((r: { choiceModuleId: number }) => r.choiceModuleId);
  }

  async setRecommendedModules(userId: string, moduleIds: number[]): Promise<void> {
    const uniqueIds = Array.from(new Set(moduleIds)).filter(
      (id) => typeof id === 'number' && Number.isInteger(id) && id > 0,
    );

    await this.prisma.$transaction(async () => {
      // 1) Ensure rows exist + set recommended=true for selected ids
      if (uniqueIds.length > 0) {
        await this.userModulesModel.updateMany({
          where: { userId, choiceModuleId: { in: uniqueIds } },
          data: { recommended: true },
        });

        await this.userModulesModel.createMany({
          data: uniqueIds.map((choiceModuleId) => ({
            userId,
            choiceModuleId,
            recommended: true,
          })),
          skipDuplicates: true,
        });
      }

      // 2) Unset recommended for anything NOT in the submitted list
      if (uniqueIds.length === 0) {
        await this.userModulesModel.updateMany({
          where: { userId, recommended: true },
          data: { recommended: false },
        });
      } else {
        await this.userModulesModel.updateMany({
          where: {
            userId,
            recommended: true,
            choiceModuleId: { notIn: uniqueIds },
          },
          data: { recommended: false },
        });
      }

      // 3) Cleanup rows where both flags are false
      await this.userModulesModel.deleteMany({
        where: {
          userId,
          favorited: false,
          recommended: false,
        },
      });
    });
  }
}
