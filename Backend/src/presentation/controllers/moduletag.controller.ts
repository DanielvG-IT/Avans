import {
  Controller,
  Inject,
  Get,
  Post,
  Body,
  Session,
  UnauthorizedException,
} from '@nestjs/common';
import { IModuleTagService } from '@/application/ports/moduletag.port';
import { ModuleTag } from '@/domain/moduletags/moduletag.model';
import { SessionData } from '@/types/session.types';

@Controller('moduletags')
export class ModuleTagController {
  private readonly moduleTagService: IModuleTagService;
  constructor(
    @Inject('SERVICE.MODULETAG') moduleTagService: IModuleTagService,
  ) {
    this.moduleTagService = moduleTagService;
  }
  @Get()
  async getAllModuleTags(): Promise<ModuleTag[]> {
    return await this.moduleTagService.getAllModuleTags();
  }
  @Post()
  async createModuleTag(
    @Body('tag') name: string,
    @Session() session: SessionData,
  ): Promise<ModuleTag> {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }
    if (session.user.role == 'STUDENT') {
      throw new UnauthorizedException('Unauthorized');
    }

    if (!name || name.trim() === '') {
      throw new UnauthorizedException('Tag name is required');
    }
    console.log(name);
    return await this.moduleTagService.createModuleTag(name);
  }
}
