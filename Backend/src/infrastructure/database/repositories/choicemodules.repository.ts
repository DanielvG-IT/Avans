import { Injectable } from '@nestjs/common';
import type {
  createModule,
  Module,
  moduleDetail,
} from '@/domain/modules/module.model';
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
      name: mod.name,
      shortdescription: mod.shortDescription ?? '',
      studyCredits: mod.studyCredits,
      level: mod.level,
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
      name: mod.name,
      description: mod.description!,
      content: mod.content!,
      level: mod.level,
      studyCredits: mod.studyCredits,
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
      availableSpots: mod.availableSpots,
    } as moduleDetail;
  }
  async createModule(module: createModule): Promise<moduleDetail> {
    const created = await this.prisma.$transaction(async (tx) => {
      const baseModule = await tx.choiceModule.create({
        data: {
          name: module.name,
          shortDescription: module.shortdescription,
          description: module.description,
          content: module.content,
          level: module.level,
          studyCredits: module.studyCredits,
          startDate: module.startDate,
          availableSpots: module.availableSpots,
          learningOutcomes: module.learningOutcomes,
        },
      });
      console.log('Created base module:', baseModule);

      if (module.location.length > 0) {
        await tx.choiceModuleLocation.createMany({
          data: module.location.map((loc) => ({
            choiceModuleId: baseModule.id,
            locationId: loc.id,
          })),
        });
      }

      if (module.moduleTags.length > 0) {
        await tx.choiceModuleTags.createMany({
          data: module.moduleTags.map((tag) => ({
            choiceModuleId: baseModule.id,
            moduleTagsId: tag.id,
          })),
        });
      }

      return tx.choiceModule.findUnique({
        where: { id: baseModule.id },
        include: {
          location: { include: { Location: true } },
          moduleTags: { include: { ModuleTags: true } },
        },
      });
    });

    if (!created) {
      throw new Error('Failed to create module');
    }

    return {
      id: created.id,
      name: created.name,
      description: created.description!,
      content: created.content!,
      level: created.level,
      studyCredits: created.studyCredits,
      startDate: created.startDate,
      location: created.location.map((cml) => ({
        id: cml.Location.id,
        name: cml.Location.name,
      })),
      moduleTags: created.moduleTags.map((cmt) => ({
        id: cmt.ModuleTags.id,
        name: cmt.ModuleTags.name,
      })),
      learningOutcomes: created.learningOutcomes!,
      availableSpots: created.availableSpots,
    } as moduleDetail;
  }
}
