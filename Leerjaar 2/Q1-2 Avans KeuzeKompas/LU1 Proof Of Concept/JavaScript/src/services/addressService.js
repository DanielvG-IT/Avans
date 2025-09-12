import {
    getOrCreateCountry,
    readAddressById,
    getOrCreateCity,
    createAddress,
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
export const makeAddress = (
    address,
    district,
    postalCode,
    phone,
    countryName,
    cityName,
    callback
) => {
    const cb = onceCallback(callback);
    try {
        logger.debug('makeAddress called', {
            address,
            district,
            postalCode,
            phone,
            countryName,
            cityName,
        });

        if (!address || !district || !postalCode) {
            logger.debug('makeAddress validation failed: missing required fields', {
                address: !!address,
                district: !!district,
                postalCode: !!postalCode,
            });
            return cb(new Error('All fields are required to create an address'));
        }
        if (callback && typeof callback !== 'function') {
            logger.debug('makeAddress validation failed: callback not a function', {
                callbackType: typeof callback,
            });
            return cb(new Error('Callback must be a function'));
        }

        // 1) country
        logger.debug('makeAddress: attempting to get or create country', { countryName });
        getOrCreateCountry(countryName, (err, countryRow) => {
            if (err) {
                logger.error('createAddress.getOrCreateCountry error:', err);
                return cb(err);
            }
            logger.debug('createAddress.getOrCreateCountry result', { countryRow });

            if (!countryRow) {
                logger.debug('createAddress.getOrCreateCountry returned no row');
                return cb(new Error('Failed to get or create country'));
            }

            // 2) city
            const countryId =
                countryRow.country_id || countryRow.id || countryRow.city_id || countryRow.id;
            logger.debug('makeAddress: attempting to get or create city', {
                cityName,
                countryId,
            });
            getOrCreateCity(cityName, countryId, (err2, cityRow) => {
                if (err2) {
                    logger.error('createAddress.getOrCreateCity error:', err2);
                    return cb(err2);
                }
                logger.debug('createAddress.getOrCreateCity result', { cityRow });

                if (!cityRow) {
                    logger.debug('createAddress.getOrCreateCity returned no row');
                    return cb(new Error('Failed to get or create city'));
                }

                // 3) address create (createAddress expected to accept DB field names)
                const cityId = cityRow.city_id || cityRow.id;
                logger.debug('makeAddress: creating address with params', {
                    address,
                    district,
                    cityId,
                    postalCode,
                    phone,
                    location: typeof location !== 'undefined' ? location : null,
                });
                createAddress(
                    address,
                    district,
                    cityRow.city_id,
                    postalCode,
                    phone,
                    typeof location !== 'undefined' ? location : undefined,
                    (err3, created) => {
                        if (err3) {
                            logger.error('createAddress.createAddress error:', err3);
                            return cb(err3);
                        }

                        logger.debug('createAddress.createAddress result', { created });

                        // If created is an insertId number
                        if (typeof created.insertId === 'number') {
                            logger.debug('createAddress returned insert id, reading by id', {
                                id: created.insertId,
                            });
                            return readAddressById(created.insertId, (err5, fullRow) => {
                                if (err5) {
                                    logger.error('readAddressById error:', err5);
                                    return cb(err5);
                                }
                                logger.debug('readAddressById returned fullRow', { fullRow });
                                return cb(null, {
                                    country: countryRow,
                                    city: cityRow,
                                    address: fullRow,
                                });
                            });
                        }
                    }
                );
            });
        });
    } catch (ex) {
        logger.error('createAddress service error:', ex);
        cb(ex);
    }
};
