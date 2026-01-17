import { Test, TestingModule } from '@nestjs/testing';
import { UserService } from '@/application/services/user.service';
import { IUserRepository } from '@/domain/user/user-repository.interface';
import { IUserModulesRepository } from '@/domain/usermodule/usermodules-repository.interface';
import { LoggerService } from '@/logger.service';
import { User } from '@/domain/user/user.model';
import { UserFavorite } from '@/domain/usermodule/userfavorite.model';

describe('UserService (Unit)', () => {
  let service: UserService;
  let userRepository: jest.Mocked<IUserRepository>;
  let userModulesRepository: jest.Mocked<IUserModulesRepository>;

  // ============================
  // Mock data
  // ============================

  const mockUser: User = {
    id: 'user-1',
    name: 'Test Student',
    email: 'student@test.nl',
    hashedPassword: 'hashed-password',
    role: 'STUDENT',
    createdAt: new Date('2024-01-01'),
    updatedAt: new Date('2024-01-02'),
  };

  const mockFavorite: UserFavorite = {
    id: 1,
    userId: 'user-1',
    moduleId: 1,
    createdAt: new Date('2024-02-01'),
  };

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [
        UserService,

        // ---- Mock User Repository ----
        {
          provide: 'REPO.USER',
          useValue: {
            findById: jest
              .fn()
              .mockResolvedValue({ ok: true, value: mockUser }),
          },
        },

        // ---- Mock UserModules Repository ----
        {
          provide: 'REPO.USER_MODULES',
          useValue: {
            findFavoritesByUserId: jest.fn(),
            isFavorited: jest.fn(),
            addFavorite: jest.fn(),
            removeFavorite: jest.fn(),
            setRecommendedModules: jest.fn(),
            findRecommendedByUserId: jest.fn(),
          },
        },

        // ---- Mock LoggerService (FIX) ----
        {
          provide: LoggerService,
          useValue: {
            setContext: jest.fn(),
            log: jest.fn(),
            error: jest.fn(),
            warn: jest.fn(),
          },
        },
      ],
    }).compile();

    service = module.get(UserService);
    userRepository = module.get('REPO.USER');
    userModulesRepository = module.get('REPO.USER_MODULES');
  });

  // ============================
  // User profile
  // ============================

  it('findById → retourneert een gebruiker', async () => {
    (userRepository.findById as jest.Mock).mockResolvedValue({
      ok: true,
      value: mockUser,
    });

    const result = await service.findById('user-1');

    expect(result).toEqual({ ok: true, value: mockUser });
    expect(userRepository.findById).toHaveBeenCalledWith('user-1');
  });

  // ============================
  // Favorites
  // ============================

  it('findFavorites → retourneert favoriete modules', async () => {
    userModulesRepository.findFavoritesByUserId.mockResolvedValue([
      mockFavorite,
    ]);

    const result = await service.findFavorites('user-1');

    expect(result).toHaveLength(1);
    expect(result[0].moduleId).toBe(1);
  });

  it('isModuleFavorited → retourneert true als module favoriet is', async () => {
    userModulesRepository.isFavorited.mockResolvedValue(true);

    const result = await service.isModuleFavorited('user-1', 1);

    expect(result).toBe(true);
  });

  it('favoriteModule → voegt favoriet toe als deze nog niet bestaat', async () => {
    userModulesRepository.isFavorited.mockResolvedValue(false);

    await service.favoriteModule('user-1', 1);

    expect(userModulesRepository.addFavorite).toHaveBeenCalledWith('user-1', 1);
  });

  it('favoriteModule → voegt geen favoriet toe als deze al bestaat', async () => {
    userModulesRepository.isFavorited.mockResolvedValue(true);

    await service.favoriteModule('user-1', 1);

    expect(userModulesRepository.addFavorite).not.toHaveBeenCalled();
  });

  it('unfavoriteModule → verwijdert favoriet', async () => {
    await service.unfavoriteModule('user-1', 1);

    expect(userModulesRepository.removeFavorite).toHaveBeenCalledWith(
      'user-1',
      1,
    );
  });

  // ============================
  // Recommended modules
  // ============================

  it('setRecommendedModules → slaat aanbevelingen op', async () => {
    const recommendations = [
      { moduleId: 1, motivation: 'Past bij je profiel' },
    ];

    await service.setRecommendedModules('user-1', recommendations);

    expect(userModulesRepository.setRecommendedModules).toHaveBeenCalledWith(
      'user-1',
      recommendations,
    );
  });

  it('getRecommendedModuleIds → retourneert alleen module IDs', async () => {
    userModulesRepository.findRecommendedByUserId.mockResolvedValue([
      { moduleId: 1 },
      { moduleId: 2 },
    ] as any);

    const result = await service.getRecommendedModuleIds('user-1');

    expect(result).toEqual([1, 2]);
  });

  it('getRecommendedModules → retourneert module IDs met motivatie', async () => {
    userModulesRepository.findRecommendedByUserId.mockResolvedValue([
      {
        moduleId: 1,
        recommendationReason: 'Interessant voor jou',
      },
    ] as any);

    const result = await service.getRecommendedModules('user-1');

    expect(result).toEqual([
      { moduleId: 1, motivation: 'Interessant voor jou' },
    ]);
  });
});
