import { ModuleTag } from '@/domain/moduletags/moduletag.model';
export interface IModuleTagRepository {
  getAllModuleTags(): Promise<ModuleTag[]>;
  createModuleTag(name: string): Promise<ModuleTag>;
}
