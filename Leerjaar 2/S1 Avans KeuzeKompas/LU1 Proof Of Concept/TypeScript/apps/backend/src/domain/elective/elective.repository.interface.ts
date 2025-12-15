import { Elective } from "./elective";

export interface IElectiveRepository {
  find(): Promise<Elective[]>;
  findById(id: string): Promise<Elective | null>;
  findByTeacherId(teacherId: string): Promise<Elective[]>;
  create(data: Elective): Promise<Elective>;
  update(id: string, data: Elective | Partial<Elective>): Promise<Elective | null>;
  delete(id: string): Promise<boolean>;
  isElectiveInUse(id: string): Promise<boolean>;
}
