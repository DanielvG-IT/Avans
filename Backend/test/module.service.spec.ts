import { Test, TestingModule } from '@nestjs/testing';
import { ModuleService } from '../src/application/services/module.service';

import { IModuleRepository } from '@/domain/module/module-repository.interface';
import { ILocationRepository } from '@/domain/location/location-repository.interface';
import { IModuleTagRepository } from '@/domain/moduletag/moduletag-repository.interface';

import {
  Module,
  ModuleDetail,
  CreateModule,
  UpdateModule,
} from '@/domain/module/module.model';

describe('ModuleService (Unit tests)', () => {
  let service: ModuleService;
  let moduleRepo: jest.Mocked<IModuleRepository>;
  let locationRepo: jest.Mocked<ILocationRepository>;
  let moduleTagRepo: jest.Mocked<IModuleTagRepository>;

  // ============================
  // Mock data
  // ============================

  const mockModuleDetail: ModuleDetail = {
    id: 1,
    name: 'Web Development',
    shortDescription: 'Leer frontend en backend',
    description: 'Uitgebreide beschrijving',
    content: 'HTML, CSS, JavaScript, API’s',
    level: 'NFQL5',
    studyCredits: 15,
    location: [{ id: 1, name: 'Breda' }],
    moduleTags: [{ id: 1, name: 'Web' }],
    learningOutcomes: 'Je kunt een webapplicatie bouwen',
    availableSpots: 20,
    startDate: '2026-02-01',
  };

  const mockModuleList: Module[] = [
    {
      id: 1,
      name: 'Web Development',
      shortDescription: 'Leer frontend en backend',
      studyCredits: 5,
      level: 'HBO',
      location: [{ id: 1, name: 'Amsterdam' }],
      startDate: '2025-02-01',
    },
  ];

  const createModuleDto: CreateModule = {
    name: 'AI Basics',
    shortDescription: 'Introductie AI',
    description: 'Basis van kunstmatige intelligentie',
    content: 'AI, ML, Data',
    level: 'HBO',
    studyCredits: 5,
    location: [{ id: 2, name: 'Utrecht' }],
    moduleTags: [{ id: 2, name: 'AI' }],
    learningOutcomes: 'Je begrijpt de basis van AI',
    availableSpots: 30,
    startDate: '2025-03-01',
  };

  // ============================
  // Test setup
  // ============================

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [
        ModuleService,
        {
          provide: 'REPO.MODULE',
          useValue: {
            getAllModules: jest.fn(),
            findById: jest.fn(),
            findManyByIds: jest.fn(),
            createModule: jest.fn(),
            updateModule: jest.fn(),
            deleteModule: jest.fn(),
          },
        },
        {
          provide: 'REPO.LOCATION',
          useValue: {
            getAllLocations: jest.fn(),
          },
        },
        {
          provide: 'REPO.MODULETAG',
          useValue: {
            getAllModuleTags: jest.fn(),
            createModuleTag: jest.fn(),
          },
        },
      ],
    }).compile();

    service = module.get<ModuleService>(ModuleService);
    moduleRepo = module.get('REPO.MODULE');
    locationRepo = module.get('REPO.LOCATION');
    moduleTagRepo = module.get('REPO.MODULETAG');
  });

  // ============================
  // Module tests
  // ============================

  it('haalt alle modules op', async () => {
    moduleRepo.getAllModules.mockResolvedValue(mockModuleList);

    const result = await service.getAllModules();

    expect(result).toEqual(mockModuleList);
    expect(moduleRepo.getAllModules).toHaveBeenCalled();
  });

  it('haalt een module op op basis van id', async () => {
    moduleRepo.findById.mockResolvedValue(mockModuleDetail);

    const result = await service.findById(1);

    expect(result).toEqual(mockModuleDetail);
    expect(moduleRepo.findById).toHaveBeenCalledWith(1);
  });

  it('maakt een nieuwe module aan', async () => {
    moduleRepo.createModule.mockResolvedValue({
      id: 2,
      ...createModuleDto,
    } as ModuleDetail);

    const result = await service.createModule(createModuleDto);

    expect(result.name).toBe('AI Basics');
    expect(moduleRepo.createModule).toHaveBeenCalledWith(createModuleDto);
  });

  it('werkt een module bij', async () => {
    const updateDto: UpdateModule = createModuleDto;

    moduleRepo.updateModule.mockResolvedValue({
      id: 1,
      ...updateDto,
    } as ModuleDetail);

    const result = await service.updateModule(1, updateDto);

    expect(result.id).toBe(1);
    expect(moduleRepo.updateModule).toHaveBeenCalledWith(1, updateDto);
  });

  it('verwijdert een module', async () => {
    moduleRepo.deleteModule.mockResolvedValue(undefined);

    await service.deleteModule(1);

    expect(moduleRepo.deleteModule).toHaveBeenCalledWith(1);
  });

  it('gooit een fout wanneer repository faalt', async () => {
    moduleRepo.findById.mockRejectedValue(new Error('Database error'));

    await expect(service.findById(999)).rejects.toThrow('Database error');
  });

  // ============================
  // Location tests
  // ============================

  it('haalt alle locaties op', async () => {
    locationRepo.getAllLocations.mockResolvedValue([
      { id: 1, name: 'Amsterdam' },
    ]);

    const result = await service.getAllLocations();

    expect(result.length).toBe(1);
    expect(locationRepo.getAllLocations).toHaveBeenCalled();
  });

  // ============================
  // ModuleTag tests
  // ============================

  it('haalt alle module tags op', async () => {
    moduleTagRepo.getAllModuleTags.mockResolvedValue([{ id: 1, name: 'Web' }]);

    const result = await service.getAllModuleTags();

    expect(result[0].name).toBe('Web');
    expect(moduleTagRepo.getAllModuleTags).toHaveBeenCalled();
  });

  it('maakt een module tag aan', async () => {
    moduleTagRepo.createModuleTag.mockResolvedValue({
      id: 3,
      name: 'Backend',
    });

    const result = await service.createModuleTag('Backend');

    expect(result.name).toBe('Backend');
    expect(moduleTagRepo.createModuleTag).toHaveBeenCalledWith('Backend');
  });
});
