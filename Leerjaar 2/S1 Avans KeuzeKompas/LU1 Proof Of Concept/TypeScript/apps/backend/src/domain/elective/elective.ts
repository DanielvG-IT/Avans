export interface Elective {
  id?: string;
  code: string;
  name: string;
  description: string;
  provider: string; // e.g. "Technische Bedrijfskunde"
  period: string; // e.g. "P3"
  duration: string; // e.g. "1 Periode"
  credits: number;
  language: string; // "Nederlands" or "Engels"
  location: string; // e.g. "Breda"
  level: string; // e.g. "NLQF5"
  tags?: string[]; // optional, for filtering/recommendations
  teachers?: string[]; // list of Teacher (User) _id's
  createdAt?: string;
  updatedAt?: string;
}
