export interface IUserRecommendedService {
  setRecommendedModules(userId: string, moduleIds: number[]): Promise<void>;
  getRecommendedModuleIds(userId: string): Promise<number[]>;
}
