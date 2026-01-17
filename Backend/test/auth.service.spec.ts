import { Test, TestingModule } from '@nestjs/testing';
import { AuthService } from '@/application/services/auth.service';
import { IUserRepository } from '@/domain/user/user-repository.interface';
import { fail, succeed } from '@/result';
import { verify } from '@node-rs/argon2';
import { User } from '@/domain/user/user.model';
import { LoggerService } from '@/logger.service';

jest.mock('@node-rs/argon2');

describe('AuthService', () => {
  let authService: AuthService;
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
    log: jest.fn(),
    error: jest.fn(),
    warn: jest.fn(),
    setContext: jest.fn(),
  };

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [
        AuthService,
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
    }).compile();

    authService = module.get(AuthService);
    userRepository = module.get('REPO.USER');
  });
  afterEach(() => {
    jest.clearAllMocks();
  });

  it('should login successfully with correct credentials', async () => {
    userRepository.findByEmail.mockResolvedValue(succeed(mockUser));
    (verify as jest.Mock).mockResolvedValue(true);

    const result = await authService.login(mockUser.email, 'password');

    expect(result._tag).toBe('Success');
    if (result._tag === 'Success') {
      expect(result.data.user.email).toBe(mockUser.email);
    }
    expect(userRepository.findByEmail).toHaveBeenCalledWith(mockUser.email);
    expect(verify).toHaveBeenCalledWith(
      mockUser.hashedPassword,
      'password',
      expect.any(Object),
    );
  });

  it('should fail login if password is incorrect', async () => {
    userRepository.findByEmail.mockResolvedValue(succeed(mockUser));
    (verify as jest.Mock).mockResolvedValue(false);

    const result = await authService.login(mockUser.email, 'wrong-password');

    expect(result._tag).toBe('Failure');
    if (result._tag === 'Failure') {
      expect(result.error.message).toBe('Invalid credentials');
    }
  });

  it('should fail login if user does not exist', async () => {
    userRepository.findByEmail.mockResolvedValue(
      fail(new Error('User not found')),
    );

    const result = await authService.login(
      'nonexistent@example.com',
      'password',
    );

    expect(result._tag).toBe('Failure');
    if (result._tag === 'Failure') {
      expect(result.error.message).toBe('User not found');
    }
  });

  it('logout should succeed', () => {
    const result = authService.logout();
    expect(result._tag).toBe('Success');
    if (result._tag === 'Success') {
      expect(result.data).toBeUndefined();
    }
  });
});
