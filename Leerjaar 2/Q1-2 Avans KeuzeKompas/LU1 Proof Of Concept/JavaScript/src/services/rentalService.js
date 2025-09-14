import {
    readInventoryByFilmId,
    readInventoryByStoreId,
    readRentalsByInventoryId,
} from '../dao/inventory.js';
import { readRentalById, readRentalsByInventoryId } from '../dao/rental.js';
import { logger } from '../util/logger.js';

/**
 * Ensure a callback is only invoked once.
 */
const onceCallback = (cb) => {
    let called = false;
    return (err, res) => {
        if (called) return;
        called = true;
        try {
            if (typeof cb === 'function') cb(err, res);
        } catch (e) {
            // Guard against exceptions thrown by the caller's callback
            logger.error('Service callback threw an error:', e);
        }
    };
};

export const addRental = (name, addressId, managerStaffId, callback) => {
    const cb = onceCallback(callback);
    try {
        // Not implemented yet
    } catch (err) {
        logger.error('addRental - unexpected error:', err);
        cb(err);
    }
};

export const fetchRentalById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
        readRentalById(id, (error, rentals) => {
            if (error) {
                logger.error('fetchRentalById - dao error:', error);
                return cb(error);
            }
            cb(null, rentals);
        });
    } catch (err) {
        logger.error('fetchRentalById - unexpected error:', err);
        cb(err);
    }
};
