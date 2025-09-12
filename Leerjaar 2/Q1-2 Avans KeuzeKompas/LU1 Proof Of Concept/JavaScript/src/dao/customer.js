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

export const createCustomer = (callback) => {
    const cb = onceCallback(callback);
    try {
        logger.warn('createCustomer is not implemented yet.');
        cb(null, null);
    } catch (err) {
        logger.error('createCustomer MySql error:', err);
        cb(err);
    }
};

export const readCustomerById = (customerId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `
          SELECT
              c.customer_id,
              c.first_name,
              c.last_name,
              a.address,
              a.postal_code,
              a.phone,
              ci.city,
              co.country,
              c.active,
              c.store_id
          FROM customer c
          JOIN address a ON c.address_id = a.address_id
          JOIN city ci ON a.city_id = ci.city_id
          JOIN country co ON ci.country_id = co.country_id
          WHERE c.customer_id = ?
      `;
        query(sql, [customerId], (error, rows) => {
            if (error) {
                logger.error('getCustomerById MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows[0] || null);
        });
    } catch (err) {
        logger.error('getCustomerById sync error:', err);
        cb(err);
    }
};

export const readCustomerByUserId = (userId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `
          SELECT
              c.customer_id,
              c.first_name,
              c.last_name,
              a.address,
              a.postal_code,
              a.phone,
              ci.city,
              co.country,
              c.active,
              c.store_id
          FROM customer c
          JOIN address a ON c.address_id = a.address_id
          JOIN city ci ON a.city_id = ci.city_id
          JOIN country co ON ci.country_id = co.country_id
          WHERE c.userId = ?
      `;

        query(sql, [userId], (error, rows) => {
            if (error) {
                logger.error('getCustomerByUserId MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows[0] || null);
        });
    } catch (err) {
        logger.error('getCustomerByUserId sync error:', err);
        cb(err);
    }
};

export const updateCustomer = (callback) => {
    logger.warn('updateCustomer is not implemented yet.');
};

export const deleteCustomer = (customerId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `UPDATE customer SET active = 0 WHERE customer_id = ?`;
        query(sql, [customerId], (error, result) => {
            if (error) {
                logger.error('deleteCustomer MySQL Error:', error);
                return cb(error);
            }
            cb(null, result);
        });
    } catch (err) {
        logger.error('deleteCustomer sync error:', err);
        cb(err);
    }
};
