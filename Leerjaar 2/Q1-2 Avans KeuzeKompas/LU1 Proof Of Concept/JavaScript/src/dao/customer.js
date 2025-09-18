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
        const email = normalizeId(emailId);
        if (!email) {
            const err = new Error('Invalid emailId');
            logger.error('createCustomer validation error:', err);
            return cb(err);
        }

        // Prevent creating a customer when the email_id is already linked to another customer
        const checkSql = `SELECT customer_id FROM customer WHERE email_id = ? LIMIT 1`;
        query(checkSql, [email], (checkError, checkRows) => {
            if (checkError) {
                logger.error('createCustomer MySQL Error (check):', checkError);
                return cb(checkError);
            }
            if (Array.isArray(checkRows) && checkRows[0]) {
                const dupErr = new Error('Email is already associated with another customer');
                dupErr.code = 'ER_DUP_ENTRY_EMAIL';
                logger.error('createCustomer Error: Email already in use for email_id:', email);
                return cb(dupErr);
            }

            const sql = `
                INSERT INTO customer (store_id, first_name, last_name, address_id, create_date, userId, email_id) VALUES (?,?,?,?,?,?,?)    
            `;
            const normalizedUserId = normalizeUserId(userId) || null;
            query(
                sql,
                [
                    normalizeId(storeId),
                    normalizeName(firstName),
                    normalizeName(lastName),
                    normalizeId(addressId),
                    new Date(),
                    normalizedUserId,
                    email,
                ],
                (error, result) => {
                    if (error) {
                        logger.error('createCustomer MySQL Error:', error);
                        return cb(error);
                    }
                    cb(null, result);
                }
            );
        });
    } catch (err) {
        logger.error('createCustomer MySql error:', err);
        cb(err);
    }
};

export const readCustomers = (filterParams, callback) => {
    // Backwards compatible: if only a callback is provided
    const cb = onceCallback(typeof filterParams === 'function' ? filterParams : callback);
    const params = typeof filterParams === 'function' ? {} : filterParams || {};

    try {
        const {
            search = '',
            active = '1',
            storeId = null,
            sort = 'createDate,asc',
            page = 1, // new
            pageSize = 30, // default 30 per page
        } = params;

        // map allowed sort keys to actual column names to avoid SQL injection
        const sortFields = {
            createDate: 'create_date',
            lastName: 'last_name',
        };

        const [sortKey, sortDirRaw] = (sort || '').split(',');
        const sortColumn = sortFields[sortKey] || 'create_date';
        const sortDir = String(sortDirRaw || 'asc').toLowerCase() === 'desc' ? 'DESC' : 'ASC';

        let baseSql = `
                FROM customer c 
                JOIN store s ON c.store_id = s.store_id
                JOIN email e ON c.email_id = e.email_id 
                JOIN address a ON c.address_id = a.address_id 
                JOIN city ci ON a.city_id = ci.city_id 
                JOIN country co ON ci.country_id = co.country_id
            `;

        const values = [];
        const whereClauses = [];

        if (search && String(search).trim() !== '') {
            const s = `%${String(search).trim()}%`;
            whereClauses.push(
                '(first_name LIKE ? OR last_name LIKE ? OR email LIKE ? OR address LIKE ? OR city LIKE ? OR country LIKE ?)'
            );
            values.push(s, s, s, s, s, s);
        }

        if (active !== 'all') {
            // treat any non-'0' value as active (1)
            const activeVal = String(active) === '0' ? 0 : 1;
            whereClauses.push('active = ?');
            values.push(activeVal);
        }

        if (storeId !== null && storeId !== undefined && storeId !== '') {
            whereClauses.push('c.store_id = ?');
            values.push(normalizeId(storeId));
        }

        if (whereClauses.length) {
            baseSql += ' WHERE ' + whereClauses.join(' AND ');
        }

        // NOTE: Do not append ORDER BY to baseSql, otherwise the COUNT(*) query may become malformed
        // Pagination calc
        const limit = Number(pageSize) > 0 ? Number(pageSize) : 20;
        const offset = (Number(page) - 1) * limit;

        // Count query for total rows (no ORDER BY / LIMIT)
        const countSql = `SELECT COUNT(*) AS total ${baseSql}`;

        // Data query - ORDER BY and LIMIT/OFFSET applied here only
        const dataSql = `
            SELECT 
                c.customer_id, s.name AS store_name, c.first_name, c.last_name, 
                e.email, a.address, a.district, a.postal_code, a.phone, ci.city, co.country, 
                c.active, c.create_date, c.last_update, c.userId
            ${baseSql}
            ORDER BY ${sortColumn} ${sortDir}
            LIMIT ? OFFSET ?
        `;
        query(countSql, values, (countError, countRows) => {
            if (countError) {
                logger.error('readCustomers Count MySQL Error:', countError);
                return cb(countError);
            }

            const total =
                Array.isArray(countRows) &&
                countRows[0] &&
                typeof countRows[0].total !== 'undefined'
                    ? Number(countRows[0].total)
                    : 0;

            query(dataSql, [...values, limit, offset], (error, rows) => {
                if (error) {
                    logger.error('readCustomers MySQL Error:', error);
                    return cb(error);
                }
                cb(null, {
                    total,
                    page: Number(page),
                    pageSize: limit,
                    totalPages: Math.ceil(total / limit),
                    customers: rows,
                });
            });
        });
    } catch (err) {
        logger.error('readCustomers sync error:', err);
        cb(err);
    }
};

export const readCustomerById = (customerId, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `
          SELECT
              c.customer_id,
              e.email,
              c.first_name,
              c.last_name,
              a.address,
              a.district,
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
          JOIN email e ON c.email_id = e.email_id
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
              e.email,
              c.first_name,
              c.last_name,
              a.address,
              a.district,
              a.postal_code,
              a.phone,
              ci.city,
              co.country,
              c.active,
              c.store_id
          FROM customer c
          JOIN address a ON c.address_id = a.address_id
          JOIN email e ON c.email_id = e.email_id
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

export const updateCustomer = (customerId, customerData, callback) => {
    const cb = onceCallback(callback);
    try {
        if (!customerData || typeof customerData !== 'object') {
            logger.warn('updateCustomer called with no customerData');
            return cb(new Error('No customer data provided'));
        }

        const fields = [];
        const values = [];

        if (customerData.firstName !== undefined) {
            fields.push('first_name = ?');
            values.push(normalizeName(customerData.firstName));
        }
        if (customerData.lastName !== undefined) {
            fields.push('last_name = ?');
            values.push(normalizeName(customerData.lastName));
        }
        if (customerData.addressId !== undefined) {
            fields.push('address_id = ?');
            values.push(normalizeId(customerData.addressId));
        }
        if (customerData.active !== undefined) {
            fields.push('active = ?');
            values.push(customerData.active ? 1 : 0);
        }
        if (customerData.emailId !== undefined) {
            fields.push('email_id = ?');
            values.push(normalizeId(customerData.emailId));
        }

        if (!fields.length) {
            logger.warn('updateCustomer called with no fields to update');
            return cb(new Error('No fields to update'));
        }

        // Always update last_update timestamp
        fields.push('last_update = NOW()');

        const sql = `UPDATE customer SET ${fields.join(', ')} WHERE customer_id = ?`;
        values.push(normalizeId(customerId));

        logger.debug('updateCustomer SQL', { sql, values });

        query(sql, values, (error, result) => {
            if (error) {
                logger.error('updateCustomer MySQL Error:', error);
                return cb(error);
            }
            logger.debug('updateCustomer result', { result });
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
