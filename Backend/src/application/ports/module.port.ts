import {
  Module,
  ModuleDetail,
  CreateModule,
} from '@/domain/module/module.model';
import { Location } from '@/domain/location/location.model';
import { ModuleTag } from '@/domain/moduletag/moduletag.model';

export interface IModuleService {
  // Module methods
  getAllModules(): Promise<Module[]>;
  findManyByIds(ids: number[]): Promise<ModuleDetail[]>;
  findById(id: number): Promise<ModuleDetail>;
  createModule(module: CreateModule): Promise<ModuleDetail>;
  deleteModule(id: number): Promise<void>;

  // Location methods
  getAllLocations(): Promise<Location[]>;

  // Module tag methods
  getAllModuleTags(): Promise<ModuleTag[]>;
  createModuleTag(name: string): Promise<ModuleTag>;
}
