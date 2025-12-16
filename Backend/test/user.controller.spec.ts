import { Test, TestingModule } from '@nestjs/testing';
import { UserController } from '../src/presentation/controllers/user.controller';
import { IUserService } from '../src/application/ports/user.port';

const mockUserService: Partial<IUserService> = {
  getAll: jest.fn(),
  getById: jest.fn(),
  update: jest.fn(),
  delete: jest.fn(),
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
      ],
    }).compile();

    controller = module.get<UserController>(UserController);
  });

  it('should be defined', () => {
    expect(controller).toBeDefined();
  });
});
