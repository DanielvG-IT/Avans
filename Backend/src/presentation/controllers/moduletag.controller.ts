import { Controller, Inject, Get } from '@nestjs/common';
import { IModuleTagService } from '@/application/ports/moduletag.port';
import { ModuleTag } from '@/domain/moduletags/moduletag.model';
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
}
