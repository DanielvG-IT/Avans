import * as dotenv from "dotenv";

dotenv.config({ quiet: true });

const jwtSecret: string = process.env.JWT_SECRET || "";
if (!jwtSecret) {
  throw new Error("JWT_SECRET is not defined in environment variables");
}

const databaseUrl: string = process.env.DATABASE_URL || "";
if (!databaseUrl) {
  throw new Error("DATABASE_URL is not defined");
}

const nodeEnv = process.env.NODE_ENV || "development";
const logLevel = process.env.LOG_LEVEL || "not set";
const corsOrigin = process.env.CORS_ORIGIN || "*";

export { jwtSecret, databaseUrl, nodeEnv, logLevel, corsOrigin };
