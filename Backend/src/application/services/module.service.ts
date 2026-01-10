import { Injectable, Inject } from '@nestjs/common';

// -- imports for modules --
import { type IModuleRepository } from '@/domain/modules/module-repository.interface';
import { IModuleService } from '@/application/ports/module.port';
import {
  Module,
  moduleDetail,
  createModule,
} from '@/domain/modules/module.model';

// -- imports for locations --
import { ILocationRepository } from '@/domain/locations/location-repository.interface';
import { Location } from '@/domain/locations/location.model';

// -- imports for module tags --
import { IModuleTagRepository } from '@/domain/moduletags/moduletag-repository.interface';
import { ModuleTag } from '@/domain/moduletags/moduletag.model';

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

  async findById(id: number): Promise<moduleDetail> {
    return await this.moduleRepository.findById(id);
  }

  async createModule(module: createModule): Promise<moduleDetail> {
    return await this.moduleRepository.createModule(module);
  }

  // ==========================================
  // Location Methods
  // ==========================================
  async getAllLocation(): Promise<Location[]> {
    return await this.locationRepository.getAllLocation();
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
