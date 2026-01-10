import { Injectable } from '@nestjs/common';
import { IModuleTagRepository } from '@/domain/moduletag/moduletag-repository.interface';
import { ModuleTag } from '@/domain/moduletag/moduletag.model';
import { PrismaService } from '@/infrastructure/database/prisma';

@Injectable()
export class ModuleTagRepository implements IModuleTagRepository {
  constructor(private readonly prisma: PrismaService) {}
  async getAllModuleTags(): Promise<ModuleTag[]> {
    const moduleTags = await this.prisma.moduleTag.findMany();
    return moduleTags.map((mt) => ({
      id: mt.id,
      name: mt.name,
    }));
  }
  async createModuleTag(name: string): Promise<ModuleTag> {
    const moduleTag = await this.prisma.moduleTag.create({
      data: {
        name,
      },
    });
    return {
      id: moduleTag.id,
      name: moduleTag.name,
    };
  }
}
