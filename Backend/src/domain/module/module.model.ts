export interface moduleDetail {
  id: number;
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
}
export interface Module {
  id: number;
  name: string;
  shortdescription: string;
  studyCredits: number;
  level: string;
  location: { id: number; name: string }[];
  startDate: string;
}
export interface createModule {
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
  shortdescription: string;
}
