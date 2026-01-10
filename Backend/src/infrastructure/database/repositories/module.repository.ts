import { Injectable } from '@nestjs/common';
import type {
  createModule,
  Module,
  moduleDetail,
} from '@/domain/module/module.model';
import { PrismaService } from '../prisma';
import { IModuleRepository } from '@/domain/module/module-repository.interface';

@Injectable()
export class ModuleRepository implements IModuleRepository {
  constructor(private readonly prisma: PrismaService) {}

  async getAllModules(): Promise<Module[]> {
    const modules = await this.prisma.module.findMany({
      include: {
        moduleTags: {
          include: { ModuleTag: true },
        },
      },
    });

    return modules.map((mod) => ({
      id: mod.id,
      name: mod.name,
      shortdescription: mod.shortDescription ?? '',
      studyCredits: mod.studyCredits,
      level: mod.level,
      startDate: mod.startDate,
      location: [],
    }));
  }
  async findById(id: number): Promise<moduleDetail> {
    const mod = await this.prisma.module.findUnique({
      where: { id },
      include: {
        location: {
          include: { Location: true },
        },
        moduleTags: {
          include: { ModuleTag: true },
        },
      },
    });
    if (!mod) {
      throw new Error('Module not found');
    }
    return {
      id: mod.id,
      name: mod.name,
      description: mod.description ?? '',
      content: mod.content ?? '',
      level: mod.level,
      studyCredits: mod.studyCredits,
      startDate: mod.startDate,
      location: mod.location.map((cml) => ({
        id: cml.Location.id,
        name: cml.Location.name,
      })),
      moduleTags: mod.moduleTags.map((cmt) => ({
        id: cmt.ModuleTag.id,
        name: cmt.ModuleTag.name,
      })),
      learningOutcomes: mod.learningOutcomes!,
      availableSpots: mod.availableSpots,
    } as moduleDetail;
  }
  async createModule(module: createModule): Promise<moduleDetail> {
    const created = await this.prisma.$transaction(async (tx) => {
      const baseModule = await tx.module.create({
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
      if (module.location.length > 0) {
        await tx.moduleLocation.createMany({
          data: module.location.map((loc) => ({
            moduleId: baseModule.id,
            locationId: loc.id,
          })),
        });
      }

      if (module.moduleTags.length > 0) {
        await tx.moduleTags.createMany({
          data: module.moduleTags.map((tag) => ({
            moduleId: baseModule.id,
            moduleTagId: tag.id,
          })),
        });
      }

      return tx.module.findUnique({
        where: { id: baseModule.id },
        include: {
          location: { include: { Location: true } },
          moduleTags: { include: { ModuleTag: true } },
        },
      });
    });

    if (!created) {
      throw new Error('Failed to create module');
    }

    return {
      id: created.id,
      name: created.name,
      description: created.description ?? '',
      content: created.content ?? '',
      level: created.level,
      studyCredits: created.studyCredits,
      startDate: created.startDate,
      location: created.location.map((cml) => ({
        id: cml.Location.id,
        name: cml.Location.name,
      })),
      moduleTags: created.moduleTags.map((cmt) => ({
        id: cmt.ModuleTag.id,
        name: cmt.ModuleTag.name,
      })),
      learningOutcomes: created.learningOutcomes ?? '',
      availableSpots: created.availableSpots,
    } as moduleDetail;
  }
}
