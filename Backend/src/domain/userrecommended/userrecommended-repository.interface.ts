export interface IUserRecommendedRepository {
  /**
   * Overwrite the set of recommended modules for a user.
   *
   * - Sets recommended=true for provided moduleIds
   * - Sets recommended=false for other previously-recommended modules
   * - Cleans up rows where favorited=false AND recommended=false
   */
  setRecommendedModules(userId: string, moduleIds: number[]): Promise<void>;

  /** Return the current recommended module IDs for the user. */
  getRecommendedModuleIds(userId: string): Promise<number[]>;
}
