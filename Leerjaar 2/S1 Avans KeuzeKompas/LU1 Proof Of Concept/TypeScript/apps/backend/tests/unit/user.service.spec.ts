import { Test, TestingModule } from "@nestjs/testing";
import { UserService } from "../../src/application/services/user.service";
import { REPOSITORIES } from "../../src/di-tokens";
import { IUserRepository } from "../../src/domain/user/user.repository.interface";
import { IElectiveRepository } from "../../src/domain/elective/elective.repository.interface";
import { User } from "../../src/domain/user/user";
import { PasswordUtil } from "../../src/application/utils/password.util";

describe("UserService", () => {
  let service: UserService;
  let userRepo: jest.Mocked<IUserRepository>;
  let electiveRepo: jest.Mocked<IElectiveRepository>;

  const mockStudent: User = {
    id: "student123",
    email: "student@example.com",
    passwordHash: "hashed",
    firstName: "John",
    lastName: "Doe",
    role: "student",
    favorites: [],
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
        UserService,
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

    service = module.get<UserService>(UserService);
    userRepo = module.get(REPOSITORIES.USER);
    electiveRepo = module.get(REPOSITORIES.ELECTIVE);
  });

  it("should be defined", () => {
    expect(service).toBeDefined();
  });

  describe("getUserById", () => {
    it("should return user by id", async () => {
      userRepo.findById.mockResolvedValue(mockStudent);

      const result = await service.getUserById("student123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toEqual(mockStudent);
      }
    });

    it("should return error if user not found", async () => {
      userRepo.findById.mockResolvedValue(null);

      const result = await service.getUserById("nonexistent");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_NOT_FOUND");
        expect(result.error.meta?.userId).toBe("nonexistent");
      }
    });
  });

  describe("getUserByEmail", () => {
    it("should return user by email", async () => {
      userRepo.findByEmail.mockResolvedValue(mockStudent);

      const result = await service.getUserByEmail("student@example.com");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toEqual(mockStudent);
      }
    });

    it("should return error if user not found", async () => {
      userRepo.findByEmail.mockResolvedValue(null);

      const result = await service.getUserByEmail("nonexistent@example.com");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_NOT_FOUND");
        expect(result.error.meta?.email).toBe("nonexistent@example.com");
      }
    });
  });

  describe("getAllUsers", () => {
    it("should return all users", async () => {
      userRepo.find.mockResolvedValue([mockStudent, mockTeacher]);

      const result = await service.getAllUsers();

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toHaveLength(2);
      }
    });

    it("should return error if fetch fails", async () => {
      userRepo.find.mockResolvedValue(null as any);

      const result = await service.getAllUsers();

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USERS_FETCH_FAILED");
      }
    });
  });

  describe("createUser", () => {
    it("should create a new user with hashed password", async () => {
      const newUser = { ...mockStudent };
      delete newUser.id;
      userRepo.findByEmail.mockResolvedValue(null);
      userRepo.create.mockResolvedValue(mockStudent);
      jest.spyOn(PasswordUtil, "hash").mockResolvedValue("hashedPassword");

      const result = await service.createUser(newUser);

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toEqual(mockStudent);
      }
      expect(PasswordUtil.hash).toHaveBeenCalled();
    });

    it("should return error if no data provided", async () => {
      const result = await service.createUser(null as any);

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("NO_USER_DATA");
      }
    });

    it("should return error if user already exists", async () => {
      const newUser = { ...mockStudent };
      userRepo.findByEmail.mockResolvedValue(mockStudent);

      const result = await service.createUser(newUser);

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_ALREADY_EXISTS");
        expect(result.error.meta?.email).toBe(newUser.email);
      }
    });

    it("should return error if creation fails", async () => {
      const newUser = { ...mockStudent };
      userRepo.findByEmail.mockResolvedValue(null);
      userRepo.create.mockResolvedValue(null as any);
      jest.spyOn(PasswordUtil, "hash").mockResolvedValue("hashedPassword");

      const result = await service.createUser(newUser);

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_CREATE_FAILED");
      }
    });
  });

  describe("updateUser", () => {
    it("should update a user", async () => {
      const updatedUser = { ...mockStudent, firstName: "Updated" };
      userRepo.findById.mockResolvedValue(mockStudent);
      userRepo.update.mockResolvedValue(updatedUser);

      const result = await service.updateUser("student123", { firstName: "Updated" });

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data.firstName).toBe("Updated");
      }
    });

    it("should return error if user not found", async () => {
      userRepo.findById.mockResolvedValue(null);

      const result = await service.updateUser("nonexistent", { firstName: "Updated" });

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_NOT_FOUND");
      }
    });

    it("should return error if update fails", async () => {
      userRepo.findById.mockResolvedValue(mockStudent);
      userRepo.update.mockResolvedValue(null as any);

      const result = await service.updateUser("student123", { firstName: "Updated" });

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_UPDATE_FAILED");
      }
    });
  });

  describe("deleteUser", () => {
    it("should delete a user with no constraints", async () => {
      const adminUser: User = {
        id: "admin123",
        email: "admin@example.com",
        passwordHash: "hashed",
        firstName: "Admin",
        lastName: "User",
        role: "admin",
      };
      userRepo.findById.mockResolvedValue(adminUser);
      userRepo.delete.mockResolvedValue(true);

      const result = await service.deleteUser("admin123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe(true);
      }
    });

    it("should return error if user not found", async () => {
      userRepo.findById.mockResolvedValue(null);

      const result = await service.deleteUser("nonexistent");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_NOT_FOUND");
      }
    });

    it("should return error if student has favorites", async () => {
      const studentWithFavorites: User = {
        ...mockStudent,
        favorites: ["elective123"],
      };
      userRepo.findById.mockResolvedValue(studentWithFavorites);

      const result = await service.deleteUser("student123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_DELETE_FAILED");
        expect(result.error.message).toContain("active favorites");
      }
    });

    it("should return error if teacher has active electives", async () => {
      userRepo.findById.mockResolvedValue(mockTeacher);
      electiveRepo.findByTeacherId.mockResolvedValue([
        {
          id: "elective123",
          code: "CS101",
          name: "Test",
          description: "Test",
          provider: "Test",
          period: "P1",
          duration: "1",
          credits: 5,
          language: "EN",
          location: "Campus",
          level: "NLQF5",
        },
      ]);

      const result = await service.deleteUser("teacher123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_DELETE_FAILED");
        expect(result.error.message).toContain("active electives");
      }
    });

    it("should delete teacher with no electives", async () => {
      userRepo.findById.mockResolvedValue(mockTeacher);
      electiveRepo.findByTeacherId.mockResolvedValue([]);
      userRepo.delete.mockResolvedValue(true);

      const result = await service.deleteUser("teacher123");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe(true);
      }
    });

    it("should return error if deletion fails", async () => {
      const adminUser: User = {
        id: "admin123",
        email: "admin@example.com",
        passwordHash: "hashed",
        firstName: "Admin",
        lastName: "User",
        role: "admin",
      };
      userRepo.findById.mockResolvedValue(adminUser);
      userRepo.delete.mockResolvedValue(false);

      const result = await service.deleteUser("admin123");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("USER_DELETE_FAILED");
      }
    });
  });
});
