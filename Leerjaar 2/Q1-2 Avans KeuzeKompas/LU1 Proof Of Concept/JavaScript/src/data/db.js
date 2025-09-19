import mysql from "mysql2";
import { logger } from "../util/logger.js";

if (
  !process.env.DB_HOST ||
  !process.env.DB_USER ||
  !process.env.DB_PASS ||
  !process.env.DB_NAME ||
  !process.env.DB_PORT
) {
  logger.fatal("Missing database configuration");
  throw new Error("Database configuration is incomplete");
}

const pool = mysql.createPool({
  host: process.env.DB_HOST,
  user: process.env.DB_USER,
  port: process.env.DB_PORT,
  password: process.env.DB_PASS,
  database: process.env.DB_NAME,
  waitForConnections: true,
  connectionLimit: Number(process.env.DB_CONNECTION_LIMIT) || 10,
});

export const query = (sql, params, callback) => {
  pool.query(sql, params, (err, results) => {
    if (err) return callback(err);
    callback(null, results);
  });
};
