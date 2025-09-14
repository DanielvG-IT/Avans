import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

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

export const createEmail = (email, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `INSERT INTO email (email) VALUES (?)`;
        query(sql, [email], (error, rows) => {
            if (error) {
                logger.error('createEmail MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('createEmail sync error:', err);
        cb(err);
    }
};

export const readEmail = (email, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM email WHERE email = ? LIMIT 1`;
        query(sql, [email], (error, rows) => {
            if (error) {
                logger.error('readEmail MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows[0] || null);
        });
    } catch (err) {
        logger.error('readEmail sync error:', err);
        cb(err);
    }
};

export const readEmailById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM email WHERE id = ? LIMIT 1`;
        query(sql, [id], (error, rows) => {
            if (error) {
                logger.error('readEmailById MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('readEmailById sync error:', err);
        cb(err);
    }
};

export const updateEmailById = (id, newEmail, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `UPDATE email SET email = ? WHERE id = ?`;
        query(sql, [newEmail, id], (error, rows) => {
            if (error) {
                logger.error('updateEmailById MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('updateEmailById sync error:', err);
        cb(err);
    }
};
