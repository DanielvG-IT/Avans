import { Inject, Injectable, Post } from '@nestjs/common';
import { IModuleTagService } from '../ports/moduletag.port';
import { IModuleTagRepository } from '@/domain/moduletags/moduletag-repository.interface';
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
  async createModuleTag(name: string): Promise<ModuleTag> {
    return await this.moduleTagRepository.createModuleTag(name);
  }
}
