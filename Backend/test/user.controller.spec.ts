import { Test, TestingModule } from '@nestjs/testing';
import { INestApplication } from '@nestjs/common';
import request from 'supertest';

import { UserController } from '@/presentation/controllers/user.controller';
import { IUserService } from '@/application/ports/user.port';
import { SessionGuard } from '@/presentation/guards/session.guard';
import { UserRole } from '@/domain/user/user.model';

// ============================
// Mock SessionGuard
// ============================
jest.mock('@/presentation/guards/session.guard', () => ({
  SessionGuard: jest.fn().mockImplementation(() => ({
    canActivate: () => true,
  })),
}));

describe('UserController (Integration)', () => {
  let app: INestApplication;
  let userService: jest.Mocked<IUserService>;

  // ============================
  // Mock session
  // ============================
  const mockSession = {
    user: {
      id: 'user-1',
      role: 'STUDENT',
    },
  };

  // ============================
  // Mock data
  // ============================
  const mockUser = {
    id: 'user-1',
    name: 'Test Student',
    email: 'student@test.nl',
    hashedPassword: 'hashed-password',
    role: 'STUDENT' as UserRole,
    createdAt: new Date(),
    updatedAt: new Date(),
  };

  beforeAll(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [UserController],
      providers: [
        {
          provide: 'SERVICE.USER',
          useValue: {
            findById: jest.fn(),
            findFavorites: jest.fn(),
            isModuleFavorited: jest.fn(),
            favoriteModule: jest.fn(),
            unfavoriteModule: jest.fn(),
            setRecommendedModules: jest.fn(),
            getRecommendedModules: jest.fn(),
          },
        },
      ],
    })
      .overrideGuard(SessionGuard)
      .useValue({ canActivate: () => true })
      .compile();

    app = module.createNestApplication();

    // Fake session middleware
    app.use((req: any, _res: any, next: any) => {
      req.session = mockSession;
      next();
    });

    await app.init();

    userService = module.get('SERVICE.USER');
  });

  afterAll(async () => {
    await app.close();
  });

  // ============================
  // GET /user/profile
  // ============================

  it('GET /user/profile → retourneert gebruikersprofiel', async () => {
    userService.findById.mockResolvedValue({
      _tag: 'Success',
      data: mockUser,
    });

    const response = await request(app.getHttpServer())
      .get('/user/profile')
      .expect(200);

    expect(response.body.user.id).toBe('user-1');
    expect(response.body.user.email).toBe('student@test.nl');
  });

  // ============================
  // GET /user/favorites
  // ============================

  it('GET /user/favorites → retourneert favorieten', async () => {
    userService.findFavorites.mockResolvedValue([
      { userId: 'user-1', moduleId: 1, id: 1, createdAt: new Date() },
    ]);

    const response = await request(app.getHttpServer())
      .get('/user/favorites')
      .expect(200);

    expect(response.body.favorites).toHaveLength(1);
    expect(response.body.favorites[0].moduleId).toBe(1);
  });

  // ============================
  // GET /user/favorites/:moduleId
  // ============================

  it('GET /user/favorites/:moduleId → controleert favoriet', async () => {
    userService.isModuleFavorited.mockResolvedValue(true);

    const response = await request(app.getHttpServer())
      .get('/user/favorites/1')
      .expect(200);

    expect(response.body.isFavorited).toBe(true);
  });

  // ============================
  // POST /user/favorites/:moduleId
  // ============================

  it('POST /user/favorites/:moduleId → voegt favoriet toe', async () => {
    userService.favoriteModule.mockResolvedValue(undefined);

    const response = await request(app.getHttpServer())
      .post('/user/favorites/1')
      .expect(201);

    expect(response.body.success).toBe(true);
  });

  // ============================
  // DELETE /user/favorites/:moduleId
  // ============================

  it('DELETE /user/favorites/:moduleId → verwijdert favoriet', async () => {
    userService.unfavoriteModule.mockResolvedValue(undefined);

    const response = await request(app.getHttpServer())
      .delete('/user/favorites/1')
      .expect(200);

    expect(response.body.success).toBe(true);
  });

  // ============================
  // GET /user/recommended
  // ============================

  it('GET /user/recommended → retourneert max 5 modules', async () => {
    userService.getRecommendedModules.mockResolvedValue([
      { moduleId: 1 },
      { moduleId: 2 },
      { moduleId: 3 },
      { moduleId: 4 },
      { moduleId: 5 },
      { moduleId: 6 },
    ]);

    const response = await request(app.getHttpServer())
      .get('/user/recommended')
      .expect(200);

    expect(response.body.recommended).toHaveLength(5);
  });

  // ============================
  // POST /user/recommended
  // ============================

  it('POST /user/recommended → slaat aanbevelingen op', async () => {
    userService.setRecommendedModules.mockResolvedValue(undefined);

    const response = await request(app.getHttpServer())
      .post('/user/recommended')
      .send({
        modules: [{ moduleId: 1, motivation: 'Past bij je profiel' }],
      })
      .expect(201);

    expect(response.body.success).toBe(true);
  });
});
