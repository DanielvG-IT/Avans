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

export const readLanguages = (callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM language`;
        query(sql, (error, rows) => {
            if (error) {
                logger.error('readLanguages MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {}
};
