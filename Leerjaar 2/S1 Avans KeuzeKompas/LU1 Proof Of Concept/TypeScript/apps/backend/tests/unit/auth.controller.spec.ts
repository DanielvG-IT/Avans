import { Test, TestingModule } from "@nestjs/testing";
import { AuthController } from "../../src/interfaces/controllers/auth.controller";
import { SERVICES } from "../../src/di-tokens";
import { IAuthService } from "../../src/application/ports/auth.port";
import { ok, err } from "../../src/domain/result";
import { UnauthorizedException } from "@nestjs/common";
import { Response } from "express";

describe("AuthController", () => {
  let controller: AuthController;
  let authService: jest.Mocked<IAuthService>;
  let mockResponse: Partial<Response>;

  beforeEach(async () => {
    const mockAuthService = {
      login: jest.fn(),
    };

    mockResponse = {
      cookie: jest.fn(),
    };

    const module: TestingModule = await Test.createTestingModule({
      controllers: [AuthController],
      providers: [
        {
          provide: SERVICES.AUTH,
          useValue: mockAuthService,
        },
      ],
    }).compile();

    controller = module.get<AuthController>(AuthController);
    authService = module.get(SERVICES.AUTH);
  });

  it("should be defined", () => {
    expect(controller).toBeDefined();
  });

  describe("login", () => {
    const loginDto = {
      email: "test@example.com",
      password: "password123",
    };

    it("should login successfully and set cookie", async () => {
      const accessToken = "mock.jwt.token";
      authService.login.mockResolvedValue(ok({ accessToken }));

      const result = await controller.login(loginDto, mockResponse as Response);

      expect(result).toEqual({ accessToken });
      expect(mockResponse.cookie).toHaveBeenCalledWith(
        "ACCESSTOKEN",
        accessToken,
        expect.objectContaining({
          httpOnly: expect.any(Boolean),
          secure: expect.any(Boolean),
          sameSite: "lax",
          maxAge: 3600000,
        }),
      );
    });

    it("should throw UnauthorizedException when login fails", async () => {
      authService.login.mockResolvedValue(err("INVALID_CREDENTIALS", "Invalid email or password"));

      await expect(controller.login(loginDto, mockResponse as Response)).rejects.toThrow(
        UnauthorizedException,
      );
    });

    it("should throw UnauthorizedException with default message when no error message provided", async () => {
      authService.login.mockResolvedValue(err("UNKNOWN_ERROR", undefined));

      await expect(controller.login(loginDto, mockResponse as Response)).rejects.toThrow(
        "Authentication failed",
      );
    });
  });
});
