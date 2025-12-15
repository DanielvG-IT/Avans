import { Test, TestingModule } from "@nestjs/testing";
import { TeacherService } from "../../src/application/services/teacher.service";
import { REPOSITORIES } from "../../src/di-tokens";
import { IUserRepository } from "../../src/domain/user/user.repository.interface";
import { IElectiveRepository } from "../../src/domain/elective/elective.repository.interface";
import { User } from "../../src/domain/user/user";
import { Elective } from "../../src/domain/elective/elective";

describe("TeacherService", () => {
  let service: TeacherService;
  let userRepo: jest.Mocked<IUserRepository>;
  let electiveRepo: jest.Mocked<IElectiveRepository>;

  const mockTeacher: User = {
    id: "teacher123",
    email: "teacher@example.com",
    passwordHash: "hashed",
    firstName: "Jane",
    lastName: "Smith",
    role: "teacher",
  };

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
    teachers: ["teacher123"],
  };

  beforeEach(async () => {
    const mockUserRepo = {
      find: jest.fn(),
      findById: jest.fn(),
      findByEmail: jest.fn(),
      create: jest.fn(),
      update: jest.fn(),
      delete: jest.fn(),
    };

    const mockElectiveRepo = {
      find: jest.fn(),
      findById: jest.fn(),
      create: jest.fn(),
      update: jest.fn(),
      delete: jest.fn(),
      isElectiveInUse: jest.fn(),
      findByTeacherId: jest.fn(),
    };

    const module: TestingModule = await Test.createTestingModule({
      providers: [
        TeacherService,
        {
          provide: REPOSITORIES.USER,
          useValue: mockUserRepo,
        },
        {
          provide: REPOSITORIES.ELECTIVE,
          useValue: mockElectiveRepo,
        },
      ],
    }).compile();

    service = module.get<TeacherService>(TeacherService);
    userRepo = module.get(REPOSITORIES.USER);
    electiveRepo = module.get(REPOSITORIES.ELECTIVE);
  });

  it("should be defined", () => {
    expect(service).toBeDefined();
  });

  describe("getElectivesGiven", () => {
    it("should return electives taught by teacher", async () => {
      userRepo.findById.mockResolvedValue(mockTeacher);
      electiveRepo.findByTeacherId.mockResolvedValue([mockElective]);

      const result = await service.getElectivesGiven("teacher123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toHaveLength(1);
        expect(result.data[0]).toEqual(mockElective);
      }
      expect(electiveRepo.findByTeacherId).toHaveBeenCalledWith("teacher123");
    });

    it("should return empty array if teacher has no electives", async () => {
      userRepo.findById.mockResolvedValue(mockTeacher);
      electiveRepo.findByTeacherId.mockResolvedValue([]);

      const result = await service.getElectivesGiven("teacher123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toHaveLength(0);
      }
    });

    it("should return error if teacher not found", async () => {
      userRepo.findById.mockResolvedValue(null);

      const result = await service.getElectivesGiven("nonexistent");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("TEACHER_NOT_FOUND");
        expect(result.error.meta?.teacherId).toBe("nonexistent");
      }
    });

    it("should return error if user is not a teacher", async () => {
      const student: User = {
        id: "student123",
        email: "student@example.com",
        passwordHash: "hashed",
        firstName: "John",
        lastName: "Doe",
        role: "student",
        favorites: [],
      };
      userRepo.findById.mockResolvedValue(student);

      const result = await service.getElectivesGiven("student123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("TEACHER_NOT_FOUND");
      }
    });
  });
});
