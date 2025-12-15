import { IUserRepository } from "../../../domain/user/user.repository.interface";
import { UserModel, type UserDocument } from "../schemas/user.schema";
import { StudentSchema } from "../schemas/student.schema";
import { TeacherSchema } from "../schemas/teacher.schema";
import { AdminSchema } from "../schemas/admin.schema";
import { User, StudentUser, TeacherUser, AdminUser } from "../../../domain/user/user";
import { InjectModel } from "@nestjs/mongoose";
import { Injectable, OnModuleInit } from "@nestjs/common";
import { Model, Types } from "mongoose";

@Injectable()
export class MongooseUserRepository implements IUserRepository, OnModuleInit {
  private studentModel: Model<any>;
  private teacherModel: Model<any>;
  private adminModel: Model<any>;

  constructor(@InjectModel("User") private readonly model: Model<UserDocument>) {}

  onModuleInit() {
    // Register discriminators after module initialization
    // Check if discriminators already exist to avoid re-registration errors
    if (!this.model.discriminators || !this.model.discriminators["student"]) {
      this.studentModel = this.model.discriminator("student", StudentSchema);
    } else {
      this.studentModel = this.model.discriminators["student"];
    }

    if (!this.model.discriminators || !this.model.discriminators["teacher"]) {
      this.teacherModel = this.model.discriminator("teacher", TeacherSchema);
    } else {
      this.teacherModel = this.model.discriminators["teacher"];
    }

    if (!this.model.discriminators || !this.model.discriminators["admin"]) {
      this.adminModel = this.model.discriminator("admin", AdminSchema);
    } else {
      this.adminModel = this.model.discriminators["admin"];
    }
  }

  public async find(): Promise<User[]> {
    const docs = (await this.model.find().lean().exec()) as Array<
      UserModel & { _id?: Types.ObjectId | string }
    >;
    return docs.map((d) => this.toDomain(d));
  }

  public async findById(id: string): Promise<User | null> {
    if (!id || !Types.ObjectId.isValid(id)) return null;
    const doc = (await this.model.findById(id).lean().exec()) as
      | (UserModel & { _id?: Types.ObjectId | string })
      | null;
    return doc ? this.toDomain(doc) : null;
  }

  public async findByEmail(email: string): Promise<User | null> {
    if (!email) return null;
    const doc = (await this.model.findOne({ email }).lean().exec()) as
      | (UserModel & { _id?: Types.ObjectId | string })
      | null;
    return doc ? this.toDomain(doc) : null;
  }

  public async create(data: User): Promise<User> {
    const created = await this.model.create(data as UserModel);
    const doc = (await this.model.findById(created._id).lean().exec()) as
      | (UserModel & { _id?: Types.ObjectId | string })
      | null;
    if (!doc) throw new Error("Failed to read created user");
    return this.toDomain(doc);
  }

  public async update(id: string, data: Partial<User> | User): Promise<User | null> {
    if (!Types.ObjectId.isValid(id)) return null;

    // Determine which model to use based on the role or existing document
    let modelToUse = this.model;
    if ("role" in data && data.role) {
      if (data.role === "student") modelToUse = this.studentModel;
      else if (data.role === "teacher") modelToUse = this.teacherModel;
      else if (data.role === "admin") modelToUse = this.adminModel;
    } else {
      // If role not in data, fetch existing document to determine type
      const existing = await this.model.findById(id).lean().exec();
      if (existing) {
        if (existing.role === "student") modelToUse = this.studentModel;
        else if (existing.role === "teacher") modelToUse = this.teacherModel;
        else if (existing.role === "admin") modelToUse = this.adminModel;
      }
    }

    const updated = (await modelToUse
      .findByIdAndUpdate(id, data as Partial<UserModel>, {
        new: true,
        runValidators: true,
      })
      .lean()
      .exec()) as (UserModel & { _id?: Types.ObjectId | string }) | null;
    return updated ? this.toDomain(updated) : null;
  }

  public async delete(id: string): Promise<boolean> {
    if (!Types.ObjectId.isValid(id)) return false;
    const removed = await this.model.findByIdAndDelete(id).exec();
    return removed != null;
  }

  // Role-specific finders
  public async findStudents(): Promise<StudentUser[]> {
    const docs = (await this.model.find({ role: "student" }).lean().exec()) as Array<
      UserModel & { _id?: Types.ObjectId | string }
    >;
    return docs.map((d) => this.toDomain(d) as StudentUser);
  }

  public async findTeachers(): Promise<TeacherUser[]> {
    const docs = (await this.model.find({ role: "teacher" }).lean().exec()) as Array<
      UserModel & { _id?: Types.ObjectId | string }
    >;
    return docs.map((d) => this.toDomain(d) as TeacherUser);
  }

  public async findAdmins(): Promise<AdminUser[]> {
    const docs = (await this.model.find({ role: "admin" }).lean().exec()) as Array<
      UserModel & { _id?: Types.ObjectId | string }
    >;
    return docs.map((d) => this.toDomain(d) as AdminUser);
  }

  public async findStudentById(id: string): Promise<StudentUser | null> {
    if (!id || !Types.ObjectId.isValid(id)) return null;
    const doc = (await this.model.findOne({ _id: id, role: "student" }).lean().exec()) as
      | (UserModel & { _id?: Types.ObjectId | string })
      | null;
    return doc ? (this.toDomain(doc) as StudentUser) : null;
  }

  public async findTeacherById(id: string): Promise<TeacherUser | null> {
    if (!id || !Types.ObjectId.isValid(id)) return null;
    const doc = (await this.model.findOne({ _id: id, role: "teacher" }).lean().exec()) as
      | (UserModel & { _id?: Types.ObjectId | string })
      | null;
    return doc ? (this.toDomain(doc) as TeacherUser) : null;
  }

  public async findAdminById(id: string): Promise<AdminUser | null> {
    if (!id || !Types.ObjectId.isValid(id)) return null;
    const doc = (await this.model.findOne({ _id: id, role: "admin" }).lean().exec()) as
      | (UserModel & { _id?: Types.ObjectId | string })
      | null;
    return doc ? (this.toDomain(doc) as AdminUser) : null;
  }

  private toDomain(doc: UserModel & { _id?: Types.ObjectId | string }): User {
    const { _id, ...rest } = doc ?? ({} as UserModel);
    const id = _id ? String(_id) : undefined;

    // Return based on role to properly type the discriminated union
    const baseUser = { ...rest, id };
    return baseUser as User;
  }
}
