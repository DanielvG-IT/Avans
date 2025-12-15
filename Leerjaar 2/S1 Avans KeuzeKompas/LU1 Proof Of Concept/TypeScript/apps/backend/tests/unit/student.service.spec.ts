import { Test, TestingModule } from "@nestjs/testing";
import { StudentService } from "../../src/application/services/student.service";
import { REPOSITORIES, SERVICES } from "../../src/di-tokens";
import { IUserRepository } from "../../src/domain/user/user.repository.interface";
import { IElectiveService } from "../../src/application/ports/elective.port";
import { User } from "../../src/domain/user/user";
import { Elective } from "../../src/domain/elective/elective";
import { ok, err } from "../../src/domain/result";

describe("StudentService", () => {
  let service: StudentService;
  let userRepo: jest.Mocked<IUserRepository>;
  let electiveService: jest.Mocked<IElectiveService>;

  const mockStudent: User = {
    id: "student123",
    email: "student@example.com",
    passwordHash: "hashed",
    firstName: "John",
    lastName: "Doe",
    role: "student",
    favorites: [],
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
      providers: [
        StudentService,
        {
          provide: REPOSITORIES.USER,
          useValue: mockUserRepo,
        },
        {
          provide: SERVICES.ELECTIVE,
          useValue: mockElectiveService,
        },
      ],
    }).compile();

    service = module.get<StudentService>(StudentService);
    userRepo = module.get(REPOSITORIES.USER);
    electiveService = module.get(SERVICES.ELECTIVE);
  });

  it("should be defined", () => {
    expect(service).toBeDefined();
  });

  describe("getFavorites", () => {
    it("should return empty array for student with no favorites", async () => {
      userRepo.findById.mockResolvedValue(mockStudent);

      const result = await service.getFavorites("student123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toEqual([]);
      }
    });

    it("should return favorites for student", async () => {
      const studentWithFavorites: User = {
        ...mockStudent,
        favorites: ["elective123"],
      };
      userRepo.findById.mockResolvedValue(studentWithFavorites);
      electiveService.getElectiveById.mockResolvedValue(ok(mockElective));

      const result = await service.getFavorites("student123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toHaveLength(1);
        expect(result.data[0]).toEqual(mockElective);
      }
    });

    it("should skip electives that cannot be found", async () => {
      const studentWithFavorites: User = {
        ...mockStudent,
        favorites: ["elective123", "nonexistent"],
      };
      userRepo.findById.mockResolvedValue(studentWithFavorites);
      electiveService.getElectiveById
        .mockResolvedValueOnce(ok(mockElective))
        .mockResolvedValueOnce(err("ELECTIVE_NOT_FOUND", "Not found"));

      const result = await service.getFavorites("student123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toHaveLength(1);
      }
    });

    it("should return error if student not found", async () => {
      userRepo.findById.mockResolvedValue(null);

      const result = await service.getFavorites("nonexistent");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("STUDENT_NOT_FOUND");
      }
    });

    it("should return error if user is not a student", async () => {
      const teacher: User = {
        id: "teacher123",
        email: "teacher@example.com",
        passwordHash: "hashed",
        firstName: "Jane",
        lastName: "Smith",
        role: "teacher",
      };
      userRepo.findById.mockResolvedValue(teacher);

      const result = await service.getFavorites("teacher123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("STUDENT_NOT_FOUND");
      }
    });
  });

  describe("addFavorite", () => {
    it("should add elective to favorites", async () => {
      userRepo.findById.mockResolvedValue(mockStudent);
      userRepo.update.mockResolvedValue({
        ...mockStudent,
        favorites: ["elective123"],
      });

      const result = await service.addFavorite("student123", "elective123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe(true);
      }
    });

    it("should return error if student not found", async () => {
      userRepo.findById.mockResolvedValue(null);

      const result = await service.addFavorite("nonexistent", "elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("STUDENT_NOT_FOUND");
      }
    });

    it("should return error if user is not a student", async () => {
      const teacher: User = {
        id: "teacher123",
        email: "teacher@example.com",
        passwordHash: "hashed",
        firstName: "Jane",
        lastName: "Smith",
        role: "teacher",
      };
      userRepo.findById.mockResolvedValue(teacher);

      const result = await service.addFavorite("teacher123", "elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("STUDENT_NOT_FOUND");
      }
    });

    it("should return error if elective is already a favorite", async () => {
      const studentWithFavorite: User = {
        ...mockStudent,
        favorites: ["elective123"],
      };
      userRepo.findById.mockResolvedValue(studentWithFavorite);

      const result = await service.addFavorite("student123", "elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_ALREADY_FAVORITE");
      }
    });
  });

  describe("removeFavorite", () => {
    it("should remove elective from favorites", async () => {
      const studentWithFavorite: User = {
        ...mockStudent,
        favorites: ["elective123"],
      };
      userRepo.findById.mockResolvedValue(studentWithFavorite);
      userRepo.update.mockResolvedValue(mockStudent);

      const result = await service.removeFavorite("student123", "elective123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe(true);
      }
    });

    it("should return error if student not found", async () => {
      userRepo.findById.mockResolvedValue(null);

      const result = await service.removeFavorite("nonexistent", "elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("STUDENT_NOT_FOUND");
      }
    });

    it("should return error if user is not a student", async () => {
      const teacher: User = {
        id: "teacher123",
        email: "teacher@example.com",
        passwordHash: "hashed",
        firstName: "Jane",
        lastName: "Smith",
        role: "teacher",
      };
      userRepo.findById.mockResolvedValue(teacher);

      const result = await service.removeFavorite("teacher123", "elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("STUDENT_NOT_FOUND");
      }
    });

    it("should return error if elective is not a favorite", async () => {
      const studentWithoutFavorite: User = {
        ...mockStudent,
        favorites: [],
      };
      userRepo.findById.mockResolvedValue(studentWithoutFavorite);

      const result = await service.removeFavorite("student123", "elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ELECTIVE_NOT_FAVORITE");
      }
    });
  });

  describe("isFavorite", () => {
    it("should return true if elective is a favorite", async () => {
      const studentWithFavorite: User = {
        ...mockStudent,
        favorites: ["elective123"],
      };
      userRepo.findById.mockResolvedValue(studentWithFavorite);

      const result = await service.isFavorite("student123", "elective123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe(true);
      }
    });

    it("should return false if elective is not a favorite", async () => {
      const studentWithoutFavorite: User = {
        ...mockStudent,
        favorites: [],
      };
      userRepo.findById.mockResolvedValue(studentWithoutFavorite);

      const result = await service.isFavorite("student123", "elective123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe(false);
      }
    });

    it("should return error if student not found", async () => {
      userRepo.findById.mockResolvedValue(null);

      const result = await service.isFavorite("nonexistent", "elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("STUDENT_NOT_FOUND");
      }
    });

    it("should return error if user is not a student", async () => {
      const teacher: User = {
        id: "teacher123",
        email: "teacher@example.com",
        passwordHash: "hashed",
        firstName: "Jane",
        lastName: "Smith",
        role: "teacher",
      };
      userRepo.findById.mockResolvedValue(teacher);

      const result = await service.isFavorite("teacher123", "elective123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("STUDENT_NOT_FOUND");
      }
    });
  });
});
