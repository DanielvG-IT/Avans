import { Prop, Schema, SchemaFactory } from "@nestjs/mongoose";
import { HydratedDocument, Model } from "mongoose";

export type UserDocument = HydratedDocument<UserModel>;

@Schema({
  timestamps: { createdAt: true, updatedAt: true },
  discriminatorKey: "role",
  collection: "users", // Explicit collection name
})
export class UserModel {
  @Prop({ required: true, trim: true, minlength: 1, maxlength: 100 })
  firstName: string;

  @Prop({ required: true, trim: true, minlength: 1, maxlength: 100 })
  lastName: string;

  @Prop({
    required: true,
    unique: true,
    lowercase: true,
    trim: true,
    match: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  })
  email: string;

  @Prop({
    required: true,
    enum: ["student", "teacher", "admin"],
  })
  role: "student" | "teacher" | "admin";

  @Prop({ required: true })
  passwordHash: string;
}

export const UserSchema = SchemaFactory.createForClass(UserModel);

// Type for discriminator models
export type UserModelType = Model<UserDocument>;
