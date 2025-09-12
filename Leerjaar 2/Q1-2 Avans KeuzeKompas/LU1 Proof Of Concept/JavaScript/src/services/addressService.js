// services/addressService.js
import {
    getOrCreateCountry,
    getOrCreateCity,
    readAddress,
    createAddress,
    readAddressById,
    updateAddress,
    deleteAddress,
} from '../dao/address.js';
import { logger } from '../util/logger.js';

/**
 * Ensure a callback is only called once.
 */
const onceCallback = (cb) => {
    let called = false;
    return (err, res) => {
        if (called) return;
        called = true;
        try {
            if (typeof cb === 'function') cb(err, res);
        } catch (e) {
            logger.error('Callback threw an error:', e);
        }
    };
};

/**
 * Create an address (creates country and city when needed).
 *
 * Callback signature: (err, { country, city, address }) where each is the DB row
 */
export const makeAddress = (address, district, postalCode, phone, location, callback) => {
    const cb = onceCallback(callback);
    try {
        if (!address || !district || !postalCode || !phone || !location) {
            return cb(new Error('All fields are required to create an address'));
        }
        if (callback && typeof callback !== 'function') {
            return cb(new Error('Callback must be a function'));
        }

        // 1) country
        getOrCreateCountry(countryName, (err, countryRow) => {
            if (err) {
                logger.error('createAddress.getOrCreateCountry error:', err);
                return cb(err);
            }
            if (!countryRow) {
                return cb(new Error('Failed to get or create country'));
            }

            // 2) city
            getOrCreateCity(
                cityName,
                countryRow.country_id || countryRow.id || countryRow.city_id || countryRow.id,
                (err2, cityRow) => {
                    if (err2) {
                        logger.error('createAddress.getOrCreateCity error:', err2);
                        return cb(err2);
                    }
                    if (!cityRow) {
                        return cb(new Error('Failed to get or create city'));
                    }

                    // 3) address create (createAddress expected to accept DB field names)
                    createAddress(
                        address,
                        district,
                        cityRow.city_id,
                        postalCode,
                        phone,
                        location,
                        (err3, created) => {
                            if (err3) {
                                logger.error('createAddress.createAddress error:', err3);
                                return cb(err3);
                            }

                            // Normalize created -> try to return full inserted row
                            // createAddress might return { insertId } or the row object
                            if (created && typeof created === 'object') {
                                // If it already contains the full address row, return it
                                if (created.address_id || created.id) {
                                    const id = created.address_id || created.id;
                                    // try to fetch full row (preferred)
                                    return readAddressById(id, (err4, fullRow) => {
                                        if (err4) {
                                            logger.warn(
                                                'createAddress: could not fetch created address by id, returning repo response',
                                                err4
                                            );
                                            return cb(null, {
                                                country: countryRow,
                                                city: cityRow,
                                                address: created,
                                            });
                                        }
                                        return cb(null, {
                                            country: countryRow,
                                            city: cityRow,
                                            address: fullRow,
                                        });
                                    });
                                }
                                // else return created as-is
                                return cb(null, {
                                    country: countryRow,
                                    city: cityRow,
                                    address: created,
                                });
                            }

                            // If created is an insertId number
                            if (typeof created === 'number') {
                                return readAddressById(created, (err5, fullRow) => {
                                    if (err5) return cb(err5);
                                    return cb(null, {
                                        country: countryRow,
                                        city: cityRow,
                                        address: fullRow,
                                    });
                                });
                            }

                            // Fallback: try to find by unique keys
                            readAddress(address, (err6, rows) => {
                                if (err6) return cb(err6);
                                return cb(null, {
                                    country: countryRow,
                                    city: cityRow,
                                    address: rows && rows[0] ? rows[0] : created,
                                });
                            });
                        }
                    );
                }
            );
        });
    } catch (ex) {
        logger.error('createAddress service error:', ex);
        cb(ex);
    }
};
