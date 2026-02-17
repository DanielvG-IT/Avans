import { Test, TestingModule } from '@nestjs/testing';
import { ModulesController } from '@/presentation/controllers/modules.controller';

import { SessionGuard } from '@/presentation/guards/session.guard';
import { Module, ModuleDetail } from '@/domain/module/module.model';
import { Location } from '@/domain/location/location.model';
import { ModuleTag } from '@/domain/moduletag/moduletag.model';
import { CreateModuleTagDto } from '@/presentation/dtos/moduleTag.dto';
import {
  CreateModuleDTO,
  UpdateModuleDTO,
} from '@/presentation/dtos/module.dto';

describe('Modules Integration Test', () => {
  let modulesController: ModulesController;

  const mockModuleService = {
    getAllModules: jest.fn(),
    findById: jest.fn(),
    findManyByIds: jest.fn(),
    createModule: jest.fn(),
    updateModule: jest.fn(),
    deleteModule: jest.fn(),
    getAllLocations: jest.fn(),
    getAllModuleTags: jest.fn(),
    createModuleTag: jest.fn(),
  };

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [ModulesController],
      providers: [{ provide: 'SERVICE.MODULE', useValue: mockModuleService }],
    })
      .overrideGuard(SessionGuard)
      .useValue({ canActivate: () => true }) // Guard overschrijven
      .compile();

    modulesController = module.get<ModulesController>(ModulesController);
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  it('should return all modules', async () => {
    const mockModules: Module[] = [
      {
        id: 1,
        name: 'Math 101',
        shortDescription: 'Basic Math',
        studyCredits: 0,
        level: '',
        location: [],
        startDate: '',
      },
      {
        id: 2,
        name: 'Physics 101',
        shortDescription: 'Basic Physics',
        studyCredits: 0,
        level: '',
        location: [],
        startDate: '',
      },
    ];
    mockModuleService.getAllModules.mockResolvedValue(mockModules);

    const result = await modulesController.getModules();
    expect(result.modules).toEqual(mockModules);
    expect(mockModuleService.getAllModules).toHaveBeenCalled();
  });

  it('should return all locations', async () => {
    const mockLocations: Location[] = [
      { id: 1, name: 'Breda' },
      { id: 2, name: 'Roosendaal' },
    ];
    mockModuleService.getAllLocations.mockResolvedValue(mockLocations);

    const result = await modulesController.getAllLocations();
    expect(result.locations).toEqual(mockLocations);
    expect(mockModuleService.getAllLocations).toHaveBeenCalled();
  });

  it('should return all module tags', async () => {
    const mockTags: ModuleTag[] = [
      { id: 1, name: 'Math' },
      { id: 2, name: 'Physics' },
    ];
    mockModuleService.getAllModuleTags.mockResolvedValue(mockTags);

    const result = await modulesController.getAllModuleTags();
    expect(result.moduleTags).toEqual(mockTags);
    expect(mockModuleService.getAllModuleTags).toHaveBeenCalled();
  });

  it('should create a module tag', async () => {
    const dto: CreateModuleTagDto = { tag: 'Chemistry' };
    const createdTag: ModuleTag = { id: 3, name: 'Chemistry' };
    mockModuleService.createModuleTag.mockResolvedValue(createdTag);

    const result = await modulesController.createModuleTag(dto);
    expect(result.moduleTag).toEqual(createdTag);
    expect(mockModuleService.createModuleTag).toHaveBeenCalledWith('Chemistry');
  });

  it('should return module by id', async () => {
    const moduleDetail: ModuleDetail = {
      id: 1,
      name: 'Math 101',
      shortDescription: 'Basic Math',
      studyCredits: 15,
      level: 'NFQL5',
      location: [{ id: 1, name: 'Breda' }],
      startDate: new Date().toISOString(),
      moduleTags: [{ id: 1, name: 'Math' }],
      description: 'Math module description',
      content: 'Math content',
      learningOutcomes: 'You will learn Math',
      availableSpots: 13,
    };
    mockModuleService.findById.mockResolvedValue(moduleDetail);

    const result = await modulesController.getModuleById(1);
    expect(result.module).toEqual(moduleDetail);
    expect(mockModuleService.findById).toHaveBeenCalledWith(1);
  });

  it('should create a module', async () => {
    const dto: CreateModuleDTO = {
      name: 'Chemistry 101',
      shortDescription: 'Basic Chemistry',
      studyCredits: 30,
      level: 'NFQL6',
      location: [{ id: 1, name: 'Breda' }],
      startDate: new Date().toISOString(),
      moduleTags: [{ id: 2, name: 'Science' }],
      description: 'Chemistry module description',
      content: 'Chemistry content',
      learningOutcomes: 'You will learn Chemistry',
      availableSpots: 10,
    };
    const createdModule: ModuleDetail = { id: 4, ...dto };
    mockModuleService.createModule.mockResolvedValue(createdModule);

    const result = await modulesController.createModule(dto);
    expect(result.module).toEqual(createdModule);
    expect(mockModuleService.createModule).toHaveBeenCalledWith(dto);
  });

  it('should update a module', async () => {
    const dto: UpdateModuleDTO = {
      name: 'Advanced Math',
      description: 'Math module description',
      content: 'Math content',
      level: 'NFQL5',
      studyCredits: 15,
      location: [{ id: 1, name: 'Breda' }],
      moduleTags: [{ id: 1, name: 'Math' }],
      learningOutcomes: 'You will learn Math',
      availableSpots: 12,
      startDate: new Date().toISOString(),
      shortDescription: 'Basic Math',
    };
    const updatedModule: ModuleDetail = {
      id: 1,
      name: 'Advanced Math',
      shortDescription: 'Basic Math',
      studyCredits: 30,
      level: 'Beginner',
      location: [{ id: 1, name: 'Breda' }],
      startDate: new Date().toISOString(),
      moduleTags: [{ id: 1, name: 'Math' }],
      description: 'Math module description',
      content: 'Math content',
      learningOutcomes: 'You will learn Math',
      availableSpots: 12,
    };
    mockModuleService.updateModule.mockResolvedValue(updatedModule);

    const result = await modulesController.updateModule(1, dto);
    expect(result.module).toEqual(updatedModule);
    expect(mockModuleService.updateModule).toHaveBeenCalledWith(1, dto);
  });

  it('should delete a module', async () => {
    mockModuleService.deleteModule.mockResolvedValue(undefined);

    const result = await modulesController.deleteModule(1);
    expect(result.success).toBe(true);
    expect(mockModuleService.deleteModule).toHaveBeenCalledWith(1);
  });
});
