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

export const createAddress = (address, district, cityId, postalCode, phone, location, callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `
            INSERT INTO address (address, district, city_id, postal_code, phone, location)
            VALUES (?, ?, ?, ?, ?, ?)
        `;
        query(
            sql,
            [address, district, normalizeId(cityId), postalCode, phone, location],
            (err, result) => {
                if (err) {
                    logger.error('createAddress MySQL Error:', err);
                    return cb(err);
                }
                cb(null, result);
            }
        );
    } catch (ex) {
        logger.error('createAddress sync error:', ex);
        cb(ex);
    }
};

export const readAddressById = (addressId, callback) => {
    const cb = onceCallback(callback);
    try {
        logger.warn('readAddressById is not implemented yet.');
        cb(null, null);
    } catch (err) {
        logger.error('readAddressById MySql error:', err);
        cb(err);
    }
};

export const readAddress = (address, callback) => {
    const cb = onceCallback(callback);
    try {
        logger.warn('readAddressById is not implemented yet.');
        cb(null, null);
    } catch (err) {
        logger.error('readAddressById MySql error:', err);
        cb(err);
    }
};
export const updateAddress = (addressId, addressData, callback) => {
    const cb = onceCallback(callback);
    try {
        logger.warn('updateAddress is not implemented yet.');
        cb(null, null);
    } catch (err) {
        logger.error('updateAddress MySql error:', err);
        cb(err);
    }
};

export const deleteAddress = (addressId, callback) => {
    const cb = onceCallback(callback);
    try {
        logger.warn('deleteAddress is not implemented yet.');
        cb(null, null);
    } catch (err) {
        logger.error('deleteAddress MySql error:', err);
        cb(err);
    }
};

export const getOrCreateCountry = (countryName, callback) => {
    const sql = `
        INSERT IGNORE INTO country (country)
        VALUES (?)
    `;
    query(sql, [countryName], (err) => {
        if (err) return callback(err);
        query('SELECT * FROM country WHERE country = ?', [countryName], (err, rows) => {
            if (err) return callback(err);
            callback(null, rows[0]);
        });
    });
};

export const getOrCreateCity = (cityName, countryId, callback) => {
    const sql = `
        INSERT IGNORE INTO city (city, country_id)
        VALUES (?, ?)
    `;
    query(sql, [cityName, normalizeId(countryId)], (err) => {
        if (err) return callback(err);
        query(
            'SELECT * FROM city WHERE city = ? AND country_id = ?',
            [cityName, normalizeId(countryId)],
            (err, rows) => {
                if (err) return callback(err);
                callback(null, rows[0]);
            }
        );
    });
};
