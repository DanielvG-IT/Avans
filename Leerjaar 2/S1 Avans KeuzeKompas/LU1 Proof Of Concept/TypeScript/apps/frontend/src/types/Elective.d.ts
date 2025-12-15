import type { User } from "./User";

export interface Elective {
  id?: string;
  code: string;
  name: string;
  description: string;
  provider: string; // e.g. "Technische Bedrijfskunde"
  period: string; // e.g. "P3"
  duration: string; // e.g. "1 Periode"
  credits: number;
  language: string; // e.g. "Nederlands" or "English"
  location: string; // e.g. "Breda"
  level: string; // e.g. "NLQF5"
  tags?: string[]; // optional, for filtering/recommendations
  teachers?: User[]; // list of teachers teaching this elective
  createdAt?: string;
  updatedAt?: string;
}
