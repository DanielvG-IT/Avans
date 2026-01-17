import { Test, TestingModule } from '@nestjs/testing';
import {
  AiService,
  PredictionWithModules,
} from '@/application/services/ai.service';
import { IPredictionClient } from '@/domain/prediction/prediction-client.interface';
import { IModuleService } from '@/application/ports/module.port';
import { PredictionDto } from '@/presentation/dtos/ai.dto';
import { Result } from '@/result';
import { LoggerService } from '@/logger.service';

describe('AiService', () => {
  let aiService: AiService;
  let predictionClient: jest.Mocked<IPredictionClient>;
  let moduleService: jest.Mocked<IModuleService>;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [
        AiService,
        {
          provide: 'CLIENT.PREDICTION',
          useValue: {
            getPrediction: jest.fn(),
          },
        },
        {
          provide: 'SERVICE.MODULE',
          useValue: {
            findManyByIds: jest.fn(),
          },
        },
        {
          provide: LoggerService, // ✅ voeg mock LoggerService toe
          useValue: {
            log: jest.fn(),
            error: jest.fn(),
            setContext: jest.fn(),
          },
        },
      ],
    }).compile();

    aiService = module.get<AiService>(AiService);
    predictionClient = module.get('CLIENT.PREDICTION');
    moduleService = module.get('SERVICE.MODULE');
  });

  it('should return predictions with full modules (success)', async () => {
    predictionClient.getPrediction.mockResolvedValue([
      { id: 1, similarity_score: 0.9, motivation: 'Perfect match' },
      { id: 2, similarity_score: 0.8, motivation: 'Good fit' },
    ]);

    moduleService.findManyByIds.mockResolvedValue([
      {
        id: 1,
        name: 'AI Basics',
        shortDescription: 'Intro',
        studyCredits: 5,
        level: 'HBO',
        location: [{ id: 1, name: 'Breda' }],
        moduleTags: [{ id: 1, name: 'AI' }],
        learningOutcomes: 'AI begrijpen',
        startDate: '2025-02-01',
        description: '',
        content: '',
        availableSpots: 0,
      },
      {
        id: 2,
        name: 'Web Dev',
        shortDescription: 'Frontend & Backend',
        studyCredits: 5,
        level: 'HBO',
        location: [{ id: 1, name: 'Breda' }],
        moduleTags: [],
        learningOutcomes: '',
        startDate: '2025-02-01',
        description: '',
        content: '',
        availableSpots: 0,
      },
    ]);

    const dto: PredictionDto = {
      currentStudy: 'CS',
      interests: ['AI'],
      wantedStudyCreditRange: [5, 10],
      locationPreference: ['Amsterdam'],
      learningGoals: ['ML'],
      levelPreference: ['HBO'],
      preferredLanguage: 'EN',
      preferredPeriod: ['P2'],
    };

    const result = await aiService.getPrediction(dto);

    expect(result._tag).toBe('Success');
    if (result._tag === 'Success') {
      expect(result.data.predictions).toHaveLength(2);
      expect(result.data.predictions[0].module.name).toBe('AI Basics');
    }
  });

  it('should fail if a module is missing', async () => {
    predictionClient.getPrediction.mockResolvedValue([
      { id: 1, similarity_score: 0.9, motivation: 'Perfect match' },
    ]);
    moduleService.findManyByIds.mockResolvedValue([]); // simulate missing module

    const dto: PredictionDto = {
      currentStudy: 'CS',
      interests: ['AI'],
      wantedStudyCreditRange: [5, 10],
      locationPreference: ['Amsterdam'],
      learningGoals: ['ML'],
      levelPreference: ['HBO'],
      preferredLanguage: 'EN',
      preferredPeriod: ['P2'],
    };

    const result = await aiService.getPrediction(dto);

    expect(result._tag).toBe('Failure');
    if (result._tag === 'Failure') {
      expect(result.error.message).toContain('Kan de aanbevolen module');
    }
  });

  it('should fail if prediction client throws', async () => {
    predictionClient.getPrediction.mockRejectedValue(
      new Error('Service unavailable'),
    );

    const dto: PredictionDto = {
      currentStudy: 'CS',
      interests: ['AI'],
      wantedStudyCreditRange: [5, 10],
      locationPreference: ['Amsterdam'],
      learningGoals: ['ML'],
      levelPreference: ['HBO'],
      preferredLanguage: 'EN',
      preferredPeriod: ['P2'],
    };

    const result = await aiService.getPrediction(dto);

    expect(result._tag).toBe('Failure');
    if (result._tag === 'Failure') {
      expect(result.error.message).toBe('Service unavailable');
    }
  });

  it('should return empty predictions array if client returns empty', async () => {
    predictionClient.getPrediction.mockResolvedValue([]);
    moduleService.findManyByIds.mockResolvedValue([]);

    const dto: PredictionDto = {
      currentStudy: 'CS',
      interests: ['AI'],
      wantedStudyCreditRange: [5, 10],
      locationPreference: ['Amsterdam'],
      learningGoals: ['ML'],
      levelPreference: ['HBO'],
      preferredLanguage: 'EN',
      preferredPeriod: ['P2'],
    };

    const result = await aiService.getPrediction(dto);

    expect(result._tag).toBe('Success');
    if (result._tag === 'Success') {
      expect(result.data.predictions).toHaveLength(0);
    }
  });
});
