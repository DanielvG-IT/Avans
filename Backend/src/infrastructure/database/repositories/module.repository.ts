import { Injectable, NotFoundException } from '@nestjs/common';
import type {
  CreateModule,
  UpdateModule,
  Module,
  ModuleDetail,
} from '@/domain/module/module.model';
import { PrismaService } from '../prisma';
import { IModuleRepository } from '@/domain/module/module-repository.interface';

@Injectable()
export class ModuleRepository implements IModuleRepository {
  constructor(private readonly prisma: PrismaService) {}

  async getAllModules(): Promise<Module[]> {
    const modules = await this.prisma.module.findMany({
      include: {
        location: {
          include: { Location: true },
        },
        moduleTags: {
          include: { ModuleTag: true },
        },
      },
    });

    return modules.map((mod) => ({
      id: mod.id,
      name: mod.name,
      shortDescription: mod.shortDescription ?? '',
      studyCredits: mod.studyCredits,
      level: mod.level,
      startDate: mod.startDate,
      location: mod.location.map((cml) => ({
        id: cml.Location.id,
        name: cml.Location.name,
      })),
    }));
  }

  async findManyByIds(ids: number[]): Promise<ModuleDetail[]> {
    const uniqueIds = Array.from(
      new Set(ids.filter((id) => Number.isInteger(id) && id > 0)),
    );

    if (uniqueIds.length === 0) return [];

    const modules = await this.prisma.module.findMany({
      where: { id: { in: uniqueIds } },
      include: {
        location: {
          include: { Location: true },
        },
        moduleTags: {
          include: { ModuleTag: true },
        },
      },
    });

    if (modules.length !== uniqueIds.length) {
      const missing = uniqueIds.filter(
        (id) => !modules.some((mod) => mod.id === id),
      );
      throw new NotFoundException(`Module(s) not found: ${missing.join(', ')}`);
    }

    return modules.map(
      (mod) =>
        ({
          id: mod.id,
          name: mod.name,
          shortDescription: mod.shortDescription ?? '',
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
          learningOutcomes: mod.learningOutcomes ?? '',
          availableSpots: mod.availableSpots,
        }) as ModuleDetail,
    );
  }
  async findById(id: number): Promise<ModuleDetail> {
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
      throw new NotFoundException('Module not found');
    }
    return {
      id: mod.id,
      name: mod.name,
      shortDescription: mod.shortDescription ?? '',
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
      learningOutcomes: mod.learningOutcomes ?? '',
      availableSpots: mod.availableSpots,
    } as ModuleDetail;
  }
  async createModule(module: CreateModule): Promise<ModuleDetail> {
    const created = await this.prisma.$transaction(async (tx) => {
      const baseModule = await tx.module.create({
        data: {
          name: module.name,
          shortDescription: module.shortDescription,
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
      shortDescription: created.shortDescription ?? '',
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
    } as ModuleDetail;
  }

  async updateModule(
    id: number,
    module: UpdateModule,
  ): Promise<ModuleDetail> {
    // Verify module exists
    const existingModule = await this.prisma.module.findUnique({
      where: { id },
    });
    if (!existingModule) {
      throw new NotFoundException('Module not found');
    }

    const updated = await this.prisma.$transaction(async (tx) => {
      // Update base module
      const baseModule = await tx.module.update({
        where: { id },
        data: {
          name: module.name,
          shortDescription: module.shortDescription,
          description: module.description,
          content: module.content,
          level: module.level,
          studyCredits: module.studyCredits,
          startDate: module.startDate,
          availableSpots: module.availableSpots,
          learningOutcomes: module.learningOutcomes,
        },
      });

      // Delete existing locations and tags
      await tx.moduleLocation.deleteMany({ where: { moduleId: id } });
      await tx.moduleTags.deleteMany({ where: { moduleId: id } });

      // Add new locations
      if (module.location.length > 0) {
        await tx.moduleLocation.createMany({
          data: module.location.map((loc) => ({
            moduleId: baseModule.id,
            locationId: loc.id,
          })),
        });
      }

      // Add new tags
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

    if (!updated) {
      throw new Error('Failed to update module');
    }

    return {
      id: updated.id,
      name: updated.name,
      shortDescription: updated.shortDescription ?? '',
      description: updated.description ?? '',
      content: updated.content ?? '',
      level: updated.level,
      studyCredits: updated.studyCredits,
      startDate: updated.startDate,
      location: updated.location.map((cml) => ({
        id: cml.Location.id,
        name: cml.Location.name,
      })),
      moduleTags: updated.moduleTags.map((cmt) => ({
        id: cmt.ModuleTag.id,
        name: cmt.ModuleTag.name,
      })),
      learningOutcomes: updated.learningOutcomes ?? '',
      availableSpots: updated.availableSpots,
    } as ModuleDetail;
  }

  async deleteModule(id: number): Promise<void> {
    // Verify module exists
    const module = await this.prisma.module.findUnique({ where: { id } });
    if (!module) {
      throw new NotFoundException('Module not found');
    }

    // Delete module (cascade will handle related records due to onDelete: Cascade in schema)
    await this.prisma.module.delete({ where: { id } });
  }
}
