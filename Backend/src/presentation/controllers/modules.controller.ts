import {
  Controller,
  Get,
  Param,
  Post,
  Session,
  UnauthorizedException,
  Body,
} from '@nestjs/common';
import { SessionData } from '@/types/session.types';
import { ModuleService } from '@/application/services/module.service';
import { Inject } from '@nestjs/common';
import {
  Module,
  moduleDetail,
  createModule,
} from '@/domain/modules/module.model';
import { CreateModuleDTO } from '@/presentation/dtos/choiceModule.dto';

@Controller('modules')
export class ModulesController {
  private readonly moduleService: ModuleService;

  constructor(@Inject('SERVICE.MODULE') _moduleService: ModuleService) {
    this.moduleService = _moduleService;
  }
  @Get()
  async getModules(
    @Session() session: SessionData,
  ): Promise<{ modules: Module[] }> {
    if (!session) {
      throw new Error('No active session');
    }

    return {
      modules: await this.moduleService.getAllModules(),
    };
  }
  @Get(':id')
  async getModuleById(
    @Session() session: SessionData,
    @Param('id') id: string,
  ): Promise<{ module: moduleDetail }> {
    if (!session) {
      throw new Error('No active session');
    }

    return {
      module: await this.moduleService.findById(id),
    };
  }
  @Post()
  async createModule(
    @Session() session: SessionData,
    @Body() moduleData: CreateModuleDTO,
  ): Promise<{ module: moduleDetail } | { message: string }> {
    if (!session) {
      throw new UnauthorizedException('No active session');
    }

    if (!session.user || session.user.role === 'STUDENT') {
      throw new UnauthorizedException('Only admins can create modules');
    }
    if (!moduleData.name || moduleData.name.trim() === '') {
      throw new UnauthorizedException('Module name is required');
    }
    if (!moduleData.description || moduleData.description.trim() === '') {
      throw new UnauthorizedException('Module description is required');
    }
    if (!moduleData.content || moduleData.content.trim() === '') {
      throw new UnauthorizedException('Module content is required');
    }
    if (!moduleData.startDate || new Date(moduleData.startDate) < new Date()) {
      throw new UnauthorizedException('(Valid) Module start date is required');
    }
    if (!moduleData.location || moduleData.location.length === 0) {
      throw new UnauthorizedException('At least one location is required');
    }
    if (!moduleData.moduleTags || moduleData.moduleTags.length === 0) {
      throw new UnauthorizedException('At least one module tag is required');
    }

    if (
      !moduleData.learningOutcomes ||
      moduleData.learningOutcomes.length === 0
    ) {
      throw new UnauthorizedException(
        'At least one learning outcome is required',
      );
    }

    if (
      moduleData.studyCredits === null ||
      (moduleData.studyCredits !== 15 && moduleData.studyCredits !== 30)
    ) {
      throw new UnauthorizedException('Study credits must be 15 or 30');
    }

    if (moduleData.level !== 'NFQL5' && moduleData.level !== 'NFQL6') {
      throw new UnauthorizedException('Level must be NFQL5 or NFQL6');
    }

    const module = await this.moduleService.createModule(moduleData);
    if (!module) {
      return { message: 'Failed to create module' };
    }

    return {
      module,
    };
  }
}
