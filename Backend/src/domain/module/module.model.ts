export interface ModuleDetail {
  id: number;
  name: string;
  shortDescription: string;
  description: string;
  content: string;
  level: string;
  studyCredits: number;
  location: { id: number; name: string }[];
  moduleTags: { id: number; name: string }[];
  learningOutcomes: string;
  availableSpots: number;
  startDate: string;
}
export interface Module {
  id: number;
  name: string;
  shortDescription: string;
  studyCredits: number;
  level: string;
  location: { id: number; name: string }[];
  startDate: string;
}
export interface CreateModule {
  name: string;
  description: string;
  content: string;
  level: string;
  studyCredits: number;
  location: { id: number; name: string }[];
  moduleTags: { id: number; name: string }[];
  learningOutcomes: string;
  availableSpots: number;
  startDate: string;
  shortDescription: string;
}

export type UpdateModule = CreateModule;
