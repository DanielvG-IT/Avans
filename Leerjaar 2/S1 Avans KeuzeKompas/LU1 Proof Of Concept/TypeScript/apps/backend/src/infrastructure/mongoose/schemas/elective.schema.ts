import { Prop, Schema, SchemaFactory } from "@nestjs/mongoose";
import { HydratedDocument, Types } from "mongoose";

export type ElectiveDocument = HydratedDocument<ElectiveModel>;

@Schema({ timestamps: { createdAt: true, updatedAt: true } })
export class ElectiveModel {
  @Prop({ required: true })
  code: string;

  @Prop({ required: true })
  name: string;

  @Prop({ required: true })
  description: string;

  @Prop({ required: true })
  provider: string;

  @Prop({ required: true })
  period: string;

  @Prop({ required: true })
  duration: string;

  @Prop({ required: true })
  credits: number;

  @Prop({ required: true })
  language: string;

  @Prop({ required: true })
  location: string;

  @Prop({ required: true })
  level: string;

  @Prop({ type: [String], required: false })
  tags: string[];

  @Prop({ type: [{ type: Types.ObjectId, ref: "User" }], default: [] })
  teachers: Types.ObjectId[];
}

export const ElectiveSchema = SchemaFactory.createForClass(ElectiveModel);
