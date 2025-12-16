import { Injectable } from '@nestjs/common';
import type { Module } from '@/domain/modules/module.model';
import { PrismaService } from '../database/prisma';
import { IModuleRepository } from '@/domain/modules/module-repository.interface';

@Injectable()
export class ChoiceModulesRepository implements IModuleRepository {
  constructor(private readonly prisma: PrismaService) {}

  async getAllModules(): Promise<Module[]> {
    const modules = await this.prisma.choiceModule.findMany({
      include: {
        location: {
          include: { Location: true },
        },
        moduleTags: {
          include: { ModuleTags: true },
        },
      },
    });

    return modules.map((mod) => ({
      id: mod.id,
      name: mod.name!,
      shortdescription: mod.shortDescription ?? '',
      studyCredits: mod.studyCredits!,
      level: mod.level!,
      startDate: mod.startDate, // string -> Date
      location: mod.location.map((cml) => ({
        id: cml.Location.id,
        name: cml.Location.name,
      })),
    }));
  }
}
