import { Test, TestingModule } from "@nestjs/testing";
import { UserController } from "../../src/interfaces/controllers/user.controller";
import { SERVICES } from "../../src/di-tokens";
import { IUserService } from "../../src/application/ports/user.port";
import { IStudentService } from "../../src/application/ports/student.port";
import { ITeacherService } from "../../src/application/ports/teacher.port";
import { User } from "../../src/domain/user/user";
import { Elective } from "../../src/domain/elective/elective";
import { ok, err } from "../../src/domain/result";
import { NotFoundException, UnauthorizedException, BadRequestException } from "@nestjs/common";
import { JwtService } from "@nestjs/jwt";
import { Reflector } from "@nestjs/core";
import { RequestWithCookies } from "../../src/interfaces/guards/auth.guard";

describe("UserController", () => {
  let controller: UserController;
  let userService: jest.Mocked<IUserService>;
  let studentService: jest.Mocked<IStudentService>;
  let teacherService: jest.Mocked<ITeacherService>;

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
    description: "Intro",
    provider: "Univ",
    period: "P1",
    duration: "1",
    credits: 5,
    language: "EN",
    location: "Campus",
    level: "NLQF5",
  };

  const mockRequest = (authClaims: any = null): Partial<RequestWithCookies> => ({
    authClaims,
  });

  beforeEach(async () => {
    const mockUserService = {
      getUserById: jest.fn(),
      getUserByEmail: jest.fn(),
      getAllUsers: jest.fn(),
      createUser: jest.fn(),
      updateUser: jest.fn(),
      deleteUser: jest.fn(),
    };

    const mockStudentService = {
      getFavorites: jest.fn(),
      addFavorite: jest.fn(),
      removeFavorite: jest.fn(),
      isFavorite: jest.fn(),
    };

    const mockTeacherService = {
      getElectivesGiven: jest.fn(),
    };

    const module: TestingModule = await Test.createTestingModule({
      controllers: [UserController],
      providers: [
        {
          provide: SERVICES.USER,
          useValue: mockUserService,
        },
        {
          provide: SERVICES.STUDENT,
          useValue: mockStudentService,
        },
        {
          provide: SERVICES.TEACHER,
          useValue: mockTeacherService,
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
            getAllAndOverride: jest.fn(),
          },
        },
      ],
    }).compile();

    controller = module.get<UserController>(UserController);
    userService = module.get(SERVICES.USER);
    studentService = module.get(SERVICES.STUDENT);
    teacherService = module.get(SERVICES.TEACHER);
  });

  it("should be defined", () => {
    expect(controller).toBeDefined();
  });

  describe("getProfile", () => {
    it("should return the authenticated user profile", async () => {
      const req = mockRequest({
        sub: "student123",
        email: "student@example.com",
        role: "student",
      });
      userService.getUserById.mockResolvedValue(ok(mockStudent));

      const result = await controller.getProfile(req as RequestWithCookies);

      // passwordHash should be excluded from the response
      const { passwordHash, ...expectedUser } = mockStudent;
      expect(result).toEqual(expectedUser);
    });

    it("should throw UnauthorizedException when user not found", async () => {
      const req = mockRequest({
        sub: "nonexistent",
        email: "test@example.com",
        role: "student",
      });
      userService.getUserById.mockResolvedValue(err("USER_NOT_FOUND", "User not found"));

      await expect(controller.getProfile(req as RequestWithCookies)).rejects.toThrow(
        UnauthorizedException,
      );
    });

    it("should throw UnauthorizedException when authClaims is missing", async () => {
      const req = mockRequest(null);

      await expect(controller.getProfile(req as RequestWithCookies)).rejects.toThrow(
        UnauthorizedException,
      );
    });
  });

  describe("getAllUsers", () => {
    it("should return all users", async () => {
      userService.getAllUsers.mockResolvedValue(ok([mockStudent]));

      const result = await controller.getAllUsers();

      // passwordHash should be excluded from the response
      const { passwordHash, ...expectedUser } = mockStudent;
      expect(result).toEqual([expectedUser]);
    });

    it("should throw NotFoundException when fetching users fails", async () => {
      userService.getAllUsers.mockResolvedValue(err("FETCH_FAILED", "Failed to fetch"));

      await expect(controller.getAllUsers()).rejects.toThrow(NotFoundException);
    });
  });

  describe("createUser", () => {
    it("should create a new user", async () => {
      const createDto: any = {
        email: "new@example.com",
        password: "password123",
        firstName: "New",
        lastName: "User",
        role: "student",
      };

      userService.createUser.mockResolvedValue(ok(mockStudent));

      const result = await controller.createUser(createDto);

      // passwordHash should be excluded from the response
      const { passwordHash, ...expectedUser } = mockStudent;
      expect(result).toEqual(expectedUser);
    });

    it("should throw BadRequestException when creation fails", async () => {
      const createDto: any = {
        email: "new@example.com",
        password: "password123",
        firstName: "New",
        lastName: "User",
        role: "student",
      };

      userService.createUser.mockResolvedValue(err("CREATE_FAILED", "Failed to create"));

      await expect(controller.createUser(createDto)).rejects.toThrow(BadRequestException);
    });

    it("should throw BadRequestException when missing required fields", async () => {
      const createDto: any = {
        email: "new@example.com",
        // missing other fields
      };

      await expect(controller.createUser(createDto)).rejects.toThrow(BadRequestException);
    });

    it("should create teacher user", async () => {
      const createDto: any = {
        email: "teacher@example.com",
        password: "password123",
        firstName: "Teacher",
        lastName: "User",
        role: "teacher",
      };

      const teacherUser: User = {
        id: "teacher123",
        email: "teacher@example.com",
        passwordHash: "hashed",
        firstName: "Teacher",
        lastName: "User",
        role: "teacher",
      };

      userService.createUser.mockResolvedValue(ok(teacherUser));

      const result = await controller.createUser(createDto);

      // passwordHash should be excluded from the response
      const { passwordHash, ...expectedUser } = teacherUser;
      expect(result).toEqual(expectedUser);
    });

    it("should create admin user", async () => {
      const createDto: any = {
        email: "admin@example.com",
        password: "password123",
        firstName: "Admin",
        lastName: "User",
        role: "admin",
      };

      const adminUser: User = {
        id: "admin123",
        email: "admin@example.com",
        passwordHash: "hashed",
        firstName: "Admin",
        lastName: "User",
        role: "admin",
      };

      userService.createUser.mockResolvedValue(ok(adminUser));

      const result = await controller.createUser(createDto);

      // passwordHash should be excluded from the response
      const { passwordHash, ...expectedUser } = adminUser;
      expect(result).toEqual(expectedUser);
    });
  });

  describe("getFavorites", () => {
    it("should return favorites for the authenticated student", async () => {
      const req = mockRequest({
        sub: "student123",
        email: "student@example.com",
        role: "student",
      });
      studentService.getFavorites.mockResolvedValue(ok([mockElective]));

      const result = await controller.getFavorites(req as RequestWithCookies);

      expect(result).toEqual([mockElective]);
    });

    it("should throw NotFoundException when fetching favorites fails", async () => {
      const req = mockRequest({
        sub: "student123",
        email: "student@example.com",
        role: "student",
      });
      studentService.getFavorites.mockResolvedValue(err("FETCH_FAILED", "Failed to fetch"));

      await expect(controller.getFavorites(req as RequestWithCookies)).rejects.toThrow(
        NotFoundException,
      );
    });
  });

  describe("addFavorite", () => {
    it("should add an elective to favorites", async () => {
      const req = mockRequest({
        sub: "student123",
        email: "student@example.com",
        role: "student",
      });
      studentService.addFavorite.mockResolvedValue(ok(true));

      await controller.addFavorite("elective123", req as RequestWithCookies);

      expect(studentService.addFavorite).toHaveBeenCalledWith("student123", "elective123");
    });

    it("should throw BadRequestException when elective is already a favorite", async () => {
      const req = mockRequest({
        sub: "student123",
        email: "student@example.com",
        role: "student",
      });
      studentService.addFavorite.mockResolvedValue(
        err("ELECTIVE_ALREADY_FAVORITE", "Already a favorite"),
      );

      await expect(
        controller.addFavorite("elective123", req as RequestWithCookies),
      ).rejects.toThrow(BadRequestException);
    });

    it("should throw NotFoundException when adding favorite fails", async () => {
      const req = mockRequest({
        sub: "student123",
        email: "student@example.com",
        role: "student",
      });
      studentService.addFavorite.mockResolvedValue(err("ADD_FAILED", "Failed to add"));

      await expect(
        controller.addFavorite("elective123", req as RequestWithCookies),
      ).rejects.toThrow(NotFoundException);
    });
  });

  describe("removeFavorite", () => {
    it("should remove an elective from favorites", async () => {
      const req = mockRequest({
        sub: "student123",
        email: "student@example.com",
        role: "student",
      });
      studentService.removeFavorite.mockResolvedValue(ok(true));

      await controller.removeFavorite("elective123", req as RequestWithCookies);

      expect(studentService.removeFavorite).toHaveBeenCalledWith("student123", "elective123");
    });

    it("should throw NotFoundException when removing favorite fails", async () => {
      const req = mockRequest({
        sub: "student123",
        email: "student@example.com",
        role: "student",
      });
      studentService.removeFavorite.mockResolvedValue(err("REMOVE_FAILED", "Failed to remove"));

      await expect(
        controller.removeFavorite("elective123", req as RequestWithCookies),
      ).rejects.toThrow(NotFoundException);
    });
  });

  describe("getElectives", () => {
    it("should return electives taught by the authenticated teacher", async () => {
      const req = mockRequest({
        sub: "teacher123",
        email: "teacher@example.com",
        role: "teacher",
      });
      teacherService.getElectivesGiven.mockResolvedValue(ok([mockElective]));

      const result = await controller.getElectives(req as RequestWithCookies);

      expect(result).toEqual([mockElective]);
    });

    it("should throw NotFoundException when fetching electives fails", async () => {
      const req = mockRequest({
        sub: "teacher123",
        email: "teacher@example.com",
        role: "teacher",
      });
      teacherService.getElectivesGiven.mockResolvedValue(err("FETCH_FAILED", "Failed to fetch"));

      await expect(controller.getElectives(req as RequestWithCookies)).rejects.toThrow(
        NotFoundException,
      );
    });
  });

  describe("updateUser", () => {
    it("should update a user", async () => {
      const updateDto: any = {
        firstName: "Updated",
      };

      userService.updateUser.mockResolvedValue(ok({ ...mockStudent, firstName: "Updated" }));

      const result = await controller.updateUser("student123", updateDto);

      expect(result.firstName).toBe("Updated");
    });

    it("should throw BadRequestException when update fails", async () => {
      const updateDto: any = {
        firstName: "Updated",
      };

      userService.updateUser.mockResolvedValue(err("UPDATE_FAILED", "Failed to update"));

      await expect(controller.updateUser("student123", updateDto)).rejects.toThrow(
        BadRequestException,
      );
    });
  });

  describe("deleteUser", () => {
    it("should delete a user", async () => {
      userService.deleteUser.mockResolvedValue(ok(true));

      await controller.deleteUser("student123");

      expect(userService.deleteUser).toHaveBeenCalledWith("student123");
    });

    it("should throw BadRequestException when deletion fails", async () => {
      userService.deleteUser.mockResolvedValue(err("DELETE_FAILED", "Failed to delete"));

      await expect(controller.deleteUser("student123")).rejects.toThrow(BadRequestException);
    });
  });
});
