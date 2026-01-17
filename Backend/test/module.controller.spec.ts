import { Test, TestingModule } from '@nestjs/testing';
import { INestApplication } from '@nestjs/common';
import request from 'supertest';

import { ModulesController } from '../src/presentation/controllers/modules.controller';
import { IModuleService } from '@/application/ports/module.port';
import { SessionGuard } from '@/presentation/guards/session.guard';

describe('ModulesController (Integration)', () => {
  let app: INestApplication;
  let moduleService: jest.Mocked<IModuleService>;

  // ============================
  // Mock Guard
  // ============================

  const mockSessionGuard = {
    canActivate: jest.fn(() => true),
  };

  // ============================
  // Mock data
  // ============================

  const mockModule = {
    id: 1,
    name: 'Web Development',
    shortDescription: 'Leer frontend en backend',
    studyCredits: 5,
    level: 'HBO',
    location: [{ id: 1, name: 'Amsterdam' }],
    startDate: '2025-02-01',
  };

  const mockModuleDetail = {
    ...mockModule,
    description: 'Volledige beschrijving',
    content: 'HTML, CSS, JS',
    moduleTags: [{ id: 1, name: 'Web' }],
    learningOutcomes: 'Je kunt webapps bouwen',
    availableSpots: 20,
  };

  // ============================
  // Setup
  // ============================

  beforeAll(async () => {
    const moduleFixture: TestingModule = await Test.createTestingModule({
      controllers: [ModulesController],
      providers: [
        {
          provide: 'SERVICE.MODULE',
          useValue: {
            getAllModules: jest.fn(),
            findById: jest.fn(),
            createModule: jest.fn(),
            updateModule: jest.fn(),
            deleteModule: jest.fn(),
            getAllLocations: jest.fn(),
            getAllModuleTags: jest.fn(),
            createModuleTag: jest.fn(),
          },
        },
      ],
    })
      .overrideGuard(SessionGuard)
      .useValue(mockSessionGuard)
      .compile();

    app = moduleFixture.createNestApplication();
    await app.init();

    moduleService = moduleFixture.get('SERVICE.MODULE');
  });

  afterAll(async () => {
    await app.close();
  });

  // ============================
  // GET /modules
  // ============================

  it('should return list of modules when retrieving all modules', async () => {
    moduleService.getAllModules.mockResolvedValue([mockModule]);

    const response = await request(app.getHttpServer())
      .get('/modules')
      .expect(200);

    expect(response.body.modules).toHaveLength(1);
    expect(response.body.modules[0].name).toBe('Web Development');
  });

  // ============================
  // GET /modules/:id
  // ============================

  it('should return module details when valid ID is provided', async () => {
    moduleService.findById.mockResolvedValue(mockModuleDetail as any);

    const response = await request(app.getHttpServer())
      .get('/modules/1')
      .expect(200);

    expect(response.body.module.id).toBe(1);
    expect(response.body.module.name).toBe('Web Development');
  });

  // ============================
  // POST /modules
  // ============================

  it('should create and return new module when valid data is provided', async () => {
    moduleService.createModule.mockResolvedValue(mockModuleDetail as any);

    const response = await request(app.getHttpServer())
      .post('/modules')
      .send({
        name: 'AI Basics',
        shortDescription: 'Introductie AI',
        description: 'AI uitleg',
        content: 'ML, AI',
        level: 'HBO',
        studyCredits: 5,
        location: [{ id: 1, name: 'Utrecht' }],
        moduleTags: [{ id: 2, name: 'AI' }],
        learningOutcomes: 'AI begrijpen',
        availableSpots: 30,
        startDate: '2025-03-01',
      })
      .expect(201);

    expect(response.body.module.name).toBe('Web Development');
  });

  // ============================
  // PUT /modules/:id
  // ============================

  it('should update and return module when valid ID and data are provided', async () => {
    moduleService.updateModule.mockResolvedValue(mockModuleDetail as any);

    const response = await request(app.getHttpServer())
      .put('/modules/1')
      .send({
        name: 'Updated Module',
      })
      .expect(200);

    expect(response.body.module.id).toBe(1);
  });

  // ============================
  // DELETE /modules/:id
  // ============================

  it('should delete module and return success when valid ID is provided', async () => {
    moduleService.deleteModule.mockResolvedValue(undefined);

    const response = await request(app.getHttpServer())
      .delete('/modules/1')
      .expect(200);

    expect(response.body.success).toBe(true);
  });

  // ============================
  // GET /modules/locations
  // ============================

  it('should return all available locations when requested', async () => {
    moduleService.getAllLocations.mockResolvedValue([
      { id: 1, name: 'Amsterdam' },
    ]);

    const response = await request(app.getHttpServer())
      .get('/modules/locations')
      .expect(200);

    expect(response.body.locations).toHaveLength(1);
  });

  // ============================
  // GET /modules/moduletags
  // ============================

  it('should return all module tags when requested', async () => {
    moduleService.getAllModuleTags.mockResolvedValue([{ id: 1, name: 'Web' }]);

    const response = await request(app.getHttpServer())
      .get('/modules/moduletags')
      .expect(200);

    expect(response.body.moduleTags[0].name).toBe('Web');
  });

  // ============================
  // POST /modules/moduletags
  // ============================

  it('should create and return new tag when valid tag name is provided', async () => {
    moduleService.createModuleTag.mockResolvedValue({
      id: 2,
      name: 'Backend',
    });

    const response = await request(app.getHttpServer())
      .post('/modules/moduletags')
      .send({ tag: 'Backend' })
      .expect(201);

    expect(response.body.moduleTag.name).toBe('Backend');
  });
});
