import { type IElectiveService } from "src/application/ports/elective.port";
import { type Elective } from "src/domain/elective/elective";
import { AuthGuard } from "../guards/auth.guard";
import { RolesGuard } from "../guards/roles.guard";
import { Roles } from "../decorators/roles.decorator";
import { ApiTags } from "@nestjs/swagger";
import { SERVICES } from "src/di-tokens";
import {
  NotFoundException,
  BadRequestException,
  HttpStatus,
  Controller,
  UseGuards,
  HttpCode,
  Inject,
  Logger,
  Param,
  Body,
  Get,
  Post,
  Put,
  Patch,
  Delete,
} from "@nestjs/common";

@ApiTags("electives")
@UseGuards(AuthGuard, RolesGuard)
@Controller("electives")
export class ElectiveController {
  private readonly logger: Logger = new Logger(ElectiveController.name);

  constructor(
    @Inject(SERVICES.ELECTIVE)
    private readonly electiveService: IElectiveService,
  ) {}

  @Post()
  @Roles("admin")
  @HttpCode(HttpStatus.CREATED)
  public async createElective(@Body() data: Elective): Promise<Elective> {
    const result = await this.electiveService.createElective(data);
    if (!result.ok) {
      this.logger.warn(`Failed to create elective: ${result.error.code}`);
      throw new NotFoundException(result.error.message || "Failed to create elective");
    }

    return result.data;
  }

  @Get()
  @HttpCode(HttpStatus.OK)
  public async getAll(): Promise<Elective[]> {
    const result = await this.electiveService.getAllElectives();
    if (!result.ok) {
      this.logger.warn(`Failed to get electives: ${result.error.code}`);
      throw new NotFoundException(result.error.message || "No electives found");
    }

    return result.data;
  }

  @Get(":id")
  @HttpCode(HttpStatus.OK)
  public async getElectiveById(@Param("id") id: string): Promise<Elective> {
    const result = await this.electiveService.getElectiveById(id);
    if (!result.ok) {
      this.logger.warn(`Elective not found: ${id}`);
      throw new NotFoundException(result.error.message || "Elective not found");
    }

    return result.data;
  }

  @Put(":id")
  @Roles("admin")
  @HttpCode(HttpStatus.OK)
  public async updateElective(@Param("id") id: string, @Body() data: Elective): Promise<Elective> {
    const result = await this.electiveService.updateElective(id, data);
    if (!result.ok) {
      this.logger.warn(`Failed to update elective: ${result.error.code}`);
      throw new NotFoundException(result.error.message || "Failed to update elective");
    }

    return result.data;
  }

  @Patch(":id")
  @Roles("admin")
  @HttpCode(HttpStatus.OK)
  public async partialUpdateElective(
    @Param("id") id: string,
    @Body() data: Partial<Elective>,
  ): Promise<Elective> {
    const result = await this.electiveService.updateElective(id, data);
    if (!result.ok) {
      this.logger.warn(`Failed to partially update elective: ${result.error.code}`);
      throw new NotFoundException(result.error.message || "Failed to partially update elective");
    }

    return result.data;
  }

  @Delete(":id")
  @Roles("admin")
  @HttpCode(HttpStatus.NO_CONTENT)
  public async deleteElective(@Param("id") id: string): Promise<void> {
    const result = await this.electiveService.deleteElective(id);
    if (!result.ok) {
      this.logger.warn(`Failed to delete elective: ${result.error.code}`);
      throw new NotFoundException(result.error.message || "Failed to delete elective");
    }
  }

  @Post(":id/teachers/:teacherId")
  @Roles("admin")
  @HttpCode(HttpStatus.OK)
  public async assignTeacherToElective(
    @Param("id") electiveId: string,
    @Param("teacherId") teacherId: string,
  ): Promise<{ message: string }> {
    const result = await this.electiveService.assignTeacherToElective(electiveId, teacherId);
    if (!result.ok) {
      this.logger.warn(`Failed to assign teacher: ${result.error.code}`);
      if (result.error.code === "ALREADY_ASSIGNED") {
        throw new BadRequestException(result.error.message || "Teacher already assigned");
      }
      throw new NotFoundException(result.error.message || "Failed to assign teacher");
    }

    return { message: "Teacher assigned successfully" };
  }

  @Delete(":id/teachers/:teacherId")
  @Roles("admin")
  @HttpCode(HttpStatus.OK)
  public async unassignTeacherFromElective(
    @Param("id") electiveId: string,
    @Param("teacherId") teacherId: string,
  ): Promise<{ message: string }> {
    const result = await this.electiveService.unassignTeacherFromElective(electiveId, teacherId);
    if (!result.ok) {
      this.logger.warn(`Failed to unassign teacher: ${result.error.code}`);
      if (result.error.code === "NOT_ASSIGNED") {
        throw new BadRequestException(result.error.message || "Teacher not assigned");
      }
      throw new NotFoundException(result.error.message || "Failed to unassign teacher");
    }

    return { message: "Teacher unassigned successfully" };
  }
}
