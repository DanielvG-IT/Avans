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

export const createRental = (
    rentalDate,
    inventoryId,
    customerId,
    returnDate,
    staffId,
    callback
) => {
    const cb = onceCallback(callback);
    try {
        const sql = `INSERT INTO rental (rental_date, inventory_id, customer_id, return_date, staff_id) VALUES (?, ?, ?, ?, ?)`;
        query(sql, [rentalDate, inventoryId, customerId, returnDate, staffId], (error, rows) => {
            if (error) {
                logger.error('createRental MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('createRental sync error:', err);
        cb(err);
    }
};

export const readRentalById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM rental WHERE rental_id = ? LIMIT 1`;
        query(sql, [id], (error, rows) => {
            if (error) {
                logger.error('readRentalById MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('readRentalById sync error:', err);
        cb(err);
    }
};

export const readRentalsByCustomerId = (customerId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM rental WHERE customer_id = ?`;
        query(sql, [customerId], (error, rows) => {
            if (error) {
                logger.error('readRentalsByCustomerId MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('readRentalsByCustomerId sync error:', err);
        cb(err);
    }
};

export const readRentalsByInventoryId = (inventoryId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT * FROM rental WHERE inventory_id = ?`;
        query(sql, [inventoryId], (error, rows) => {
            if (error) {
                logger.error('readRentalsByInventoryId MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('readRentalsByInventoryId sync error:', err);
        cb(err);
    }
};

export const readCustomerRentalHistory = (customerId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `
            SELECT 
                r.rental_id,
                r.rental_date,
                r.return_date,
                
                f.film_id,
                f.title,
                f.description,
                f.release_year,
                f.length,
                f.rating,
                f.rental_duration,
                f.rental_rate,
                f.replacement_cost,
                
                p.amount,
                p.payment_date,
                
                s.staff_id,
                s.first_name AS staff_first_name,
                s.last_name AS staff_last_name,
                
                CASE 
                WHEN r.return_date IS NULL THEN 'Active'
                WHEN r.return_date > NOW() THEN 'Future'
                ELSE 'Past'
                END AS rental_status

            FROM rental r
            JOIN inventory i ON i.inventory_id = r.inventory_id
            JOIN film f ON f.film_id = i.film_id
            JOIN staff s ON s.staff_id = r.staff_id
            LEFT JOIN payment p ON p.rental_id = r.rental_id
            WHERE r.customer_id = ?
            ORDER BY r.rental_date DESC
        `;

        query(sql, [customerId], (error, rows) => {
            if (error) {
                logger.error('readCustomerRentalHistory MySQL Error:', error);
                return cb(error);
            }
            return cb(null, rows);
        });
    } catch (err) {
        logger.error('readCustomerRentalHistory sync error:', err);
        return cb(err);
    }
};
