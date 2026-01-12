import { Injectable, Inject } from '@nestjs/common';

// -- imports for modules --
import { type IModuleRepository } from '@/domain/module/module-repository.interface';
import { IModuleService } from '@/application/ports/module.port';
import {
  Module,
  ModuleDetail,
  CreateModule,
} from '@/domain/module/module.model';

// -- imports for locations --
import { ILocationRepository } from '@/domain/location/location-repository.interface';
import { Location } from '@/domain/location/location.model';

// -- imports for module tags --
import { IModuleTagRepository } from '@/domain/moduletag/moduletag-repository.interface';
import { ModuleTag } from '@/domain/moduletag/moduletag.model';

@Injectable()
export class ModuleService implements IModuleService {
  constructor(
    @Inject('REPO.MODULE') private readonly moduleRepository: IModuleRepository,
    @Inject('REPO.LOCATION')
    private readonly locationRepository: ILocationRepository,
    @Inject('REPO.MODULETAG')
    private readonly moduleTagRepository: IModuleTagRepository,
  ) {}

  // ==========================================
  // Module Methods
  // ==========================================
  async getAllModules(): Promise<Module[]> {
    return await this.moduleRepository.getAllModules();
  }

  async findById(id: number): Promise<ModuleDetail> {
    return await this.moduleRepository.findById(id);
  }

  async createModule(module: CreateModule): Promise<ModuleDetail> {
    return await this.moduleRepository.createModule(module);
  }

  async deleteModule(id: number): Promise<void> {
    return await this.moduleRepository.deleteModule(id);
  }

  // ==========================================
  // Location Methods
  // ==========================================
  async getAllLocations(): Promise<Location[]> {
    return await this.locationRepository.getAllLocations();
  }

  // ==========================================
  // Module Tag Methods
  // ==========================================
  async getAllModuleTags(): Promise<ModuleTag[]> {
    return await this.moduleTagRepository.getAllModuleTags();
  }

  async createModuleTag(name: string): Promise<ModuleTag> {
    return await this.moduleTagRepository.createModuleTag(name);
  }
}
