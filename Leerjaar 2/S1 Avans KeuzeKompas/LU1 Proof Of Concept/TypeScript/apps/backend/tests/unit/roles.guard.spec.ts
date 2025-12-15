import { Test, TestingModule } from "@nestjs/testing";
import { ExecutionContext, ForbiddenException } from "@nestjs/common";
import { Reflector } from "@nestjs/core";
import { RolesGuard } from "../../src/interfaces/guards/roles.guard";
import { ROLES_KEY } from "../../src/interfaces/decorators/roles.decorator";

describe("RolesGuard", () => {
  let guard: RolesGuard;
  let reflector: jest.Mocked<Reflector>;

  beforeEach(async () => {
    const mockReflector = {
      getAllAndOverride: jest.fn(),
    };

    const module: TestingModule = await Test.createTestingModule({
      providers: [
        RolesGuard,
        {
          provide: Reflector,
          useValue: mockReflector,
        },
      ],
    }).compile();

    guard = module.get<RolesGuard>(RolesGuard);
    reflector = module.get(Reflector);
  });

  it("should be defined", () => {
    expect(guard).toBeDefined();
  });

  const createMockExecutionContext = (authClaims?: any): ExecutionContext => {
    return {
      switchToHttp: () => ({
        getRequest: () => ({
          authClaims,
        }),
      }),
      getHandler: jest.fn(),
      getClass: jest.fn(),
    } as any;
  };

  describe("canActivate", () => {
    it("should allow access when no roles are required", () => {
      reflector.getAllAndOverride.mockReturnValue(null);
      const mockContext = createMockExecutionContext();

      const result = guard.canActivate(mockContext);

      expect(result).toBe(true);
    });

    it("should allow access when required roles array is empty", () => {
      reflector.getAllAndOverride.mockReturnValue([]);
      const mockContext = createMockExecutionContext();

      const result = guard.canActivate(mockContext);

      expect(result).toBe(true);
    });

    it("should allow access when user has required role", () => {
      reflector.getAllAndOverride.mockReturnValue(["student"]);
      const mockContext = createMockExecutionContext({
        sub: "user123",
        email: "student@example.com",
        role: "student",
        first: "John",
        last: "Doe",
      });

      const result = guard.canActivate(mockContext);

      expect(result).toBe(true);
      expect(reflector.getAllAndOverride).toHaveBeenCalledWith(ROLES_KEY, [
        mockContext.getHandler(),
        mockContext.getClass(),
      ]);
    });

    it("should allow access when user has one of multiple required roles", () => {
      reflector.getAllAndOverride.mockReturnValue(["admin", "teacher"]);
      const mockContext = createMockExecutionContext({
        sub: "user123",
        email: "teacher@example.com",
        role: "teacher",
        first: "Jane",
        last: "Smith",
      });

      const result = guard.canActivate(mockContext);

      expect(result).toBe(true);
    });

    it("should throw ForbiddenException when user role not found", () => {
      reflector.getAllAndOverride.mockReturnValue(["admin"]);
      const mockContext = createMockExecutionContext({
        sub: "user123",
        email: "user@example.com",
      });

      expect(() => guard.canActivate(mockContext)).toThrow(ForbiddenException);
      expect(() => guard.canActivate(mockContext)).toThrow("User role not found");
    });

    it("should throw ForbiddenException when authClaims is missing", () => {
      reflector.getAllAndOverride.mockReturnValue(["admin"]);
      const mockContext = createMockExecutionContext(undefined);

      expect(() => guard.canActivate(mockContext)).toThrow(ForbiddenException);
      expect(() => guard.canActivate(mockContext)).toThrow("User role not found");
    });

    it("should throw ForbiddenException when user does not have required role", () => {
      reflector.getAllAndOverride.mockReturnValue(["admin"]);
      const mockContext = createMockExecutionContext({
        sub: "user123",
        email: "student@example.com",
        role: "student",
        first: "John",
        last: "Doe",
      });

      expect(() => guard.canActivate(mockContext)).toThrow(ForbiddenException);
      expect(() => guard.canActivate(mockContext)).toThrow(
        "Access denied. Required roles: admin. Your role: student",
      );
    });

    it("should throw ForbiddenException with multiple required roles in message", () => {
      reflector.getAllAndOverride.mockReturnValue(["admin", "teacher"]);
      const mockContext = createMockExecutionContext({
        sub: "user123",
        email: "student@example.com",
        role: "student",
        first: "John",
        last: "Doe",
      });

      expect(() => guard.canActivate(mockContext)).toThrow(ForbiddenException);
      expect(() => guard.canActivate(mockContext)).toThrow(
        "Access denied. Required roles: admin, teacher. Your role: student",
      );
    });

    it("should handle admin role correctly", () => {
      reflector.getAllAndOverride.mockReturnValue(["admin"]);
      const mockContext = createMockExecutionContext({
        sub: "admin123",
        email: "admin@example.com",
        role: "admin",
        first: "Admin",
        last: "User",
      });

      const result = guard.canActivate(mockContext);

      expect(result).toBe(true);
    });

    it("should be case-sensitive for roles", () => {
      reflector.getAllAndOverride.mockReturnValue(["Admin"]);
      const mockContext = createMockExecutionContext({
        sub: "user123",
        email: "admin@example.com",
        role: "admin",
        first: "Admin",
        last: "User",
      });

      expect(() => guard.canActivate(mockContext)).toThrow(ForbiddenException);
    });
  });
});
