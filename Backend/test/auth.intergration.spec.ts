import { Test, TestingModule } from '@nestjs/testing';
import { AuthController } from '@/presentation/controllers/auth.controller';
import { AuthService } from '@/application/services/auth.service';
import { IUserRepository } from '@/domain/user/user-repository.interface';
import { LoginDto } from '@/presentation/dtos/auth.dto';
import { fail, succeed } from '@/result';
import { User } from '@/domain/user/user.model';
import { verify } from '@node-rs/argon2';
import { LoggerService } from '@/logger.service';
import { SessionGuard } from '@/presentation/guards/session.guard';
import { ExecutionContext } from '@nestjs/common';

jest.mock('@node-rs/argon2');

describe('Auth Integration Test', () => {
  let authController: AuthController;
  let userRepository: jest.Mocked<IUserRepository>;

  const mockUser: User = {
    id: 'user-1',
    name: 'Test User',
    email: 'test@example.com',
    hashedPassword: 'hashed-password',
    role: 'STUDENT',
    createdAt: new Date(),
    updatedAt: new Date(),
  };

  const mockLogger = {
    setContext: jest.fn(),
    log: jest.fn(),
    error: jest.fn(),
    warn: jest.fn(),
  };

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [AuthController],
      providers: [
        // hier registreren we AuthService onder dezelfde token als de controller verwacht
        {
          provide: 'SERVICE.AUTH',
          useClass: AuthService,
        },
        {
          provide: 'REPO.USER',
          useValue: {
            findByEmail: jest.fn(),
          },
        },
        {
          provide: LoggerService,
          useValue: mockLogger,
        },
      ],
    })
      // override SessionGuard zodat hij altijd true returned
      .overrideGuard(SessionGuard)
      .useValue({
        canActivate: (context: ExecutionContext) => true,
      })
      .compile();

    authController = module.get<AuthController>(AuthController);
    userRepository = module.get('REPO.USER');
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  it('should login successfully through controller → service → repository', async () => {
    userRepository.findByEmail.mockResolvedValue(succeed(mockUser));
    (verify as jest.Mock).mockResolvedValue(true);

    const session: any = {};
    const dto: LoginDto = { email: mockUser.email, password: 'password' };

    const result = await authController.login(dto, session);

    // Response should only contain name and role (security: no email or password)
    expect(result.user.name).toBe(mockUser.name);
    expect(result.user.role).toBe(mockUser.role);
    expect(result.user).not.toHaveProperty('email');
    expect(result.user).not.toHaveProperty('hashedPassword');
    // Session should still contain full user object
    expect(session.user).toEqual(mockUser);
    expect(session.lastActivity).toBeDefined();
    expect(userRepository.findByEmail).toHaveBeenCalledWith(mockUser.email);
    expect(verify).toHaveBeenCalledWith(
      mockUser.hashedPassword,
      'password',
      expect.any(Object),
    );
  });

  it('should fail login if credentials are invalid', async () => {
    userRepository.findByEmail.mockResolvedValue(succeed(mockUser));
    (verify as jest.Mock).mockResolvedValue(false);

    const session: any = {};
    const dto: LoginDto = { email: mockUser.email, password: 'wrong-password' };

    await expect(authController.login(dto, session)).rejects.toThrow(
      'Failed to login: Invalid credentials',
    );
  });

  it('should fail login if user does not exist', async () => {
    userRepository.findByEmail.mockResolvedValue(
      fail(new Error('User not found')),
    );

    const session: any = {};
    const dto: LoginDto = { email: 'noone@test.com', password: 'password' };

    await expect(authController.login(dto, session)).rejects.toThrow(
      'Failed to login: User not found',
    );
  });
});
