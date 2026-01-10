import {
  Module,
  moduleDetail,
  createModule,
} from '@/domain/module/module.model';
export interface IModuleRepository {
  getAllModules(): Promise<Module[]>;
  findById(id: number): Promise<moduleDetail>;
  createModule(module: createModule): Promise<moduleDetail>;
}
