import {
  Module,
  ModuleDetail,
  CreateModule,
} from '@/domain/module/module.model';
export interface IModuleRepository {
  getAllModules(): Promise<Module[]>;
  findById(id: number): Promise<ModuleDetail>;
  createModule(module: CreateModule): Promise<ModuleDetail>;
  deleteModule(id: number): Promise<void>;
}
