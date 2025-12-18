import { ModuleTag } from '@/domain/moduletags/moduletag.model';
export interface IModuleTagService {
  getAllModuleTags(): Promise<ModuleTag[]>;
}
