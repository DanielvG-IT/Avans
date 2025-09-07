import dotenv from 'dotenv';
dotenv.config();

import mysql from 'mysql2';

if (!process.env.DB_HOST || !process.env.DB_USER || !process.env.DB_PASS || !process.env.DB_NAME) {
    throw new Error('Missing database configuration');
}

const pool = mysql.createPool({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
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
