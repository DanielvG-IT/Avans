import { Test, TestingModule } from '@nestjs/testing';
import { UserService } from '../src/application/services/user.service';
import { IUserRepository } from '../src/domain/user/user-repository.interface';
import { IUserModulesRepository } from '../src/domain/usermodule/usermodules-repository.interface';
import { LoggerService } from '../src/logger.service';

const mockUserRepository: Partial<IUserRepository> = {
  findById: jest.fn(),
  findByEmail: jest.fn(),
  create: jest.fn(),
  update: jest.fn(),
  delete: jest.fn(),
};

const mockUserModulesRepository: Partial<IUserModulesRepository> = {
  findFavoritesByUserId: jest.fn(),
  isFavorited: jest.fn(),
  addFavorite: jest.fn(),
  removeFavorite: jest.fn(),
  findRecommendedByUserId: jest.fn(),
  setRecommendedModules: jest.fn(),
};

const mockLoggerService = {
  setContext: jest.fn(),
  log: jest.fn(),
  error: jest.fn(),
  warn: jest.fn(),
  debug: jest.fn(),
  verbose: jest.fn(),
};

describe('UserService', () => {
  let service: UserService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [
        UserService,
        {
          provide: 'REPO.USER',
          useValue: mockUserRepository,
        },
        {
          provide: 'REPO.USER_MODULES',
          useValue: mockUserModulesRepository,
        },
        {
          provide: LoggerService,
          useValue: mockLoggerService,
        },
      ],
    }).compile();

    service = module.get<UserService>(UserService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });
});
