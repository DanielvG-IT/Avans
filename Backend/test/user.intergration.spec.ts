import { Test, TestingModule } from '@nestjs/testing';
import { UserController } from '@/presentation/controllers/user.controller';
import { UserService } from '@/application/services/user.service';
import { SessionGuard } from '@/presentation/guards/session.guard';
import { ExecutionContext } from '@nestjs/common';
import { LoggerService } from '@/logger.service';
import { AuthenticatedSession } from '@/types/session.types';

describe('User Integration Test', () => {
  let userController: UserController;
  let userService: UserService;
  let mockUserRepository: any;
  let mockUserModulesRepository: any;

  const mockLogger = {
    setContext: jest.fn(),
    log: jest.fn(),
    warn: jest.fn(),
    error: jest.fn(),
  };

  const mockUser = {
    id: 'user-1',
    name: 'Test User',
    email: 'test@example.com',
    hashedPassword: 'hashed-password',
    role: 'STUDENT',
    createdAt: new Date(),
    updatedAt: new Date(),
  };

  beforeEach(async () => {
    mockUserRepository = { findById: jest.fn() };
    mockUserModulesRepository = {
      findFavoritesByUserId: jest.fn(),
      isFavorited: jest.fn(),
      addFavorite: jest.fn(),
      removeFavorite: jest.fn(),
      setRecommendedModules: jest.fn(),
      findRecommendedByUserId: jest.fn(),
    };

    const module: TestingModule = await Test.createTestingModule({
      controllers: [UserController],
      providers: [
        // Service token zoals in AppModule
        { provide: 'SERVICE.USER', useClass: UserService },
        { provide: 'REPO.USER', useValue: mockUserRepository },
        { provide: 'REPO.USER_MODULES', useValue: mockUserModulesRepository },
        { provide: LoggerService, useValue: mockLogger },
      ],
    })
      .overrideGuard(SessionGuard)
      .useValue({
        canActivate: (context: ExecutionContext) => true,
      })
      .compile();

    userController = module.get<UserController>(UserController);
    userService = module.get<UserService>('SERVICE.USER');
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  it('should return user profile', async () => {
    mockUserRepository.findById.mockResolvedValue({
      _tag: 'Success',
      data: mockUser,
    });
    const session = { user: mockUser } as unknown as AuthenticatedSession;

    const result = await userController.getProfile(session);
    expect(result.user.email).toBe(mockUser.email);
  });

  // Voeg hier je andere tests toe: favorites, recommended modules, etc.
});
