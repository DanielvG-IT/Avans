import { Injectable, Inject } from '@nestjs/common';
import { IUserRecommendedService } from '@/application/ports/userrecommended.port';
import { IUserRecommendedRepository } from '@/domain/userrecommended/userrecommended-repository.interface';

@Injectable()
export class UserRecommendedService implements IUserRecommendedService {
  constructor(
    @Inject('REPO.USER_RECOMMENDED')
    private readonly repo: IUserRecommendedRepository,
  ) {}

  setRecommendedModules(userId: string, moduleIds: number[]): Promise<void> {
    return this.repo.setRecommendedModules(userId, moduleIds);
  }

  getRecommendedModuleIds(userId: string): Promise<number[]> {
    return this.repo.getRecommendedModuleIds(userId);
  }
}
