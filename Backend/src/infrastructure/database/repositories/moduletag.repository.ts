import { Injectable } from '@nestjs/common';
import { IModuleTagRepository } from '@/domain/moduletags/moduletag-repository';
import { ModuleTag } from '@/domain/moduletags/moduletag.model';
import { PrismaService } from '@/infrastructure/database/prisma';

@Injectable()
export class ModuleTagRepository implements IModuleTagRepository {
  constructor(private readonly prisma: PrismaService) {}
  async getAllModuleTags(): Promise<ModuleTag[]> {
    const moduleTags = await this.prisma.moduleTags.findMany();
    return moduleTags.map((mt) => ({
      id: mt.id,
      name: mt.name,
    }));
  }
}
