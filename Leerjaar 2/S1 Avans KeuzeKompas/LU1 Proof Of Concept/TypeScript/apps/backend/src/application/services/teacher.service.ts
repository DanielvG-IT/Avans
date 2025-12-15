import { type IElectiveRepository } from "../../domain/elective/elective.repository.interface";
import { type IUserRepository } from "../../domain/user/user.repository.interface";
import { Injectable, Inject, Logger } from "@nestjs/common";
import { Elective } from "../../domain/elective/elective";
import { ITeacherService } from "../ports/teacher.port";
import { Result, ok, err } from "../../domain/result";
import { REPOSITORIES } from "../../di-tokens";

//* Teacher Service Implementation
@Injectable()
export class TeacherService implements ITeacherService {
  private readonly logger = new Logger(TeacherService.name);

  constructor(
    @Inject(REPOSITORIES.USER)
    private readonly userRepo: IUserRepository,
    @Inject(REPOSITORIES.ELECTIVE)
    private readonly electiveRepo: IElectiveRepository,
  ) {}

  public async getElectivesGiven(teacherId: string): Promise<Result<Elective[]>> {
    const user = await this.userRepo.findById(teacherId);
    if (!user || user.role !== "teacher") {
      this.logger.warn(`Teacher with id ${teacherId} not found`);
      return err("TEACHER_NOT_FOUND", "Teacher not found", { teacherId });
    }

    // Query electives by teacherId in the teachers array
    const electives = await this.electiveRepo.findByTeacherId(teacherId);
    return ok(electives);
  }
}
