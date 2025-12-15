import { Schema } from "mongoose";

// Admin schema (same as base, no additional fields)
export const AdminSchema = new Schema({
  // Inherits all fields from UserSchema
  // No additional fields needed for admin
});
