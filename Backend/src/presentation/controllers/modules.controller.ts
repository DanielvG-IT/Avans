import { Controller, Get, Session } from '@nestjs/common';
import { SessionData } from 'express-session';
import { ModuleService } from '@/application/services/module.service';
import { Inject } from '@nestjs/common';
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
}
