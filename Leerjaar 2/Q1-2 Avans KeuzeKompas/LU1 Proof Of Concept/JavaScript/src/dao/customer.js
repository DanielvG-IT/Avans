import { query } from '../data/db.js';
import { logger } from '../util/logger.js';
import { normalizeId, normalizeUserId, normalizeName } from '../util/normalize.js';

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

export const createCustomer = (
    firstName,
    lastName,
    addressId,
    userId,
    emailId,
    storeId,
    callback
) => {
    const cb = onceCallback(callback);
    try {
        const sql = `
            INSERT INTO customer (store_id, first_name, last_name, address_id, create_date, userId, email_id) VALUES (?,?,?,?,?,?,?)    
        `;
        query(
            sql,
            [
                normalizeId(storeId),
                normalizeName(firstName),
                normalizeName(lastName),
                normalizeId(addressId),
                new Date(),
                normalizeUserId(userId),
                normalizeId(emailId),
            ],
            (error, result) => {
                if (error) {
                    logger.error('createCustomer MySQL Error:', error);
                    return cb(error);
                }
                cb(null, result);
            }
        );
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
        query(sql, [normalizeId(customerId)], (error, rows) => {
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

export const readCustomerByEmailId = (emailId, callback) => {
    const sql = `
        SELECT * FROM customer
        WHERE email_id = ? LIMIT 1
    `;
    query(sql, [normalizeId(emailId)], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('readCustomerByEmailId MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows[0] || null);
    });
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

        query(sql, [normalizeUserId(userId)], (error, rows) => {
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

export const updateCustomer = (customerId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = ``;
        query(sql, [normalizeId(customerId)], (error, result) => {
            if (error) {
                logger.error('updateCustomer MySQL Error:', error);
                return cb(error);
            }
            cb(null, result);
        });
    } catch (err) {
        logger.error('updateCustomer sync error:', err);
        cb(err);
    }
};

export const softDeleteCustomer = (customerId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `UPDATE customer SET active = 0 WHERE customer_id = ?`;
        query(sql, [normalizeId(customerId)], (error, result) => {
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

export const unlinkCustomerFromUser = (customerId, callback) => {
    const sql = `UPDATE customer SET userId = NULL WHERE customer_id = ?`;
    query(sql, [normalizeId(customerId)], (error, result) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('unlinkCustomerFromUser MySQL Error:', error);
            return callback(error);
        }
        callback(null, result);
    });
};

export const linkCustomerToUser = (customerId, userId, callback) => {
    const sql = `UPDATE customer SET userId = ? WHERE customer_id = ?`;
    query(sql, [normalizeUserId(userId), normalizeId(customerId)], (error, result) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('linkCustomerToUser MySQL Error:', error);
            return callback(error);
        }
        callback(null, result);
    });
};
