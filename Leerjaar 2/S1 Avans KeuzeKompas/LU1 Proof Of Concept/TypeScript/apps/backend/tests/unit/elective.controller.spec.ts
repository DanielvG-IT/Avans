import { Test, TestingModule } from "@nestjs/testing";
import { ElectiveController } from "../../src/interfaces/controllers/elective.controller";
import { SERVICES } from "../../src/di-tokens";
import { IElectiveService } from "../../src/application/ports/elective.port";
import { Elective } from "../../src/domain/elective/elective";
import { ok, err } from "../../src/domain/result";
import { NotFoundException, BadRequestException } from "@nestjs/common";
import { JwtService } from "@nestjs/jwt";
import { Reflector } from "@nestjs/core";

describe("ElectiveController", () => {
  let controller: ElectiveController;
  let electiveService: jest.Mocked<IElectiveService>;

  const mockElective: Elective = {
    id: "elective123",
    code: "CS101",
    name: "Computer Science",
    description: "Intro to CS",
    provider: "University",
    period: "P1",
    duration: "1 Periode",
    credits: 5,
    language: "English",
    location: "Campus",
    level: "NLQF5",
  };

  beforeEach(async () => {
    const mockElectiveService = {
      getAllElectives: jest.fn(),
      getElectiveById: jest.fn(),
      createElective: jest.fn(),
      updateElective: jest.fn(),
      deleteElective: jest.fn(),
      assignTeacherToElective: jest.fn(),
      unassignTeacherFromElective: jest.fn(),
    };

    const module: TestingModule = await Test.createTestingModule({
      controllers: [ElectiveController],
      providers: [
        {
          provide: SERVICES.ELECTIVE,
          useValue: mockElectiveService,
        },
        {
          provide: JwtService,
          useValue: {
            sign: jest.fn(),
            verify: jest.fn(),
          },
        },
        {
          provide: Reflector,
          useValue: {
            get: jest.fn(),
            getAll: jest.fn(),
            getAllAndOverride: jest.fn(),
          },
        },
      ],
    }).compile();

    controller = module.get<ElectiveController>(ElectiveController);
    electiveService = module.get(SERVICES.ELECTIVE);
  });

  it("should be defined", () => {
    expect(controller).toBeDefined();
  });

  describe("createElective", () => {
    it("should create an elective", async () => {
      electiveService.createElective.mockResolvedValue(ok(mockElective));

      const result = await controller.createElective(mockElective);

      expect(result).toEqual(mockElective);
    });

    it("should throw NotFoundException when creation fails", async () => {
      electiveService.createElective.mockResolvedValue(err("CREATE_FAILED", "Failed to create"));

      await expect(controller.createElective(mockElective)).rejects.toThrow(NotFoundException);
    });
  });

  describe("getAll", () => {
    it("should return all electives", async () => {
      electiveService.getAllElectives.mockResolvedValue(ok([mockElective]));

      const result = await controller.getAll();

      expect(result).toEqual([mockElective]);
    });

    it("should throw NotFoundException when no electives found", async () => {
      electiveService.getAllElectives.mockResolvedValue(
        err("NO_ELECTIVES_FOUND", "No electives found"),
      );

      await expect(controller.getAll()).rejects.toThrow(NotFoundException);
    });
  });

  describe("getElectiveById", () => {
    it("should return an elective by id", async () => {
      electiveService.getElectiveById.mockResolvedValue(ok(mockElective));

      const result = await controller.getElectiveById("elective123");

      expect(result).toEqual(mockElective);
    });

    it("should throw NotFoundException when elective not found", async () => {
      electiveService.getElectiveById.mockResolvedValue(
        err("ELECTIVE_NOT_FOUND", "Elective not found"),
      );

      await expect(controller.getElectiveById("nonexistent")).rejects.toThrow(NotFoundException);
    });
  });

  describe("updateElective", () => {
    it("should update an elective", async () => {
      const updatedElective = { ...mockElective, name: "Updated Name" };
      electiveService.updateElective.mockResolvedValue(ok(updatedElective));

      const result = await controller.updateElective("elective123", updatedElective);

      expect(result).toEqual(updatedElective);
    });

    it("should throw NotFoundException when update fails", async () => {
      electiveService.updateElective.mockResolvedValue(err("UPDATE_FAILED", "Failed to update"));

      await expect(controller.updateElective("elective123", mockElective)).rejects.toThrow(
        NotFoundException,
      );
    });
  });

  describe("partialUpdateElective", () => {
    it("should partially update an elective", async () => {
      const updatedElective = { ...mockElective, name: "Partially Updated" };
      electiveService.updateElective.mockResolvedValue(ok(updatedElective));

      const result = await controller.partialUpdateElective("elective123", {
        name: "Partially Updated",
      });

      expect(result).toEqual(updatedElective);
    });

    it("should throw NotFoundException when partial update fails", async () => {
      electiveService.updateElective.mockResolvedValue(err("UPDATE_FAILED", "Failed to update"));

      await expect(
        controller.partialUpdateElective("elective123", { name: "New Name" }),
      ).rejects.toThrow(NotFoundException);
    });
  });

  describe("deleteElective", () => {
    it("should delete an elective", async () => {
      electiveService.deleteElective.mockResolvedValue(ok(true));

      await controller.deleteElective("elective123");

      expect(electiveService.deleteElective).toHaveBeenCalledWith("elective123");
    });

    it("should throw NotFoundException when deletion fails", async () => {
      electiveService.deleteElective.mockResolvedValue(err("DELETE_FAILED", "Failed to delete"));

      await expect(controller.deleteElective("elective123")).rejects.toThrow(NotFoundException);
    });
  });

  describe("assignTeacherToElective", () => {
    it("should assign a teacher to an elective", async () => {
      electiveService.assignTeacherToElective.mockResolvedValue(ok(true));

      const result = await controller.assignTeacherToElective("elective123", "teacher123");

      expect(result).toEqual({ message: "Teacher assigned successfully" });
    });

    it("should throw BadRequestException when teacher already assigned", async () => {
      electiveService.assignTeacherToElective.mockResolvedValue(
        err("ALREADY_ASSIGNED", "Teacher already assigned"),
      );

      await expect(controller.assignTeacherToElective("elective123", "teacher123")).rejects.toThrow(
        BadRequestException,
      );
    });

    it("should throw NotFoundException when assignment fails", async () => {
      electiveService.assignTeacherToElective.mockResolvedValue(
        err("ASSIGNMENT_FAILED", "Failed to assign"),
      );

      await expect(controller.assignTeacherToElective("elective123", "teacher123")).rejects.toThrow(
        NotFoundException,
      );
    });
  });

  describe("unassignTeacherFromElective", () => {
    it("should unassign a teacher from an elective", async () => {
      electiveService.unassignTeacherFromElective.mockResolvedValue(ok(true));

      const result = await controller.unassignTeacherFromElective("elective123", "teacher123");

      expect(result).toEqual({ message: "Teacher unassigned successfully" });
    });

    it("should throw BadRequestException when teacher not assigned", async () => {
      electiveService.unassignTeacherFromElective.mockResolvedValue(
        err("NOT_ASSIGNED", "Teacher not assigned"),
      );

      await expect(
        controller.unassignTeacherFromElective("elective123", "teacher123"),
      ).rejects.toThrow(BadRequestException);
    });

    it("should throw NotFoundException when unassignment fails", async () => {
      electiveService.unassignTeacherFromElective.mockResolvedValue(
        err("UNASSIGNMENT_FAILED", "Failed to unassign"),
      );

      await expect(
        controller.unassignTeacherFromElective("elective123", "teacher123"),
      ).rejects.toThrow(NotFoundException);
    });
  });
});
