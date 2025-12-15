import { readStoreById, readStores, createStore } from '../dao/store.js';
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

export const addStore = (name, addressId, managerStaffId, callback) => {
    const cb = onceCallback(callback);
    try {
        createStore(name, addressId, managerStaffId, (error, result) => {
            if (error) {
                logger.error('addStore - dao error:', error);
                return cb(error);
            }
            cb(null, result);
        });
    } catch (err) {
        logger.error('addStore - unexpected error:', err);
        cb(err);
    }
};

export const fetchAllStores = (callback) => {
    const cb = onceCallback(callback);
    logger.debug('fetchAllStores - called');
    try {
        readStores((error, stores) => {
            if (error) {
                logger.error('fetchAllStores - dao error:', error);
                return cb(error);
            }

            if (!stores) {
                logger.error('fetchAllStores - no stores found');
                return cb(new Error('No stores found.'));
            }

            const storeArray = (stores || []).map((s) => ({
                id: s.store_id,
                name: s.name,
                addressId: s.address_id,
            }));
            logger.debug(`fetchAllStores - found ${storeArray.length} stores`);
            return cb(null, storeArray);
        });
    } catch (err) {
        logger.error('fetchAllStores - unexpected error:', err);
        cb(err);
    }
};

export const fetchStoreById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
        readStoreById(id, (error, stores) => {
            if (error) {
                logger.error('fetchStoreById - dao error:', error);
                return cb(error);
            }

            if (!stores || stores.length === 0) {
                logger.error(`fetchStoreById - no store found with id ${id}`);
                return cb(new Error('Store not found.'));
            }

            const s = stores[0];
            const store = {
                id: s.store_id,
                name: s.store_name,
                addressId: s.address_id,
            };
            cb(null, store);
        });
    } catch (err) {
        logger.error('fetchStoreById - unexpected error:', err);
        cb(err);
    }
};
