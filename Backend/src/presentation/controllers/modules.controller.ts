import { RequireAuth } from '../decorators/auth.decorator';
import { SessionGuard } from '../guards/session.guard';
import {
  BadRequestException,
  ParseIntPipe,
  Controller,
  HttpStatus,
  UseGuards,
  HttpCode,
  Inject,
  Param,
  Body,
  Post,
  Get,
} from '@nestjs/common';

// -- imports for modules --
import { IModuleService } from '@/application/ports/module.port';
import { moduleDetail, Module } from '@/domain/module/module.model';
import { Location } from '@/domain/location/location.model';
import { CreateModuleTagDto } from '@/presentation/dtos/moduleTag.dto';
import { CreateModuleDTO } from '@/presentation/dtos/module.dto';
import { ModuleTag } from '@/domain/moduletag/moduletag.model';

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
  ): Promise<{ module: moduleDetail }> {
    // Repository throws error if module not found, no null check needed
    const module = await this.moduleService.findById(id);
    return { module };
  }

  @Post()
  @RequireAuth('ADMIN', 'TEACHER')
  @HttpCode(HttpStatus.CREATED)
  async createModule(
    @Body() moduleData: CreateModuleDTO,
  ): Promise<{ module: moduleDetail }> {
    // studyCredits and level validation now handled by DTO validators
    // Repository transaction will either return module or throw error
    const module = await this.moduleService.createModule(moduleData);
    return { module };
  }
}
