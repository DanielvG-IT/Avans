import { Injectable, Inject } from '@nestjs/common';
import { IModuleService } from '@/application/ports/module.port';
import { type IModuleRepository } from '@/domain/modules/module-repository.interface';
import { Module } from '@/domain/modules/module.model';

@Injectable()
export class ModuleService implements IModuleService {
  private readonly moduleRepository: IModuleRepository;
  constructor(@Inject('REPO.MODULE') _moduleRepository: IModuleRepository) {
    this.moduleRepository = _moduleRepository;
  }
  getAllModules(): Promise<Module[]> {
    return this.moduleRepository.getAllModules();
  }
}
