import { Injectable, Inject } from '@nestjs/common';
import { IModuleService } from '@/application/ports/module.port';
import { type IModuleRepository } from '@/domain/modules/module-repository.interface';
import {
  Module,
  moduleDetail,
  createModule,
} from '@/domain/modules/module.model';

@Injectable()
export class ModuleService implements IModuleService {
  private readonly moduleRepository: IModuleRepository;
  constructor(@Inject('REPO.MODULE') _moduleRepository: IModuleRepository) {
    this.moduleRepository = _moduleRepository;
  }
  async getAllModules(): Promise<Module[]> {
    return await this.moduleRepository.getAllModules();
  }
  async findById(id: number): Promise<moduleDetail> {
    return await this.moduleRepository.findById(id);
  }
  async createModule(module: createModule): Promise<moduleDetail> {
    console.log(`Creating module: ${JSON.stringify(module)}`);
    return await this.moduleRepository.createModule(module);
  }
}
