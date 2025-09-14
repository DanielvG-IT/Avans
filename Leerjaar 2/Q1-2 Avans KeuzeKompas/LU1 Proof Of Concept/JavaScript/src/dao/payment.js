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

export const createPayment = (customerId, staffId, rentalId, amount, paymentDate, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date) VALUES (?, ?, ?, ?, ?)`;
        query(sql, [customerId, staffId, rentalId, amount, paymentDate], (error, rows) => {
            if (error) {
                logger.error('createPayment MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {}
};

export const readPaymentById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM payment WHERE payment_id = ? LIMIT 1`;
        query(sql, [id], (error, rows) => {
            if (error) {
                logger.error('readPaymentById MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {}
};

export const readPaymentsByCustomerId = (customerId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM payment WHERE customer_id = ?`;
        query(sql, [customerId], (error, rows) => {
            if (error) {
                logger.error('readPaymentsByCustomerId MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {}
};

export const readPaymentsByStaffId = (staffId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM payment WHERE staff_id = ?`;
        query(sql, [staffId], (error, rows) => {
            if (error) {
                logger.error('readPaymentsByStaffId MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {}
};

export const readPaymentByRentalId = (rentalId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM payment WHERE rental_id = ?`;
        query(sql, [rentalId], (error, rows) => {
            if (error) {
                logger.error('readPaymentByRentalId MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {}
};
