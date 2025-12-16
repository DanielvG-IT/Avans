export interface IModuleRepository {
  getAllModules(): Promise<any>;
  // findById(id: string): Promise<any>;
}
