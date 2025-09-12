import { query } from '../data/db.js';
import { logger } from '../util/logger.js';
import {
    normalizeId,
    normalizeEmail,
    normalizeRole,
    normalizeFormat,
    normalizeUserId,
    normalizeBase64,
} from '../util/normalize.js';

const mimeMap = {
    JPG: 'image/jpeg',
    JPEG: 'image/jpeg',
    PNG: 'image/png',
    WEBP: 'image/webp',
};

const makeDataUrl = (avatar, format) => {
    if (!avatar || !format) return null;
    if (typeof avatar === 'string' && avatar.startsWith('data:')) return avatar;

    const mime = mimeMap[normalizeFormat(format)] || 'application/octet-stream';
    const base64 = Buffer.isBuffer(avatar) ? avatar.toString('base64') : normalizeBase64(avatar);

    return `data:${mime};base64,${base64}`;
};

// --- Queries ---
export const createUser = (
    userId,
    emailId,
    hashedPassword,
    role,
    avatar,
    avatarFormat,
    callback
) => {
    const sql = `
        INSERT INTO user (userId, emailId, passwordHash, role, avatar, avatarFormat) VALUES (?,?,?,?,?,?);
    `;

    query(
        sql,
        [
            normalizeUserId(userId),
            normalizeId(emailId),
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
    const sql = `
        SELECT u.userId, e.email, u.passwordHash, u.role, u.avatar, u.avatarFormat 
        FROM user u 
        JOIN email e ON e.email_id = u.emailId
        WHERE u.userId = ? 
        LIMIT 1
    `;
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
    const sql = `
        SELECT u.userId, e.email, u.passwordHash, u.role, u.avatar, u.avatarFormat 
        FROM user u 
        JOIN email e ON e.email_id = u.emailId
        WHERE e.email = ? 
        LIMIT 1
    `;
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
    const sql = `
        SELECT u.userId, e.email, u.passwordHash, u.role, u.avatar, u.avatarFormat
        FROM user u
        JOIN email e ON e.email_id = u.emailId
        WHERE u.refreshToken = ? LIMIT 1
    `;
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
    const sql = `UPDATE user SET refreshToken = ? WHERE userId = ?`;

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
    const sql = `UPDATE user SET passwordHash = ? WHERE userId = ?`;

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
    const sql = `UPDATE user SET avatar = ?, avatarFormat = ? WHERE userId = ?`;

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
