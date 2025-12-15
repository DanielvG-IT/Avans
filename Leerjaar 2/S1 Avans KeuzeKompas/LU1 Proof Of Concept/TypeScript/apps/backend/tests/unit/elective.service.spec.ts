import { Test, TestingModule } from "@nestjs/testing";
import { ElectiveService } from "../../src/application/services/elective.service";
import { REPOSITORIES } from "src/di-tokens";
import { IElectiveRepository } from "src/domain/elective/elective.repository.interface";
import { IUserRepository } from "src/domain/user/user.repository.interface";
import { Elective } from "src/domain/elective/elective";
import { User } from "src/domain/user/user";

describe("ElectiveService", () => {
  let service: ElectiveService;
  let electiveRepo: jest.Mocked<IElectiveRepository>;
  let userRepo: jest.Mocked<IUserRepository>;

  const mockElective: Elective = {
    id: "elective123",
    code: "CS101",
    name: "Computer Science Basics",
    description: "Introduction to CS",
    provider: "University",
    period: "P1",
    duration: "1 Periode",
    credits: 5,
    language: "English",
    location: "Campus A",
    level: "NLQF5",
    tags: ["programming", "basics"],
    teachers: [],
  };

  const mockTeacher: User = {
    id: "teacher123",
    email: "teacher@example.com",
    passwordHash: "hashed",
    firstName: "Jane",
    lastName: "Smith",
    role: "teacher",
  };

  beforeEach(async () => {
    const mockElectiveRepo = {
      find: jest.fn(),
      findById: jest.fn(),
      create: jest.fn(),
      update: jest.fn(),
      delete: jest.fn(),
      isElectiveInUse: jest.fn(),
      findByTeacherId: jest.fn(),
    };

    const mockUserRepo = {
      find: jest.fn(),
      findById: jest.fn(),
      findByEmail: jest.fn(),
      create: jest.fn(),
      update: jest.fn(),
      delete: jest.fn(),
    };

    const module: TestingModule = await Test.createTestingModule({
      providers: [
        ElectiveService,
        {
          provide: REPOSITORIES.ELECTIVE,
          useValue: mockElectiveRepo,
        },
        {
          provide: REPOSITORIES.USER,
          useValue: mockUserRepo,
        },
      ],
    }).compile();

    service = module.get<ElectiveService>(ElectiveService);
    electiveRepo = module.get(REPOSITORIES.ELECTIVE);
    userRepo = module.get(REPOSITORIES.USER);
  });

  it("should be defined", () => {
    expect(service).toBeDefined();
  });

  describe("getAllElectives", () => {
    it("should return all electives", async () => {
      electiveRepo.find.mockResolvedValue([mockElective]);

      const result = await service.getAllElectives();

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toEqual([mockElective]);
      }
    });

    it("should return error if no electives found", async () => {
      electiveRepo.find.mockResolvedValue([]);

      const result = await service.getAllElectives();

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("NO_ELECTIVES_FOUND");
      }
    });

    it("should return error if find returns null", async () => {
      electiveRepo.find.mockResolvedValue(null as any);

      const result = await service.getAllElectives();

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("NO_ELECTIVES_FOUND");
      }
    });
  });

  describe("getElectiveById", () => {
    it("should return elective by id", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);

      const result = await service.getElectiveById("elective123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toEqual(mockElective);
      }
    });

    it("should return error if elective not found", async () => {
      electiveRepo.findById.mockResolvedValue(null);

      const result = await service.getElectiveById("nonexistent");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_NOT_FOUND");
        expect(result.error.meta?.electiveId).toBe("nonexistent");
      }
    });
  });

  describe("createElective", () => {
    it("should create a new elective", async () => {
      electiveRepo.create.mockResolvedValue(mockElective);

      const result = await service.createElective(mockElective);

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toEqual(mockElective);
      }
      expect(electiveRepo.create).toHaveBeenCalledWith(mockElective);
    });

    it("should return error if no data provided", async () => {
      const result = await service.createElective(null as any);

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("NO_ELECTIVE_DATA");
      }
    });

    it("should return error if creation fails", async () => {
      electiveRepo.create.mockResolvedValue(null);

      const result = await service.createElective(mockElective);

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_CREATE_FAILED");
      }
    });
  });

  describe("updateElective", () => {
    it("should update an elective", async () => {
      const updatedElective = { ...mockElective, name: "Updated Name" };
      electiveRepo.findById.mockResolvedValue(mockElective);
      electiveRepo.update.mockResolvedValue(updatedElective);

      const result = await service.updateElective("elective123", updatedElective);

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data.name).toBe("Updated Name");
      }
    });

    it("should return error if elective not found", async () => {
      electiveRepo.findById.mockResolvedValue(null);

      const result = await service.updateElective("nonexistent", mockElective);

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_NOT_FOUND");
      }
    });

    it("should return error if update fails", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);
      electiveRepo.update.mockResolvedValue(null);

      const result = await service.updateElective("elective123", mockElective);

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_UPDATE_FAILED");
      }
    });
  });

  describe("deleteElective", () => {
    it("should delete an elective", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);
      electiveRepo.isElectiveInUse.mockResolvedValue(false);
      electiveRepo.delete.mockResolvedValue(true);

      const result = await service.deleteElective("elective123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe(true);
      }
    });

    it("should return error if elective not found", async () => {
      electiveRepo.findById.mockResolvedValue(null);

      const result = await service.deleteElective("nonexistent");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_NOT_FOUND");
      }
    });

    it("should return error if elective is in use", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);
      electiveRepo.isElectiveInUse.mockResolvedValue(true);

      const result = await service.deleteElective("elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_IN_USE");
      }
    });

    it("should return error if deletion fails", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);
      electiveRepo.isElectiveInUse.mockResolvedValue(false);
      electiveRepo.delete.mockResolvedValue(false);

      const result = await service.deleteElective("elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_DELETE_FAILED");
      }
    });
  });

  describe("assignTeacherToElective", () => {
    it("should assign a teacher to an elective", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);
      userRepo.findById.mockResolvedValue(mockTeacher);
      electiveRepo.update.mockResolvedValue({ ...mockElective, teachers: ["teacher123"] });

      const result = await service.assignTeacherToElective("elective123", "teacher123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe(true);
      }
    });

    it("should return error if elective not found", async () => {
      electiveRepo.findById.mockResolvedValue(null);

      const result = await service.assignTeacherToElective("nonexistent", "teacher123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_NOT_FOUND");
      }
    });

    it("should return error if teacher not found", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);
      userRepo.findById.mockResolvedValue(null);

      const result = await service.assignTeacherToElective("elective123", "nonexistent");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("TEACHER_NOT_FOUND");
      }
    });

    it("should return error if user is not a teacher", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);
      const studentUser: User = { ...mockTeacher, role: "student", favorites: [] };
      userRepo.findById.mockResolvedValue(studentUser);

      const result = await service.assignTeacherToElective("elective123", "student123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("TEACHER_NOT_FOUND");
      }
    });

    it("should return error if teacher already assigned", async () => {
      const electiveWithTeacher = { ...mockElective, teachers: ["teacher123"] };
      electiveRepo.findById.mockResolvedValue(electiveWithTeacher);
      userRepo.findById.mockResolvedValue(mockTeacher);

      const result = await service.assignTeacherToElective("elective123", "teacher123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ALREADY_ASSIGNED");
      }
    });

    it("should return error if assignment fails", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);
      userRepo.findById.mockResolvedValue(mockTeacher);
      electiveRepo.update.mockResolvedValue(null);

      const result = await service.assignTeacherToElective("elective123", "teacher123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ASSIGNMENT_FAILED");
      }
    });
  });

  describe("unassignTeacherFromElective", () => {
    it("should unassign a teacher from an elective", async () => {
      const electiveWithTeacher = { ...mockElective, teachers: ["teacher123"] };
      electiveRepo.findById.mockResolvedValue(electiveWithTeacher);
      electiveRepo.update.mockResolvedValue({ ...mockElective, teachers: [] });

      const result = await service.unassignTeacherFromElective("elective123", "teacher123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe(true);
      }
    });

    it("should return error if elective not found", async () => {
      electiveRepo.findById.mockResolvedValue(null);

      const result = await service.unassignTeacherFromElective("nonexistent", "teacher123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_NOT_FOUND");
      }
    });

    it("should return error if teacher not assigned", async () => {
      electiveRepo.findById.mockResolvedValue(mockElective);

      const result = await service.unassignTeacherFromElective("elective123", "teacher123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("NOT_ASSIGNED");
      }
    });

    it("should return error if unassignment fails", async () => {
      const electiveWithTeacher = { ...mockElective, teachers: ["teacher123"] };
      electiveRepo.findById.mockResolvedValue(electiveWithTeacher);
      electiveRepo.update.mockResolvedValue(null);

      const result = await service.unassignTeacherFromElective("elective123", "teacher123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("UNASSIGNMENT_FAILED");
      }
    });
  });
});
