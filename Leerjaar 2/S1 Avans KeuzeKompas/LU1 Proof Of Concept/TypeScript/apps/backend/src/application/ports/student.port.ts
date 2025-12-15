import { Elective } from "../../domain/elective/elective";
import { Result } from "../../domain/result";

//* Student Service Interface
export interface IStudentService {
  getFavorites(studentId: string): Promise<Result<Elective[]>>;
  addFavorite(studentId: string, electiveId: string): Promise<Result<boolean>>;
  removeFavorite(studentId: string, electiveId: string): Promise<Result<boolean>>;
  isFavorite(studentId: string, electiveId: string): Promise<Result<boolean>>;
}
