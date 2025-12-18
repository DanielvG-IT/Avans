import { ModuleTag } from '@/domain/moduletags/moduletag.model';
export interface IModuleTagRepository {
  getAllModuleTags(): Promise<ModuleTag[]>;
}
