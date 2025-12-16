import { Module } from '@/domain/modules/module.model';
export interface IModuleRepository {
  getAllModules(): Promise<Module[]>;
  findById(id: string): Promise<any>;
}
