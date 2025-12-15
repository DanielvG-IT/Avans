import { Injectable, Inject, Logger } from "@nestjs/common";
import { IStudentService } from "../ports/student.port";
import { REPOSITORIES, SERVICES } from "../../di-tokens";
import { type IUserRepository } from "../../domain/user/user.repository.interface";
import { type IElectiveService } from "../ports/elective.port";
import { Elective } from "../../domain/elective/elective";
import { Result, ok, err } from "../../domain/result";
import { normalizeIds } from "../utils/id-normalizer.util";

//* Student Service Implementation
@Injectable()
export class StudentService implements IStudentService {
  private readonly logger = new Logger(StudentService.name);

  constructor(
    @Inject(REPOSITORIES.USER)
    private readonly userRepo: IUserRepository,
    @Inject(SERVICES.ELECTIVE)
    private readonly electiveService: IElectiveService,
  ) {}

  public async getFavorites(studentId: string): Promise<Result<Elective[]>> {
    const user = await this.userRepo.findById(studentId);
    if (!user || user.role !== "student") {
      this.logger.warn(`Student with id ${studentId} not found`);
      return err("STUDENT_NOT_FOUND", "Student not found", { studentId });
    }

    // TypeScript narrows user to StudentUser after the role check
    const favoriteList = user.favorites || [];
    const favorites: Elective[] = [];

    for (const electiveId of favoriteList) {
      const electiveResult = await this.electiveService.getElectiveById(electiveId);
      if (electiveResult.ok) {
        favorites.push(electiveResult.data);
      }
    }

    return ok(favorites);
  }

  public async addFavorite(studentId: string, electiveId: string): Promise<Result<boolean>> {
    const user = await this.userRepo.findById(studentId);
    if (!user || user.role !== "student") {
      this.logger.warn(`Student with id ${studentId} not found`);
      return err("STUDENT_NOT_FOUND", "Student not found", { studentId });
    }

    // TypeScript narrows user to StudentUser after the role check
    const favoriteIds = normalizeIds((user.favorites ?? []) as unknown[]);

    if (favoriteIds.includes(electiveId)) {
      this.logger.warn(`Elective ${electiveId} is already a favorite of student ${studentId}`);
      return err("ELECTIVE_ALREADY_FAVORITE", "Elective is already a favorite", {
        studentId,
        electiveId,
      });
    }

    user.favorites.push(electiveId);
    await this.userRepo.update(studentId, user);

    return ok(true);
  }

  public async removeFavorite(studentId: string, electiveId: string): Promise<Result<boolean>> {
    const user = await this.userRepo.findById(studentId);
    if (!user || user.role !== "student") {
      this.logger.warn(`Student with id ${studentId} not found`);
      return err("STUDENT_NOT_FOUND", "Student not found", { studentId });
    }

    // TypeScript narrows user to StudentUser after the role check
    const favoriteIds = normalizeIds((user.favorites ?? []) as unknown[]);

    if (!favoriteIds.includes(electiveId)) {
      this.logger.warn(`Elective ${electiveId} is not a favorite of student ${studentId}`);
      return err("ELECTIVE_NOT_FAVORITE", "Elective is not a favorite", { studentId, electiveId });
    }

    user.favorites = favoriteIds.filter((id) => id !== electiveId);
    await this.userRepo.update(studentId, user);

    return ok(true);
  }

  public async isFavorite(studentId: string, electiveId: string): Promise<Result<boolean>> {
    const user = await this.userRepo.findById(studentId);
    if (!user || user.role !== "student") {
      this.logger.warn(`Student with id ${studentId} not found`);
      return err("STUDENT_NOT_FOUND", "Student not found", { studentId });
    }

    // TypeScript narrows user to StudentUser after the role check
    const favoriteIds = normalizeIds((user.favorites ?? []) as unknown[]);
    return ok(favoriteIds.includes(electiveId));
  }
}
