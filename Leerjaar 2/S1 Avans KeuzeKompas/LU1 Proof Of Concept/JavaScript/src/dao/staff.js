import { query } from '../data/db.js';
import { logger } from '../util/logger.js';
import { normalizeId } from '../util/normalize.js';

/**
 * Helper to ensure a callback is only called once.
 * Returns a wrapper around the original callback.
 */
const onceCallback = (cb) => {
    let called = false;
    return (err, res) => {
        if (called) return;
        called = true;
        try {
            if (typeof cb === 'function') cb(err, res);
        } catch (e) {
            // In case the caller throws inside their callback, log it but don't crash
            logger.error('Callback threw an error:', e);
        }
    };
};

export const readStaff = (callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM staff`;
        query(sql, null, (error, rows) => {
            if (error) {
                logger.error('readStaff MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('readStaff sync error:', err);
        cb(err);
    }
};

export const readStaffById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM staff WHERE id = ? LIMIT 1`;
        query(sql, [normalizeId(id)], (error, rows) => {
            if (error) {
                logger.error('readStaffById MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows[0] || null);
        });
    } catch (err) {
        logger.error('readStaffById sync error:', err);
        cb(err);
    }
};

export const readStaffByUserId = (userId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM staff WHERE userId = ? LIMIT 1`;
        query(sql, [userId], (error, rows) => {
            if (error) {
                logger.error('readStaffByUserId MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows[0] || null);
        });
    } catch (err) {
        logger.error('readStaffByUserId sync error:', err);
        cb(err);
    }
};
