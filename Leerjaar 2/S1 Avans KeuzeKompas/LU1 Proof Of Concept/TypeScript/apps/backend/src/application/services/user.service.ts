import { User } from "src/domain/user/user";
import { IUserService } from "../ports/user.port";
import { REPOSITORIES } from "../../di-tokens";
import { Injectable, Inject, Logger } from "@nestjs/common";
import { type IUserRepository } from "../../domain/user/user.repository.interface";
import { type IElectiveRepository } from "../../domain/elective/elective.repository.interface";
import { Result, ok, err } from "../../domain/result";
import { PasswordUtil } from "../utils/password.util";

//* User Service Implementation
@Injectable()
export class UserService implements IUserService {
  private readonly logger = new Logger(UserService.name);

  constructor(
    @Inject(REPOSITORIES.USER)
    private readonly userRepo: IUserRepository,
    @Inject(REPOSITORIES.ELECTIVE)
    private readonly electiveRepo: IElectiveRepository,
  ) {}

  public async getUserById(id: string): Promise<Result<User>> {
    const user = await this.userRepo.findById(id);
    if (!user) {
      this.logger.warn(`User with id ${id} not found`);
      return err("USER_NOT_FOUND", "User not found", { userId: id });
    }
    return ok(user);
  }

  public async getUserByEmail(email: string): Promise<Result<User>> {
    const user = await this.userRepo.findByEmail(email);
    if (!user) {
      this.logger.warn(`User with email ${email} not found`);
      return err("USER_NOT_FOUND", "User not found", { email });
    }

    return ok(user);
  }

  public async getAllUsers(): Promise<Result<User[]>> {
    const users = await this.userRepo.find();
    if (!users) {
      this.logger.error("Failed to fetch all users");
      return err("USERS_FETCH_FAILED", "Failed to fetch users");
    }
    return ok(users);
  }

  public async createUser(data: User): Promise<Result<User>> {
    if (!data) {
      this.logger.warn("No user data provided for creation");
      return err("NO_USER_DATA", "No user data provided");
    }

    if (await this.userRepo.findByEmail(data.email)) {
      this.logger.warn(`User with email ${data.email} already exists`);
      return err("USER_ALREADY_EXISTS", "User with this email already exists", {
        email: data.email,
      });
    }

    // Hash the password before saving using centralized PasswordUtil
    data.passwordHash = await PasswordUtil.hash(data.passwordHash);

    const created = await this.userRepo.create(data);
    if (!created) {
      this.logger.error("Failed to create user");
      return err("USER_CREATE_FAILED", "Failed to create user");
    }

    return ok(created);
  }

  public async updateUser(id: string, data: User | Partial<User>): Promise<Result<User>> {
    const user = await this.userRepo.findById(id);
    if (!user) {
      this.logger.warn(`User with id ${id} not found`);
      return err("USER_NOT_FOUND", "User not found", { userId: id });
    }

    const updated = await this.userRepo.update(id, data);
    if (!updated) {
      this.logger.error(`Failed to update user with id ${id}`);
      return err("USER_UPDATE_FAILED", "Failed to update user", { userId: id });
    }

    return ok(updated);
  }

  public async deleteUser(id: string): Promise<Result<boolean>> {
    const user = await this.userRepo.findById(id);
    if (!user) {
      this.logger.warn(`User with id ${id} not found`);
      return err("USER_NOT_FOUND", "User not found", { userId: id });
    }

    // Check if user is a student or teacher with active data
    if (user.role === "student" && "favorites" in user && user.favorites.length > 0) {
      this.logger.warn(`Cannot delete student with id ${id} who has favorites`);
      return err("USER_DELETE_FAILED", "Cannot delete student with active favorites", {
        userId: id,
      });
    }

    if (user.role === "teacher") {
      // Check if teacher is assigned to any electives
      const electives = await this.electiveRepo.findByTeacherId(id);
      if (electives.length > 0) {
        this.logger.warn(`Cannot delete teacher with id ${id} who teaches electives`);
        return err("USER_DELETE_FAILED", "Cannot delete teacher with active electives", {
          userId: id,
        });
      }
    }

    const deleted = await this.userRepo.delete(id);
    if (!deleted) {
      this.logger.error(`Failed to delete user with id ${id}`);
      return err("USER_DELETE_FAILED", "Failed to delete user", { userId: id });
    }

    return ok(true);
  }
}
