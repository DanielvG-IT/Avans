export interface UserRecommended {
  id: number;
  userId: string;
  moduleId: number;
  recommendationReason?: string | null;
  createdAt: Date;
}
