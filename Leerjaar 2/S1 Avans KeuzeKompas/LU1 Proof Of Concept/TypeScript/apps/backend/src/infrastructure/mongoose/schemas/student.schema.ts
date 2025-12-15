import { Prop, Schema, SchemaFactory } from "@nestjs/mongoose";
import { Types } from "mongoose";

@Schema()
export class StudentModel {
  @Prop({ type: [{ type: Types.ObjectId, ref: "Elective" }], default: [] })
  favorites: Types.ObjectId[];
}

export const StudentSchema = SchemaFactory.createForClass(StudentModel);
