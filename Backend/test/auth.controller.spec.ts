import { Test, TestingModule } from '@nestjs/testing';
import { AuthController } from '@/presentation/controllers/auth.controller';
import { IAuthService } from '@/application/ports/auth.port';
import { LoginDto } from '@/presentation/dtos/auth.dto';
import { User } from '@/domain/user/user.model';
import { fail, succeed } from '@/result';
import { AuthenticatedSession, SessionData } from '@/types/session.types';
import { ConflictException, BadRequestException } from '@nestjs/common';
import { SessionGuard } from '@/presentation/guards/session.guard';

describe('AuthController', () => {
  let authController: AuthController;
  let authService: jest.Mocked<IAuthService>;

  const mockUser: User = {
    id: 'user-1',
    name: 'Test User',
    email: 'test@example.com',
    hashedPassword: 'hashed-password',
    role: 'STUDENT',
    createdAt: new Date(),
    updatedAt: new Date(),
  };

  const mockSession = () =>
    ({
      user: undefined,
      lastActivity: undefined,
      destroy: jest.fn((cb) => cb()),
    }) as unknown as SessionData;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [AuthController],
      providers: [
        {
          provide: 'SERVICE.AUTH',
          useValue: {
            login: jest.fn(),
          },
        },
      ],
    })
      // Guards mocken zodat ze altijd 'pass' geven
      .overrideGuard(SessionGuard)
      .useValue({ canActivate: () => true })
      .compile();

    authController = module.get(AuthController);
    authService = module.get('SERVICE.AUTH');
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  describe('login', () => {
    it('should return name and role when valid credentials are provided', async () => {
      const session = mockSession();
      const dto: LoginDto = { email: mockUser.email, password: 'password' };

      authService.login.mockResolvedValue(succeed({ user: mockUser }));

      const result = await authController.login(dto, session);

      // Response should only contain name and role (not email or other sensitive data)
      expect(result.user.name).toBe(mockUser.name);
      expect(result.user.role).toBe(mockUser.role);
      expect(result.user).not.toHaveProperty('email');
      expect(result.user).not.toHaveProperty('hashedPassword');
      // Session should still contain full user object
      expect(session.user).toEqual(mockUser);
      expect(session.lastActivity).toBeDefined();
      expect(authService.login).toHaveBeenCalledWith(dto.email, dto.password);
    });

    it('should throw BadRequest when invalid credentials are provided', async () => {
      const session = mockSession();
      const dto: LoginDto = { email: 'wrong@example.com', password: '1234' };

      authService.login.mockResolvedValue(
        fail(new Error('Invalid credentials')),
      );

      await expect(authController.login(dto, session)).rejects.toThrow(
        BadRequestException,
      );
    });

    it('should throw Conflict when user is already authenticated', async () => {
      const session = mockSession();
      session.user = mockUser;
      const dto: LoginDto = { email: mockUser.email, password: 'password' };

      await expect(authController.login(dto, session)).rejects.toThrow(
        ConflictException,
      );
    });
  });

  describe('logout', () => {
    it('should destroy session and clear cookie when user logs out', async () => {
      const destroyMock = jest.fn((cb) => cb());
      const session = {
        user: mockUser,
        destroy: destroyMock,
      } as unknown as AuthenticatedSession;

      const res = { clearCookie: jest.fn() } as unknown as any;

      const result = await authController.logout(session, res);

      expect(result.message).toBe('Successfully logged out');
      expect(destroyMock).toHaveBeenCalled();
      expect(res.clearCookie).toHaveBeenCalledWith('connect.sid');
    });

    it('should throw BadRequest when session destruction fails', async () => {
      const destroyMock = jest.fn((cb) => cb(new Error('fail')));
      const session = {
        user: mockUser,
        destroy: destroyMock,
      } as unknown as AuthenticatedSession;

      const res = { clearCookie: jest.fn() } as unknown as any;

      await expect(authController.logout(session, res)).rejects.toThrow(
        BadRequestException,
      );
    });
  });
});
