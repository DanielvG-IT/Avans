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

@Controller('modules')
export class ModulesController {
  private readonly moduleService: ModuleService;

  constructor(@Inject('SERVICE.MODULE') _moduleService: ModuleService) {
    this.moduleService = _moduleService;
  }
  @Get()
  async getModules(@Session() session: SessionData): Promise<any> {
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
  ): Promise<any> {
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
    @Body() moduleData: createModule,
  ): Promise<any> {
    if (!session) {
      throw new UnauthorizedException('No active session');
    }
    if (!session.user || session.user.role === 'STUDENT') {
      console.log(session.user);
      return { message: 'Unauthorized' };
    }

    if (!session.user || session.user.role === 'STUDENT') {
      console.log(session.user);
      return { message: `Unauthorized ${session.user}` };
    }
    const module = await this.moduleService.createModule(moduleData);
    if (!module) {
      console.log(module);
      return { message: `Failed to create module` };
    }

    return {
      module,
    };
  }
}
