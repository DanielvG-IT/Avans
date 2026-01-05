import { IUserFavoritesRepository } from '@/domain/userfavorites/userfavorites-repository.interface';
import { UserFavorite } from '@/domain/userfavorites/userfavorites.model';
import { Injectable } from '@nestjs/common';
import { PrismaService } from '../prisma';

@Injectable()
export class UserFavoritesRepository implements IUserFavoritesRepository {
  constructor(private readonly prisma: PrismaService) {}

  async findByUserId(userId: string): Promise<UserFavorite[]> {
    const favorites = await this.prisma.userFavorites.findMany({
      where: { userId },
      orderBy: { createdAt: 'desc' },
    });

    return favorites.map((fav) => ({
      id: fav.id,
      userId: fav.userId,
      choiceModuleId: fav.choiceModuleId,
      createdAt: fav.createdAt,
    }));
  }

  async add(userId: string, itemId: string): Promise<void> {
    await this.prisma.userFavorites.create({
      data: {
        userId,
        choiceModuleId: itemId,
      },
    });
  }

  async remove(userId: string, itemId: string): Promise<void> {
    await this.prisma.userFavorites.deleteMany({
      where: {
        userId,
        choiceModuleId: itemId,
      },
    });
  }

  async exists(userId: string, itemId: string): Promise<boolean> {
    const favorite = await this.prisma.userFavorites.findFirst({
      where: {
        userId,
        choiceModuleId: itemId,
      },
    });

    return favorite !== null;
  }
}
