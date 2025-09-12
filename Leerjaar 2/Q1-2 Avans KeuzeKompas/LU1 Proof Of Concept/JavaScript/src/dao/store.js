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

export const createStore = (name, addressId, managerStaffId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `INSERT INTO store (name, address_id, manager_staff_id) VALUES (?, ?, ?)`;
        query(sql, [name, normalizeId(addressId), normalizeId(managerStaffId)], (error, rows) => {
            if (error) {
                logger.error('createStore MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('createStore sync error:', err);
        cb(err);
    }
};

export const readStores = (callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM store`;
        query(sql, null, (error, rows) => {
            if (error) {
                logger.error('readStores MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('readStores sync error:', err);
        cb(err);
    }
};

export const readStoreById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM store WHERE id = ? LIMIT 1`;
        query(sql, [id], (error, rows) => {
            if (error) {
                logger.error('readStoreById MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('readStoreById sync error:', err);
        cb(err);
    }
};
