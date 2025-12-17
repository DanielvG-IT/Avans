import { Injectable } from '@nestjs/common';
import type { Module, moduleDetail } from '@/domain/modules/module.model';
import { PrismaService } from '../prisma';
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
  async findById(id: string): Promise<moduleDetail> {
    const mod = await this.prisma.choiceModule.findUnique({
      where: { id },
      include: {
        location: {
          include: { Location: true },
        },
        moduleTags: {
          include: { ModuleTags: true },
        },
      },
    });
    if (!mod) {
      throw new Error('Module not found');
    }
    return {
      id: mod.id,
      name: mod.name!,
      description: mod.description!,
      content: mod.content!,
      level: mod.level!,
      studyCredits: mod.studyCredits!,
      startDate: mod.startDate, // string -> Date
      location: mod.location.map((cml) => ({
        id: cml.Location.id,
        name: cml.Location.name,
      })),
      moduleTags: mod.moduleTags.map((cmt) => ({
        id: cmt.ModuleTags.id,
        name: cmt.ModuleTags.name,
      })),
      learningOutcomes: mod.learningOutcomes!,
      availableSpots: mod.availableSpots!,
    } as moduleDetail;
  }
}
