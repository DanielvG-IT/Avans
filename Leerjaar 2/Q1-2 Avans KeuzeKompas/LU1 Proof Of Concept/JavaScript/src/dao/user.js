import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

const mimeMap = {
    JPG: 'image/jpeg',
    JPEG: 'image/jpeg',
    PNG: 'image/png',
    WEBP: 'image/webp',
};

// --- Normalizers ---
const normalizeEmail = (email) =>
    String(email || '')
        .trim()
        .toLowerCase();
const normalizeRole = (role) =>
    String(role || '')
        .trim()
        .toUpperCase();
const normalizeFormat = (format) =>
    String(format || '')
        .trim()
        .toUpperCase();
const normalizeUserId = (id) => String(id || '').trim();
const normalizeBase64 = (input) =>
    String(input || '')
        .replace(/^data:[^;]+;base64,/, '')
        .trim();

const makeDataUrl = (avatar, format) => {
    if (!avatar || !format) return null;
    if (typeof avatar === 'string' && avatar.startsWith('data:')) return avatar;

    const mime = mimeMap[normalizeFormat(format)] || 'application/octet-stream';
    const base64 = Buffer.isBuffer(avatar) ? avatar.toString('base64') : normalizeBase64(avatar);

    return `data:${mime};base64,${base64}`;
};

// --- Queries ---
export const createUser = (userId, email, hashedPassword, role, avatar, avatarFormat, callback) => {
    const sql = `
        INSERT INTO users (userId, email, passwordHash, role, avatar, avatarFormat) VALUES (?,?,?,?,?,?);
    `;

    query(
        sql,
        [
            normalizeUserId(userId),
            normalizeEmail(email),
            hashedPassword,
            normalizeRole(role),
            avatar ? normalizeBase64(avatar) : null,
            avatar ? normalizeFormat(avatarFormat) : null,
        ],
        (error, rows) => {
            if (typeof callback !== 'function') return;
            if (error) {
                logger.error('MySQL Error:', error);
                return callback(error);
            }
            callback(null, rows);
        }
    );
};

export const readUserById = (userId, callback) => {
    const sql = `SELECT userId, email, passwordHash, role, avatar, avatarFormat FROM users WHERE userId = ? LIMIT 1`;

    query(sql, [normalizeUserId(userId)], (error, rows) => {
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

    query(sql, [normalizeEmail(email)], (error, rows) => {
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

export const updateUserRefreshTokenById = (userId, refreshToken, callback) => {
    const sql = `UPDATE users SET refreshToken = ? WHERE userId = ?`;

    query(sql, [refreshToken, normalizeUserId(userId)], (error, rows) => {
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

    query(sql, [newPasswordHash, normalizeUserId(userId)], (error, rows) => {
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

    query(
        sql,
        [normalizeBase64(avatarBase64), normalizeFormat(avatarFormat), normalizeUserId(userId)],
        (error, result) => {
            if (error) {
                logger.error('MySQL Error:', error);
                return callback(error);
            }
            callback(null, result);
        }
    );
};

export const deleteUserById = (userId, callback) => {
    logger.warn;
    ('deleteUserById is not implemented yet.');
};
