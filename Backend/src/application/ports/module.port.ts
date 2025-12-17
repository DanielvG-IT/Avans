import { Module } from '@/domain/modules/module.model';
export interface IModuleService {
  getAllModules(): Promise<Module[]>;
  //   findById(id: string): Promise<any>;
}
