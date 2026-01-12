import {
  Module,
  ModuleDetail,
  CreateModule,
} from '@/domain/module/module.model';
export interface IModuleRepository {
  getAllModules(): Promise<Module[]>;
  findManyByIds(ids: number[]): Promise<ModuleDetail[]>;
  findById(id: number): Promise<ModuleDetail>;
  createModule(module: CreateModule): Promise<ModuleDetail>;
  deleteModule(id: number): Promise<void>;
}
