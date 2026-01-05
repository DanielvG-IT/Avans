import { ModuleTag } from '@/domain/moduletags/moduletag.model';
export interface IModuleTagService {
  getAllModuleTags(): Promise<ModuleTag[]>;
  createModuleTag(name: string): Promise<ModuleTag>;
}
