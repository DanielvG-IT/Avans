import { SessionData } from '@/types/session.types';
import {
  UnauthorizedException,
  BadRequestException,
  ParseIntPipe,
  Controller,
  HttpStatus,
  HttpCode,
  Session,
  Inject,
  Param,
  Body,
  Post,
  Get,
} from '@nestjs/common';

// -- imports for modules --
import { IModuleService } from '@/application/ports/module.port';
import { moduleDetail, Module } from '@/domain/modules/module.model';
import { Location } from '@/domain/locations/location.model';
import { CreateModuleTagDto } from '@/presentation/dtos/moduleTag.dto';
import { CreateModuleDTO } from '@/presentation/dtos/module.dto';
import { ModuleTag } from '@/domain/moduletags/moduletag.model';

@Controller('modules')
export class ModulesController {
  constructor(
    @Inject('SERVICE.MODULE') private readonly moduleService: IModuleService,
  ) {}

  @Get()
  @HttpCode(HttpStatus.OK)
  async getModules(
    @Session() session: SessionData,
  ): Promise<{ modules: Module[] }> {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    return {
      modules: await this.moduleService.getAllModules(),
    };
  }

  @Get('locations')
  @HttpCode(HttpStatus.OK)
  async getAllLocation(): Promise<{ locations: Location[] }> {
    return {
      locations: await this.moduleService.getAllLocation(),
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
  @HttpCode(HttpStatus.CREATED)
  async createModuleTag(
    @Body() dto: CreateModuleTagDto,
    @Session() session: SessionData,
  ): Promise<{ moduleTag: ModuleTag }> {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }
    if (session.user.role === 'STUDENT') {
      throw new UnauthorizedException('Only admins can create module tags');
    }

    const moduleTag = await this.moduleService.createModuleTag(dto.tag);
    return { moduleTag };
  }

  @Get(':id')
  @HttpCode(HttpStatus.OK)
  async getModuleById(
    @Session() session: SessionData,
    @Param('id', ParseIntPipe) id: number,
  ): Promise<{ module: moduleDetail }> {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    const module = await this.moduleService.findById(id);
    if (!module) {
      throw new BadRequestException('Module not found');
    }

    return { module };
  }
  @Post()
  @HttpCode(HttpStatus.CREATED)
  async createModule(
    @Session() session: SessionData,
    @Body() moduleData: CreateModuleDTO,
  ): Promise<{ module: moduleDetail }> {
    if (!session || !session.user) {
      throw new UnauthorizedException('No active session');
    }

    if (session.user.role === 'STUDENT') {
      throw new UnauthorizedException('Only admins can create modules');
    }

    if (moduleData.studyCredits !== 15 && moduleData.studyCredits !== 30) {
      throw new BadRequestException('Study credits must be 15 or 30');
    }

    if (moduleData.level !== 'NLQF5' && moduleData.level !== 'NLQF6') {
      throw new BadRequestException('Level must be NLQF5 or NLQF6');
    }

    const module = await this.moduleService.createModule(moduleData);
    if (!module) {
      throw new BadRequestException('Failed to create module');
    }

    return { module };
  }
}
