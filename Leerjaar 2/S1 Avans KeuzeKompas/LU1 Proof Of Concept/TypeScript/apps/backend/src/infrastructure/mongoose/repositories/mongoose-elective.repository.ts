import { Model, Types } from "mongoose";
import { Injectable } from "@nestjs/common";
import { InjectModel } from "@nestjs/mongoose";
import { Elective } from "src/domain/elective/elective";
import { ElectiveModel, type ElectiveDocument } from "../schemas/elective.schema";
import { IElectiveRepository } from "src/domain/elective/elective.repository.interface";

/**
 * A Mongoose-backed implementation of IElectiveRepository.
 * Expects a Mongoose Model<Elective & Document> to be injected.
 */
@Injectable()
export class MongooseElectiveRepository implements IElectiveRepository {
  constructor(@InjectModel("Elective") private readonly model: Model<ElectiveDocument>) {}

  public async find(): Promise<Elective[]> {
    const docs = await this.model.find().lean().exec();
    return docs.map((d) => this.toDomain(d));
  }

  public async findById(id: string): Promise<Elective | null> {
    if (!Types.ObjectId.isValid(id)) return null;
    const doc = await this.model.findById(id).lean().exec();
    return doc ? this.toDomain(doc) : null;
  }

  public async findByTeacherId(teacherId: string): Promise<Elective[]> {
    if (!Types.ObjectId.isValid(teacherId)) return [];
    const teacherObjectId = new Types.ObjectId(teacherId);
    const docs = await this.model.find({ teachers: teacherObjectId }).lean().exec();
    return docs.map((d) => this.toDomain(d));
  }

  public async create(data: Elective): Promise<Elective> {
    // Convert string teachers to ObjectIds
    const modelData = {
      ...data,
      teachers: data.teachers?.map((id) => new Types.ObjectId(id)) || [],
    };
    const created = await this.model.create(modelData as ElectiveModel);
    // fetch lean doc to normalize
    const doc = (await this.model.findById(created._id).lean().exec()) as ElectiveModel;
    return this.toDomain(doc);
  }

  public async update(id: string, data: Elective | Partial<Elective>): Promise<Elective | null> {
    if (!Types.ObjectId.isValid(id)) return null;
    // Convert string teachers to ObjectIds if present
    const modelData = {
      ...data,
      ...(data.teachers && { teachers: data.teachers.map((id) => new Types.ObjectId(id)) }),
    };
    const updated = await this.model
      .findByIdAndUpdate(id, modelData as Partial<ElectiveModel>, {
        new: true,
        runValidators: true,
      })
      .lean()
      .exec();
    return updated ? this.toDomain(updated) : null;
  }

  public async delete(id: string): Promise<boolean> {
    if (!Types.ObjectId.isValid(id)) return false;
    const res = await this.model.findByIdAndDelete(id).exec();
    return res !== null;
  }

  private toDomain(doc: ElectiveModel & { _id?: Types.ObjectId | string }): Elective {
    const { _id, teachers, ...rest } = doc ?? ({} as ElectiveModel);
    return {
      ...(rest as Omit<Elective, "id" | "teachers">),
      id: _id ? String(_id) : undefined,
      teachers: teachers ? teachers.map((t) => String(t)) : undefined,
    };
  }

  public async isElectiveInUse(id: string): Promise<boolean> {
    if (!Types.ObjectId.isValid(id)) return false;

    const objectId = new Types.ObjectId(id);
    const studentCount = await this.model.db
      .collection("students")
      .countDocuments({ favorites: objectId });

    return studentCount > 0;
  }
}
