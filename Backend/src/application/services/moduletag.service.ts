import { Inject, Injectable } from '@nestjs/common';
import { IModuleTagService } from '../ports/moduletag.port';
import { IModuleTagRepository } from '@/domain/moduletags/moduletag-repository';
import { ModuleTag } from '@/domain/moduletags/moduletag.model';

@Injectable()
export class ModuleTagService implements IModuleTagService {
  constructor(
    @Inject('REPO.MODULETAG')
    private readonly moduleTagRepository: IModuleTagRepository,
  ) {}

  async getAllModuleTags(): Promise<ModuleTag[]> {
    return await this.moduleTagRepository.getAllModuleTags();
  }
}
