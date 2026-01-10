import { Test, TestingModule } from '@nestjs/testing';
import { ConfigService } from '@nestjs/config';
import { UserController } from '../src/presentation/controllers/user.controller';
import { IUserService } from '../src/application/ports/user.port';

const mockUserService: Partial<IUserService> = {
  findById: jest.fn(),
};

const mockConfigService = {
  get: jest.fn((key: string) => {
    if (key === 'INACTIVITY_TIMEOUT') return '1800000';
    return null;
  }),
};

describe('UserController', () => {
  let controller: UserController;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [UserController],
      providers: [
        {
          provide: 'SERVICE.USER',
          useValue: mockUserService,
        },
        {
          provide: ConfigService,
          useValue: mockConfigService,
        },
      ],
    }).compile();

    controller = module.get<UserController>(UserController);
  });

  it('should be defined', () => {
    expect(controller).toBeDefined();
  });
});
