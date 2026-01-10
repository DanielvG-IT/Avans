import {
  Module,
  moduleDetail,
  createModule,
} from '@/domain/module/module.model';
import { Location } from '@/domain/location/location.model';
import { ModuleTag } from '@/domain/moduletag/moduletag.model';

export interface IModuleService {
  // Module methods
  getAllModules(): Promise<Module[]>;
  findById(id: number): Promise<moduleDetail>;
  createModule(module: createModule): Promise<moduleDetail>;

  // Location methods
  getAllLocations(): Promise<Location[]>;

  // Module tag methods
  getAllModuleTags(): Promise<ModuleTag[]>;
  createModuleTag(name: string): Promise<ModuleTag>;
}
