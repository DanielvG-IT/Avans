import { Test, TestingModule } from "@nestjs/testing";
import { ExecutionContext, UnauthorizedException } from "@nestjs/common";
import { JwtService } from "@nestjs/jwt";
import { AuthGuard } from "../../src/interfaces/guards/auth.guard";
import { jwtSecret } from "../../src/constants";

describe("AuthGuard", () => {
  let guard: AuthGuard;
  let jwtService: jest.Mocked<JwtService>;

  beforeEach(async () => {
    const mockJwtService = {
      verifyAsync: jest.fn(),
      signAsync: jest.fn(),
    };

    const module: TestingModule = await Test.createTestingModule({
      providers: [
        AuthGuard,
        {
          provide: JwtService,
          useValue: mockJwtService,
        },
      ],
    }).compile();

    guard = module.get<AuthGuard>(AuthGuard);
    jwtService = module.get(JwtService);
  });

  it("should be defined", () => {
    expect(guard).toBeDefined();
  });

  describe("canActivate", () => {
    const createMockExecutionContext = (cookies?: any): ExecutionContext => {
      return {
        switchToHttp: () => ({
          getRequest: () => ({
            cookies: cookies || {},
          }),
        }),
      } as ExecutionContext;
    };

    it("should return true when valid token is provided", async () => {
      const mockPayload = {
        sub: "user123",
        email: "test@example.com",
        role: "student",
        first: "John",
        last: "Doe",
      };

      const mockContext = createMockExecutionContext({ ACCESSTOKEN: "valid.jwt.token" });
      jwtService.verifyAsync.mockResolvedValue(mockPayload);

      const result = await guard.canActivate(mockContext);

      expect(result).toBe(true);
      expect(jwtService.verifyAsync).toHaveBeenCalledWith("valid.jwt.token", {
        secret: jwtSecret,
      });
    });

    it("should throw UnauthorizedException when no token is provided", async () => {
      const mockContext = createMockExecutionContext({});

      await expect(guard.canActivate(mockContext)).rejects.toThrow(UnauthorizedException);
    });

    it("should throw UnauthorizedException when cookies are undefined", async () => {
      const mockContext = createMockExecutionContext(undefined);

      await expect(guard.canActivate(mockContext)).rejects.toThrow(UnauthorizedException);
    });

    it("should throw UnauthorizedException when ACCESSTOKEN is not a string", async () => {
      const mockContext = createMockExecutionContext({ ACCESSTOKEN: 123 });

      await expect(guard.canActivate(mockContext)).rejects.toThrow(UnauthorizedException);
    });

    it("should throw UnauthorizedException when ACCESSTOKEN is an empty string", async () => {
      const mockContext = createMockExecutionContext({ ACCESSTOKEN: "" });

      await expect(guard.canActivate(mockContext)).rejects.toThrow(UnauthorizedException);
    });

    it("should throw UnauthorizedException when token verification fails", async () => {
      const mockContext = createMockExecutionContext({ ACCESSTOKEN: "invalid.jwt.token" });
      jwtService.verifyAsync.mockRejectedValue(new Error("Invalid token"));

      await expect(guard.canActivate(mockContext)).rejects.toThrow(UnauthorizedException);
    });

    it("should attach authClaims to request when token is valid", async () => {
      const mockPayload = {
        sub: "user123",
        email: "test@example.com",
        role: "teacher",
        first: "Jane",
        last: "Smith",
      };

      const mockRequest = {
        cookies: { ACCESSTOKEN: "valid.jwt.token" },
      };

      const mockContext = {
        switchToHttp: () => ({
          getRequest: () => mockRequest,
        }),
      } as ExecutionContext;

      jwtService.verifyAsync.mockResolvedValue(mockPayload);

      await guard.canActivate(mockContext);

      expect((mockRequest as any).authClaims).toEqual(mockPayload);
    });

    it("should handle token with extra properties", async () => {
      const mockPayload = {
        sub: "user123",
        email: "test@example.com",
        role: "admin",
        first: "Admin",
        last: "User",
        iat: 1234567890,
        exp: 1234567890,
      };

      const mockContext = createMockExecutionContext({ ACCESSTOKEN: "valid.jwt.token" });
      jwtService.verifyAsync.mockResolvedValue(mockPayload);

      const result = await guard.canActivate(mockContext);

      expect(result).toBe(true);
    });
  });

  describe("extractTokenFromCookie", () => {
    it("should extract token from cookies", async () => {
      const mockContext = {
        switchToHttp: () => ({
          getRequest: () => ({
            cookies: { ACCESSTOKEN: "test.token.value" },
          }),
        }),
      } as ExecutionContext;

      jwtService.verifyAsync.mockResolvedValue({
        sub: "123",
        email: "test@example.com",
        role: "student",
        first: "Test",
        last: "User",
      });

      await guard.canActivate(mockContext);

      expect(jwtService.verifyAsync).toHaveBeenCalledWith("test.token.value", {
        secret: jwtSecret,
      });
    });

    it("should handle null token in cookies", async () => {
      const mockContext = {
        switchToHttp: () => ({
          getRequest: () => ({
            cookies: { ACCESSTOKEN: null },
          }),
        }),
      } as ExecutionContext;

      await expect(guard.canActivate(mockContext)).rejects.toThrow(UnauthorizedException);
    });
  });
});
