import {
  Module,
  moduleDetail,
  createModule,
} from '@/domain/modules/module.model';
import { Location } from '@/domain/locations/location.model';
import { ModuleTag } from '@/domain/moduletags/moduletag.model';

export interface IModuleService {
  // Module methods
  getAllModules(): Promise<Module[]>;
  findById(id: number): Promise<moduleDetail>;
  createModule(module: createModule): Promise<moduleDetail>;

  // Location methods
  getAllLocation(): Promise<Location[]>;

  // Module tag methods
  getAllModuleTags(): Promise<ModuleTag[]>;
  createModuleTag(name: string): Promise<ModuleTag>;
}
