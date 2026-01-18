import { Test, TestingModule } from '@nestjs/testing';
import { AiController } from '@/presentation/controllers/ai.controller';
import { AiService } from '@/application/services/ai.service';
import { BadRequestException } from '@nestjs/common';
import { PredictionDto } from '@/presentation/dtos/ai.dto';
import { LoggerService } from '@/logger.service';
import { SessionGuard } from '@/presentation/guards/session.guard';

describe('AI Integration Test', () => {
  let aiController: AiController;

  const mockLogger = {
    setContext: jest.fn(),
    log: jest.fn(),
    warn: jest.fn(),
    error: jest.fn(),
  };

  const mockPredictionClient = {
    getPrediction: jest.fn(),
  };

  const mockModuleService = {
    findManyByIds: jest.fn(),
  };

  const mockPredictionResult = [
    { id: 1, similarity_score: 0.95, motivation: 'Great match' },
    { id: 2, similarity_score: 0.87, motivation: 'Good fit' },
  ];

  const mockModules = [
    {
      id: 1,
      name: 'Math 101',
      shortDescription: 'Basic Math',
      studyCredits: 30,
      level: 'NFQL5',
      location: ['Breda', 'Roosendaal'],
      startDate: new Date(),
    },
    {
      id: 2,
      name: 'Physics 101',
      shortDescription: 'Basic Physics',
      studyCredits: 15,
      level: 'Beginner',
      location: ['Breda', 'Roosendaal'],
      startDate: new Date(),
    },
  ];

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [AiController],
      providers: [
        { provide: 'SERVICE.AI', useClass: AiService },
        { provide: 'CLIENT.PREDICTION', useValue: mockPredictionClient },
        { provide: 'SERVICE.MODULE', useValue: mockModuleService },
        { provide: LoggerService, useValue: mockLogger },
      ],
    })
      .overrideGuard(SessionGuard) // <-- guard overschrijven
      .useValue({ canActivate: () => true })
      .compile();

    aiController = module.get<AiController>(AiController);
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  it('should return predictions successfully', async () => {
    mockPredictionClient.getPrediction.mockResolvedValue(mockPredictionResult);
    mockModuleService.findManyByIds.mockResolvedValue(mockModules);

    const session: any = { user: { id: 'user-1', role: 'STUDENT' } };
    const wantedCredits: [number, number] = [15, 30];

    const dto: PredictionDto = {
      currentStudy: 'Science',
      interests: ['Math', 'Physics'],
      wantedStudyCreditRange: wantedCredits,
      locationPreference: ['Breda', 'Roosendaal'],
      learningGoals: ['Understand basics'],
      levelPreference: ['NFQL5'],
      preferredLanguage: 'EN',
      preferredPeriod: ['P1', 'P2'],
    };

    const result = await aiController.createPrediction(session, dto);

    expect(result.predictions.length).toBe(2);
    expect(result.predictions[0].module.name).toBe('Math 101');

    expect(mockPredictionClient.getPrediction).toHaveBeenCalled();
    expect(mockModuleService.findManyByIds).toHaveBeenCalledWith([1, 2]);
  });

  it('should throw BadRequestException on failure', async () => {
    mockPredictionClient.getPrediction.mockRejectedValue(
      new Error('AI service down'),
    );
    const session: any = { user: { id: 'user-1', role: 'STUDENT' } };
    const wantedCredits: [number, number] = [15, 30];

    const dto: PredictionDto = {
      currentStudy: 'Science',
      interests: [],
      wantedStudyCreditRange: wantedCredits,
      locationPreference: ['Breda', 'Roosendaal'],
      learningGoals: [],
      levelPreference: ['NFQL5'],
      preferredLanguage: 'EN',
      preferredPeriod: ['P1', 'P2'],
    };

    await expect(aiController.createPrediction(session, dto)).rejects.toThrow(
      BadRequestException,
    );
  });
});
