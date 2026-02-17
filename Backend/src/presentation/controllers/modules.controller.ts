import { RequireAuth } from '../decorators/auth.decorator';
import { SessionGuard } from '../guards/session.guard';
import {
  ParseIntPipe,
  Controller,
  HttpStatus,
  UseGuards,
  HttpCode,
  Inject,
  Delete,
  Param,
  Body,
  Post,
  Put,
  Get,
} from '@nestjs/common';

// -- imports for modules --
import { CreateModuleTagDto } from '@/presentation/dtos/moduleTag.dto';
import { ModuleDetail, Module } from '@/domain/module/module.model';
import { IModuleService } from '@/application/ports/module.port';
import { CreateModuleDTO, UpdateModuleDTO } from '@/presentation/dtos/module.dto';
import { ModuleTag } from '@/domain/moduletag/moduletag.model';
import { Location } from '@/domain/location/location.model';

@Controller('modules')
@UseGuards(SessionGuard)
export class ModulesController {
  constructor(
    @Inject('SERVICE.MODULE') private readonly moduleService: IModuleService,
  ) {}

  @Get()
  @HttpCode(HttpStatus.OK)
  async getModules(): Promise<{ modules: Module[] }> {
    return {
      modules: await this.moduleService.getAllModules(),
    };
  }

  @Get('locations')
  @HttpCode(HttpStatus.OK)
  async getAllLocations(): Promise<{ locations: Location[] }> {
    return {
      locations: await this.moduleService.getAllLocations(),
    };
  }

  @Get('moduletags')
  @HttpCode(HttpStatus.OK)
  async getAllModuleTags(): Promise<{ moduleTags: ModuleTag[] }> {
    return {
      moduleTags: await this.moduleService.getAllModuleTags(),
    };
  }
  @Post('moduletags')
  @RequireAuth('ADMIN', 'TEACHER')
  @HttpCode(HttpStatus.CREATED)
  async createModuleTag(
    @Body() dto: CreateModuleTagDto,
  ): Promise<{ moduleTag: ModuleTag }> {
    const moduleTag = await this.moduleService.createModuleTag(dto.tag);
    return { moduleTag };
  }

  @Get(':id')
  @HttpCode(HttpStatus.OK)
  async getModuleById(
    @Param('id', ParseIntPipe) id: number,
  ): Promise<{ module: ModuleDetail }> {
    // Repository throws error if module not found, no null check needed
    const module = await this.moduleService.findById(id);
    return { module };
  }

  @Post()
  @RequireAuth('ADMIN', 'TEACHER')
  @HttpCode(HttpStatus.CREATED)
  async createModule(
    @Body() moduleData: CreateModuleDTO,
  ): Promise<{ module: ModuleDetail }> {
    // studyCredits and level validation now handled by DTO validators
    // Repository transaction will either return module or throw error
    const module = await this.moduleService.createModule(moduleData);
    return { module };
  }

  @Put(':id')
  @RequireAuth('ADMIN')
  @HttpCode(HttpStatus.OK)
  async updateModule(
    @Param('id', ParseIntPipe) id: number,
    @Body() moduleData: UpdateModuleDTO,
  ): Promise<{ module: ModuleDetail }> {
    const module = await this.moduleService.updateModule(id, moduleData);
    return { module };
  }

  @Delete(':id')
  @RequireAuth('ADMIN')
  @HttpCode(HttpStatus.OK)
  async deleteModule(
    @Param('id', ParseIntPipe) id: number,
  ): Promise<{ success: boolean }> {
    await this.moduleService.deleteModule(id);
    return { success: true };
  }
}
