import { type RequestWithCookies, AuthGuard } from "../guards/auth.guard";
import { type IStudentService } from "src/application/ports/student.port";
import { type ITeacherService } from "src/application/ports/teacher.port";
import { userDTO, createUserDTO, updateUserDTO } from "../dtos/user.dto";
import { type IUserService } from "src/application/ports/user.port";
import { type Elective } from "src/domain/elective/elective";
import { Roles } from "../decorators/roles.decorator";
import { RolesGuard } from "../guards/roles.guard";
import { User } from "src/domain/user/user";
import { ApiTags } from "@nestjs/swagger";
import { SERVICES } from "src/di-tokens";
import {
  UnauthorizedException,
  BadRequestException,
  NotFoundException,
  Controller,
  HttpStatus,
  UseGuards,
  HttpCode,
  Delete,
  Inject,
  Logger,
  Param,
  Post,
  Body,
  Patch,
  Get,
  Req,
} from "@nestjs/common";

@ApiTags("users")
@UseGuards(AuthGuard, RolesGuard)
@Controller("users")
export class UserController {
  private readonly logger = new Logger(UserController.name);

  constructor(
    @Inject(SERVICES.USER)
    private readonly userService: IUserService,
    @Inject(SERVICES.STUDENT)
    private readonly studentService: IStudentService,
    @Inject(SERVICES.TEACHER)
    private readonly teacherService: ITeacherService,
  ) {}

  /**
   * Helper to extract authenticated user claims
   * AuthGuard ensures authClaims exists, but TypeScript doesn't know that
   */
  private getAuthClaims(req: RequestWithCookies) {
    if (!req.authClaims) {
      throw new UnauthorizedException("Authentication required");
    }
    return req.authClaims;
  }

  /**
   * Convert domain User entity to DTO (removes sensitive fields like passwordHash)
   */
  private toDTO(user: User): userDTO {
    // eslint-disable-next-line @typescript-eslint/no-unused-vars, @typescript-eslint/no-unsafe-assignment
    const { passwordHash, ...safeUser } = user as any;
    return safeUser as userDTO;
  }

  /**
   * Get the authenticated user's profile
   * Available to all authenticated users
   */
  @Get("me")
  @HttpCode(HttpStatus.OK)
  public async getProfile(@Req() req: RequestWithCookies): Promise<userDTO> {
    const { sub: userId } = this.getAuthClaims(req);

    const userResult = await this.userService.getUserById(userId.toString());
    if (!userResult.ok) {
      this.logger.warn(`User not found: ${userId}`);
      throw new UnauthorizedException("User not found");
    }

    return this.toDTO(userResult.data);
  }

  /**
   * Get all users (admin only)
   * Available to users with role: admin
   */
  @Get()
  @Roles("admin")
  @HttpCode(HttpStatus.OK)
  public async getAllUsers(): Promise<userDTO[]> {
    const result = await this.userService.getAllUsers();
    if (!result.ok) {
      this.logger.warn(`Failed to get all users: ${result.error.code}`);
      throw new NotFoundException(result.error.message || "Failed to get users");
    }

    return result.data.map((user) => this.toDTO(user));
  }

  /**
   * Create a new user (admin only)
   * Available to users with role: admin
   */
  @Post()
  @Roles("admin")
  @HttpCode(HttpStatus.CREATED)
  public async createUser(@Body() createUserDto: createUserDTO): Promise<userDTO> {
    if (
      !createUserDto.firstName ||
      !createUserDto.lastName ||
      !createUserDto.email ||
      !createUserDto.password ||
      !createUserDto.role
    ) {
      throw new BadRequestException("Missing required fields");
    }

    // Create user object based on role (password will be hashed in service layer)
    let userData: User;
    if (createUserDto.role === "student") {
      userData = {
        firstName: createUserDto.firstName,
        lastName: createUserDto.lastName,
        email: createUserDto.email,
        role: "student",
        passwordHash: createUserDto.password, // Pass plain password, service will hash it
        favorites: [],
      };
    } else if (createUserDto.role === "teacher") {
      userData = {
        firstName: createUserDto.firstName,
        lastName: createUserDto.lastName,
        email: createUserDto.email,
        role: "teacher",
        passwordHash: createUserDto.password, // Pass plain password, service will hash it
      };
    } else {
      userData = {
        firstName: createUserDto.firstName,
        lastName: createUserDto.lastName,
        email: createUserDto.email,
        role: "admin",
        passwordHash: createUserDto.password, // Pass plain password, service will hash it
      };
    }

    const result = await this.userService.createUser(userData);
    if (!result.ok) {
      this.logger.warn(`Failed to create user: ${result.error.code}`);
      throw new BadRequestException(result.error.message || "Failed to create user");
    }

    return this.toDTO(result.data);
  }

  /**
   * Update a user (admin only)
   * Available to users with role: admin
   */
  @Patch(":userId")
  @Roles("admin")
  @HttpCode(HttpStatus.OK)
  public async updateUser(
    @Param("userId") userId: string,
    @Body() updateUserDto: updateUserDTO,
  ): Promise<userDTO> {
    if (!userId) {
      throw new BadRequestException("User ID is required");
    }

    const result = await this.userService.updateUser(userId, updateUserDto);
    if (!result.ok) {
      this.logger.warn(`Failed to update user: ${result.error.code}`);
      throw new BadRequestException(result.error.message || "Failed to update user");
    }

    return this.toDTO(result.data);
  }

  /**
   * Delete a user (admin only)
   * Available to users with role: admin
   */
  @Delete(":userId")
  @Roles("admin")
  @HttpCode(HttpStatus.NO_CONTENT)
  public async deleteUser(@Param("userId") userId: string): Promise<void> {
    if (!userId) {
      throw new BadRequestException("User ID is required");
    }

    const result = await this.userService.deleteUser(userId);
    if (!result.ok) {
      this.logger.warn(`Failed to delete user: ${result.error.code}`);
      throw new BadRequestException(result.error.message || "Failed to delete user");
    }
  }

  /**
   * Get student's favorite electives
   * Only available to users with role: student
   */
  @Get("me/favorites")
  @Roles("student")
  @HttpCode(HttpStatus.OK)
  public async getFavorites(@Req() req: RequestWithCookies): Promise<Elective[]> {
    const { sub: userId } = this.getAuthClaims(req);

    const result = await this.studentService.getFavorites(userId.toString());
    if (!result.ok) {
      this.logger.warn(`Failed to get favorites: ${result.error.code}`);
      throw new NotFoundException(result.error.message || "Failed to get favorites");
    }

    return result.data;
  }

  /**
   * Check if an elective is in student's favorites
   * Only available to users with role: student
   */
  @Get("me/favorites/:electiveId")
  @Roles("student")
  @HttpCode(HttpStatus.OK)
  public async checkIfFavorite(
    @Param("electiveId") electiveId: string,
    @Req() req: RequestWithCookies,
  ): Promise<{ isFavorite: boolean }> {
    const { sub: userId } = this.getAuthClaims(req);

    if (!electiveId) {
      throw new BadRequestException("Elective ID is required");
    }

    const result = await this.studentService.isFavorite(userId.toString(), electiveId);
    if (!result.ok) {
      this.logger.warn(`Failed to check favorite: ${result.error.code}`);
      throw new NotFoundException(result.error.message);
    }

    return { isFavorite: result.data };
  }

  /**
   * Add an elective to student's favorites
   * Only available to users with role: student
   */
  @Post("me/favorites/:electiveId")
  @Roles("student")
  @HttpCode(HttpStatus.CREATED)
  public async addFavorite(
    @Param("electiveId") electiveId: string,
    @Req() req: RequestWithCookies,
  ): Promise<void> {
    const { sub: userId } = this.getAuthClaims(req);

    if (!electiveId) {
      throw new BadRequestException("Elective ID is required");
    }

    const result = await this.studentService.addFavorite(userId.toString(), electiveId);
    if (!result.ok) {
      this.logger.warn(`Failed to add favorite: ${result.error.code}`);
      if (result.error.code === "ELECTIVE_ALREADY_FAVORITE") {
        throw new BadRequestException(result.error.message || "Elective is already a favorite");
      }
      throw new NotFoundException(result.error.message || "Failed to add favorite");
    }
  }

  /**
   * Remove an elective from student's favorites
   * Only available to users with role: student
   */
  @Delete("me/favorites/:electiveId")
  @Roles("student")
  @HttpCode(HttpStatus.NO_CONTENT)
  public async removeFavorite(
    @Param("electiveId") electiveId: string,
    @Req() req: RequestWithCookies,
  ): Promise<void> {
    const { sub: userId } = this.getAuthClaims(req);

    if (!electiveId) {
      throw new BadRequestException("Elective ID is required");
    }

    const result = await this.studentService.removeFavorite(userId.toString(), electiveId);
    if (!result.ok) {
      this.logger.warn(`Failed to remove favorite: ${result.error.code}`);
      throw new NotFoundException(result.error.message || "Failed to remove favorite");
    }
  }

  /**
   * Get electives taught by the teacher
   * Only available to users with role: teacher
   */
  @Get("me/electives")
  @Roles("teacher")
  @HttpCode(HttpStatus.OK)
  public async getElectives(@Req() req: RequestWithCookies): Promise<Elective[]> {
    const { sub: userId } = this.getAuthClaims(req);

    const result = await this.teacherService.getElectivesGiven(userId.toString());
    if (!result.ok) {
      this.logger.warn(`Failed to get electives: ${result.error.code}`);
      throw new NotFoundException(result.error.message || "Failed to get electives");
    }

    return result.data;
  }
}
