import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

export const createUser = (userId, email, hashedPassword, role, callback) => {
    const sql = `
        INSERT INTO users (userId, email, passwordHash, role) VALUES (?,?,?,?);
    `;

    query(sql, [userId, email, hashedPassword, role], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows);
    });
};

export const readUserById = (userId, callback) => {
    const sql = `SELECT userId, email, passwordHash, role, avatarUrl FROM users WHERE userId = ? LIMIT 1`;

    query(sql, [userId], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows[0]);
    });
};

export const readUserByEmail = (email, callback) => {
    const sql = `SELECT userId, email, passwordHash, role, avatarUrl FROM users WHERE email = ? LIMIT 1`;

    query(sql, [email], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows[0]);
    });
};

// Refresh Tokens
export const readUserByRefreshToken = (refreshToken, callback) => {
    const sql = `SELECT userId, email, passwordHash, role, avatarUrl FROM users WHERE refreshToken = ? LIMIT 1`;

    query(sql, [refreshToken], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows[0]);
    });
};

export const updateUserRefreshTokenByUserId = (userId, refreshToken, callback) => {
    const sql = `UPDATE users SET refreshToken = ? WHERE userId = ?`;

    query(sql, [refreshToken, userId], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows);
    });
};

export const updateUserPasswordById = (userId, newPasswordHash, callback) => {
    const sql = `UPDATE users SET passwordHash = ? WHERE userId = ?`;

    query(sql, [newPasswordHash, userId], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows);
    });
};
