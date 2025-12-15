import { Schema, SchemaFactory } from "@nestjs/mongoose";

@Schema()
export class TeacherModel {
  // Teacher-specific fields can be added here in the future if needed
}

export const TeacherSchema = SchemaFactory.createForClass(TeacherModel);
