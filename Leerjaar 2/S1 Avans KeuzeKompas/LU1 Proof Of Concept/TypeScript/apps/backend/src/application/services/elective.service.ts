import { REPOSITORIES } from "src/di-tokens";
import { Elective } from "src/domain/elective/elective";
import { IElectiveService } from "../ports/elective.port";
import { Injectable, Inject, Logger } from "@nestjs/common";
import { type IElectiveRepository } from "src/domain/elective/elective.repository.interface";
import { type IUserRepository } from "src/domain/user/user.repository.interface";
import { Result, ok, err } from "src/domain/result";

//* Elective Service Implementation
@Injectable()
export class ElectiveService implements IElectiveService {
  private readonly logger = new Logger(ElectiveService.name);

  constructor(
    @Inject(REPOSITORIES.ELECTIVE)
    private readonly electiveRepo: IElectiveRepository,
    @Inject(REPOSITORIES.USER)
    private readonly userRepo: IUserRepository,
  ) {}

  public async getAllElectives(): Promise<Result<Elective[]>> {
    const electives = await this.electiveRepo.find();
    if (!electives || electives.length === 0) {
      this.logger.warn("No electives found");
      return err("NO_ELECTIVES_FOUND", "No electives found");
    }

    return ok(electives);
  }

  public async getElectiveById(id: string): Promise<Result<Elective>> {
    const elective = await this.electiveRepo.findById(id);
    if (!elective) {
      this.logger.warn(`Elective with id ${id} not found`);
      return err("ELECTIVE_NOT_FOUND", "Elective not found", { electiveId: id });
    }

    return ok(elective);
  }

  public async createElective(data: Elective): Promise<Result<Elective>> {
    if (!data) {
      this.logger.warn("No elective data provided for creation");
      return err("NO_ELECTIVE_DATA", "No elective data provided");
    }

    const created = await this.electiveRepo.create(data);
    if (!created) {
      this.logger.error("Failed to create elective");
      return err("ELECTIVE_CREATE_FAILED", "Failed to create elective");
    }

    return ok(created);
  }

  public async updateElective(
    id: string,
    data: Elective | Partial<Elective>,
  ): Promise<Result<Elective>> {
    const elective = await this.electiveRepo.findById(id);
    if (!elective) {
      this.logger.warn(`Elective with id ${id} not found`);
      return err("ELECTIVE_NOT_FOUND", "Elective not found", { electiveId: id });
    }

    const updated = await this.electiveRepo.update(id, data);
    if (!updated) {
      this.logger.error(`Failed to update elective with id ${id}`);
      return err("ELECTIVE_UPDATE_FAILED", "Failed to update elective", { electiveId: id });
    }

    return ok(updated);
  }

  public async deleteElective(id: string): Promise<Result<boolean>> {
    const elective = await this.electiveRepo.findById(id);
    if (!elective) {
      this.logger.warn(`Elective with id ${id} not found`);
      return err("ELECTIVE_NOT_FOUND", "Elective not found", { electiveId: id });
    }

    const deletedInUse = await this.electiveRepo.isElectiveInUse(id);
    if (deletedInUse) {
      this.logger.warn(`Cannot delete elective with id ${id} because it is in use`);
      return err("ELECTIVE_IN_USE", "Cannot delete elective because it is in use", {
        electiveId: id,
      });
    }

    const deleted = await this.electiveRepo.delete(id);
    if (!deleted) {
      this.logger.error(`Failed to delete elective with id ${id}`);
      return err("ELECTIVE_DELETE_FAILED", "Failed to delete elective", { electiveId: id });
    }
    this.logger.log(`Elective with id ${id} deleted`);
    return ok(true);
  }

  public async assignTeacherToElective(
    electiveId: string,
    teacherId: string,
  ): Promise<Result<boolean>> {
    // Verify elective exists
    const elective = await this.electiveRepo.findById(electiveId);
    if (!elective) {
      this.logger.warn(`Elective with id ${electiveId} not found`);
      return err("ELECTIVE_NOT_FOUND", "Elective not found", { electiveId });
    }

    // Verify teacher exists
    const user = await this.userRepo.findById(teacherId);
    if (!user || user.role !== "teacher") {
      this.logger.warn(`Teacher with id ${teacherId} not found`);
      return err("TEACHER_NOT_FOUND", "Teacher not found", { teacherId });
    }

    // Check if already assigned
    const teacherList = elective.teachers || [];
    if (teacherList.includes(teacherId)) {
      this.logger.warn(`Teacher ${teacherId} already assigned to elective ${electiveId}`);
      return err("ALREADY_ASSIGNED", "Teacher already assigned to this elective");
    }

    // Add teacher to elective's teachers array
    const updatedElective = await this.electiveRepo.update(electiveId, {
      ...elective,
      teachers: [...teacherList, teacherId],
    });

    if (!updatedElective) {
      this.logger.error(`Failed to assign teacher ${teacherId} to elective ${electiveId}`);
      return err("ASSIGNMENT_FAILED", "Failed to assign teacher to elective");
    }

    this.logger.log(`Teacher ${teacherId} assigned to elective ${electiveId}`);
    return ok(true);
  }

  public async unassignTeacherFromElective(
    electiveId: string,
    teacherId: string,
  ): Promise<Result<boolean>> {
    // Verify elective exists
    const elective = await this.electiveRepo.findById(electiveId);
    if (!elective) {
      this.logger.warn(`Elective with id ${electiveId} not found`);
      return err("ELECTIVE_NOT_FOUND", "Elective not found", { electiveId });
    }

    // Check if assigned
    const teacherList = elective.teachers || [];
    if (!teacherList.includes(teacherId)) {
      this.logger.warn(`Teacher ${teacherId} not assigned to elective ${electiveId}`);
      return err("NOT_ASSIGNED", "Teacher not assigned to this elective");
    }

    // Remove teacher from elective's teachers array
    const updatedElective = await this.electiveRepo.update(electiveId, {
      ...elective,
      teachers: teacherList.filter((id) => id !== teacherId),
    });

    if (!updatedElective) {
      this.logger.error(`Failed to unassign teacher ${teacherId} from elective ${electiveId}`);
      return err("UNASSIGNMENT_FAILED", "Failed to unassign teacher from elective");
    }

    this.logger.log(`Teacher ${teacherId} unassigned from elective ${electiveId}`);
    return ok(true);
  }
}
