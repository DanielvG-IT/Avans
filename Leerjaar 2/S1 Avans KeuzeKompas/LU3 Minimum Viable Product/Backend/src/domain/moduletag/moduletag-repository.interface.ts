import { ModuleTag } from '@/domain/moduletag/moduletag.model';
export interface IModuleTagRepository {
  getAllModuleTags(): Promise<ModuleTag[]>;
  createModuleTag(name: string): Promise<ModuleTag>;
}
