import { Test, TestingModule } from '@nestjs/testing';
import { INestApplication, BadRequestException } from '@nestjs/common';
import request from 'supertest';

import { AiController } from '@/presentation/controllers/ai.controller';
import { IAiService } from '@/application/ports/ai.port';
import { SessionGuard } from '@/presentation/guards/session.guard';
import {
  PredictionDto,
  PredictionResponseDto,
} from '@/presentation/dtos/ai.dto';
import {
  AiService,
  PredictionWithModules,
} from '@/application/services/ai.service';

// ============================
// Mock SessionGuard
// ============================
jest.mock('@/presentation/guards/session.guard', () => ({
  SessionGuard: jest.fn().mockImplementation(() => ({
    canActivate: () => true,
  })),
}));

describe('AiController (Integration)', () => {
  let app: INestApplication;
  let aiService: jest.Mocked<IAiService>;

  const mockSession = {
    user: { id: 'user-1', role: 'STUDENT' },
  };

  const predictionDto: PredictionDto = {
    currentStudy: 'CS',
    interests: ['AI'],
    wantedStudyCreditRange: [5, 10],
    locationPreference: ['Amsterdam'],
    learningGoals: ['ML'],
    levelPreference: ['HBO'],
    preferredLanguage: 'EN',
    preferredPeriod: ['P2'],
  };

  const mockPrediction: PredictionWithModules = {
    predictions: [
      {
        module: {
          id: 1,
          name: 'AI Basics',
          shortDescription: 'Intro',
          studyCredits: 5,
          level: 'HBO',
          location: [{ id: 1, name: 'Breda' }],
          startDate: '2025-02-01',
        },
        score: 0.9,
        motivation: 'Perfect match',
      },
    ],
  };

  beforeAll(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [AiController],
      providers: [
        {
          provide: 'SERVICE.AI',
          useValue: {
            getPrediction: jest.fn(),
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

    aiService = module.get('SERVICE.AI');
  });

  afterAll(async () => {
    await app.close();
  });

  // ============================
  // POST /ai/predict → Success
  // ============================
  it('POST /ai/predict → retourneert voorspellingen', async () => {
    aiService.getPrediction.mockResolvedValue({
      _tag: 'Success',
      data: mockPrediction,
    });

    const response = await request(app.getHttpServer())
      .post('/ai/predict')
      .send(predictionDto)
      .expect(200);

    expect(response.body.predictions).toBeDefined();
    expect(response.body.predictions).toHaveLength(1);
    expect(response.body.predictions[0].module.name).toBe('AI Basics');
  });

  // ============================
  // POST /ai/predict → Failure
  // ============================
  it('POST /ai/predict → faalt bij error', async () => {
    aiService.getPrediction.mockResolvedValue({
      _tag: 'Failure',
      error: new Error('AI service unavailable'),
    });

    const response = await request(app.getHttpServer())
      .post('/ai/predict')
      .send(predictionDto)
      .expect(400);

    expect(response.body.message).toBe('Prediction generation failed');
    expect(response.body.details).toBe('AI service unavailable');
  });

  // ============================
  // POST /ai/predict → lege predictions
  // ============================
  it('POST /ai/predict → retourneert lege array', async () => {
    aiService.getPrediction.mockResolvedValue({
      _tag: 'Success',
      data: { predictions: [] },
    });

    const response = await request(app.getHttpServer())
      .post('/ai/predict')
      .send(predictionDto)
      .expect(200);

    expect(response.body.predictions).toBeDefined();
    expect(response.body.predictions).toHaveLength(0);
  });
});
