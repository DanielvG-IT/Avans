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

export const createInventory = (filmId, storeId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `INSERT INTO inventory (film_id, store_id) VALUES (?, ?)`;
        query(sql, [filmId, storeId], (error, rows) => {
            if (error) {
                logger.error('createInventory MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('createInventory sync error:', err);
        cb(err);
    }
};

export const readInventoryById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM inventory WHERE inventory_id = ? LIMIT 1`;
        query(sql, [id], (error, rows) => {
            if (error) {
                logger.error('readInventoryById MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows[0]);
        });
    } catch (err) {
        logger.error('readInventoryById sync error:', err);
        cb(err);
    }
};

export const readInventoryByFilmIdAtStore = (filmId, storeId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM inventory WHERE film_id = ? AND store_id = ?`;
        query(sql, [filmId, storeId], (error, rows) => {
            if (error) {
                logger.error('readInventoryByFilmId MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('readInventoryByFilmId sync error:', err);
        cb(err);
    }
};
