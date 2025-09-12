import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

const mimeMap = {
    JPG: 'image/jpeg',
    JPEG: 'image/jpeg',
    PNG: 'image/png',
    WEBP: 'image/webp',
};

const makeDataUrl = (avatar, format) => {
    if (!avatar || !format) return null;
    // if it's already a data URI, return as-is
    if (typeof avatar === 'string' && avatar.startsWith('data:')) return avatar;

    const mime = mimeMap[(format || '').toUpperCase()] || 'application/octet-stream';

    // avatar may be stored as a Buffer or as a base64 string
    let base64;
    if (Buffer.isBuffer(avatar)) {
        base64 = avatar.toString('base64');
    } else {
        // strip any accidental data URI prefix if present and ensure it's a string
        base64 = String(avatar).replace(/^data:[^;]+;base64,/, '');
    }

    return `data:${mime};base64,${base64}`;
};

export const createUser = (userId, email, hashedPassword, role, avatar, avatarFormat, callback) => {
    const sql = `
        INSERT INTO users (userId, email, passwordHash, role, avatar, avatarFormat) VALUES (?,?,?,?,?,?);
    `;

    query(sql, [userId, email, hashedPassword, role, avatar, avatarFormat], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows);
    });
};

export const readUserById = (userId, callback) => {
    const sql = `SELECT userId, email, passwordHash, role, avatar, avatarFormat FROM users WHERE userId = ? LIMIT 1`;

    query(sql, [userId], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        const user = rows[0];
        if (user) user.avatar = makeDataUrl(user.avatar, user.avatarFormat);
        callback(null, user);
    });
};

export const readUserByEmail = (email, callback) => {
    const sql = `SELECT userId, email, passwordHash, role, avatar, avatarFormat FROM users WHERE email = ? LIMIT 1`;

    query(sql, [email], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        const user = rows[0];
        if (user) user.avatar = makeDataUrl(user.avatar, user.avatarFormat);
        callback(null, user);
    });
};

// Refresh Tokens
export const readUserByRefreshToken = (refreshToken, callback) => {
    const sql = `SELECT userId, email, passwordHash, role, avatar, avatarFormat FROM users WHERE refreshToken = ? LIMIT 1`;

    query(sql, [refreshToken], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        const user = rows[0];
        if (user) user.avatar = makeDataUrl(user.avatar, user.avatarFormat);
        callback(null, user);
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

export const updateUserAvatarById = (userId, avatarBase64, avatarFormat, callback) => {
    const sql = `UPDATE users SET avatar = ?, avatarFormat = ? WHERE userId = ?`;

    query(sql, [avatarBase64, avatarFormat, userId], (error, result) => {
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, result);
    });
};
