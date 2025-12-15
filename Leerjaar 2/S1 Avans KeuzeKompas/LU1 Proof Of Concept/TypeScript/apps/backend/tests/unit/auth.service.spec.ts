import { Test, TestingModule } from "@nestjs/testing";
import { JwtService } from "@nestjs/jwt";
import { AuthService } from "../../src/application/services/auth.service";
import { SERVICES } from "../../src/di-tokens";
import { IUserService } from "../../src/application/ports/user.port";
import { PasswordUtil } from "../../src/application/utils/password.util";
import { ok, err } from "../../src/domain/result";
import { User } from "../../src/domain/user/user";

describe("AuthService", () => {
  let service: AuthService;
  let userService: jest.Mocked<IUserService>;
  let jwtService: jest.Mocked<JwtService>;

  const mockUser: User = {
    id: "user123",
    email: "test@example.com",
    passwordHash: "hashedPassword123",
    firstName: "John",
    lastName: "Doe",
    role: "student",
    favorites: [],
  };

  beforeEach(async () => {
    const mockUserService = {
      getUserByEmail: jest.fn(),
      getUserById: jest.fn(),
      getAllUsers: jest.fn(),
      createUser: jest.fn(),
      updateUser: jest.fn(),
      deleteUser: jest.fn(),
    };

    const mockJwtService = {
      signAsync: jest.fn(),
      verifyAsync: jest.fn(),
    };

    const module: TestingModule = await Test.createTestingModule({
      providers: [
        AuthService,
        {
          provide: SERVICES.USER,
          useValue: mockUserService,
        },
        {
          provide: JwtService,
          useValue: mockJwtService,
        },
      ],
    }).compile();

    service = module.get<AuthService>(AuthService);
    userService = module.get(SERVICES.USER);
    jwtService = module.get(JwtService);
  });

  it("should be defined", () => {
    expect(service).toBeDefined();
  });

  describe("login", () => {
    it("should return error if email is missing", async () => {
      const result = await service.login({ email: "", password: "password123" });

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("INVALID_INPUT");
        expect(result.error.message).toBe("Email and password are required");
      }
    });

    it("should return error if password is missing", async () => {
      const result = await service.login({ email: "test@example.com", password: "" });

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("INVALID_INPUT");
        expect(result.error.message).toBe("Email and password are required");
      }
    });

    it("should return error if data is null", async () => {
      const result = await service.login(null as any);

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("INVALID_INPUT");
      }
    });

    it("should return error if user not found", async () => {
      userService.getUserByEmail.mockResolvedValue(err("USER_NOT_FOUND", "User not found"));

      const result = await service.login({
        email: "nonexistent@example.com",
        password: "password123",
      });

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("INVALID_CREDENTIALS");
        expect(result.error.message).toBe("Invalid email or password");
      }
      expect(userService.getUserByEmail).toHaveBeenCalledWith("nonexistent@example.com");
    });

    it("should return error if password is invalid", async () => {
      userService.getUserByEmail.mockResolvedValue(ok(mockUser));
      jest.spyOn(PasswordUtil, "compare").mockResolvedValue(false);

      const result = await service.login({
        email: "test@example.com",
        password: "wrongPassword",
      });

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("INVALID_CREDENTIALS");
        expect(result.error.message).toBe("Invalid email or password");
      }
    });

    it("should return access token on successful login", async () => {
      userService.getUserByEmail.mockResolvedValue(ok(mockUser));
      jest.spyOn(PasswordUtil, "compare").mockResolvedValue(true);
      jwtService.signAsync.mockResolvedValue("mock.jwt.token");

      const result = await service.login({
        email: "test@example.com",
        password: "correctPassword",
      });

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data.accessToken).toBe("mock.jwt.token");
      }
      expect(jwtService.signAsync).toHaveBeenCalledWith({
        sub: mockUser.id,
        email: mockUser.email,
        role: mockUser.role,
        first: mockUser.firstName,
        last: mockUser.lastName,
      });
    });

    it("should return error if JWT signing fails", async () => {
      userService.getUserByEmail.mockResolvedValue(ok(mockUser));
      jest.spyOn(PasswordUtil, "compare").mockResolvedValue(true);
      jwtService.signAsync.mockRejectedValue(new Error("JWT error"));

      const result = await service.login({
        email: "test@example.com",
        password: "correctPassword",
      });

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("JWT_SIGN_ERROR");
        expect(result.error.message).toBe("Failed to generate access token");
      }
    });
  });
});
