import { IUserFavoritesRepository } from '@/domain/userfavorites/userfavorites-repository.interface';
import { UserFavorite } from '@/domain/userfavorites/userfavorites.model';
import { Injectable } from '@nestjs/common';
import { PrismaService } from '../prisma';

@Injectable()
export class UserFavoritesRepository implements IUserFavoritesRepository {
  constructor(private readonly prisma: PrismaService) {}

  private get userModulesModel() {
    const client = this.prisma as unknown as {
      userModules?: any;
      userRecommended?: any;
    };

    return client.userModules ?? client.userRecommended;
  }

  async findByUserId(userId: string): Promise<UserFavorite[]> {
    const favorites = await this.userModulesModel.findMany({
      where: { userId, favorited: true },
      orderBy: { updatedAt: 'desc' },
    });

    return favorites.map((fav) => ({
      id: fav.id,
      userId: fav.userId,
      choiceModuleId: fav.choiceModuleId,
      createdAt: fav.createdAt,
    }));
  }

  async add(userId: string, itemId: number): Promise<void> {
    await this.userModulesModel.upsert({
      where: {
        userId_choiceModuleId: {
          userId,
          choiceModuleId: itemId,
        },
      },
      create: {
        userId,
        choiceModuleId: itemId,
        favorited: true,
        recommended: false,
      },
      update: {
        favorited: true,
      },
    });
  }

  async remove(userId: string, itemId: number): Promise<void> {
    await this.prisma.$transaction([
      this.userModulesModel.updateMany({
        where: {
          userId,
          choiceModuleId: itemId,
        },
        data: {
          favorited: false,
        },
      }),
      this.userModulesModel.deleteMany({
        where: {
          userId,
          choiceModuleId: itemId,
          favorited: false,
          recommended: false,
        },
      }),
    ]);
  }

  async exists(userId: string, itemId: number): Promise<boolean> {
    const favorite = await this.userModulesModel.findFirst({
      where: {
        userId,
        choiceModuleId: itemId,
        favorited: true,
      },
    });

    return favorite !== null;
  }
}
