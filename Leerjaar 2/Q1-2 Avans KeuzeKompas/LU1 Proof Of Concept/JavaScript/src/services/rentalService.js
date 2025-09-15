import { readRentalById, readRentalsByInventoryId, createRental } from '../dao/rental.js';
import { readInventoryByFilmIdAtStore } from '../dao/inventory.js';
import { readMovieById } from '../dao/movie.js';
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

export const addRental = (movieId, customerId, staffId, storeId, callback) => {
    const cb = onceCallback(callback);
    try {
        readMovieById(movieId, (error, movie) => {
            if (error) {
                logger.error('fetchRentalById - dao error:', error);
                return cb(error);
            }
            if (!movie) {
                logger.error('');
                return cb('nah movie mate');
            }

            readInventoryByFilmIdAtStore(movie.film_id, storeId, (error, inventory) => {
                if (error) {
                    logger.error('fetchRentalById - dao error:', error);
                    return cb(error);
                }
                if (!inventory) {
                    logger.error('');
                    return cb('nah inventory mate');
                }

                let firstAvailableCopy;
                // So for example 4 copies mean 4 rows of inventory
                for (const item of inventory) {
                    readRentalsByInventoryId(item.inventory_id, (error, rentals) => {
                        if (error) {
                            logger.error('fetchRentalById - dao error:', error);
                            return cb(error);
                        }
                        if (rentals && rentals.length > 0) {
                            // This copy is rented out, try the next one
                            return;
                        }

                        // Found an available copy
                        firstAvailableCopy = item;
                    });
                }

                if (!firstAvailableCopy) {
                    logger.error('');
                    return cb('nah no copies mate');
                }

                // All good, create the rental
                const rentalDate = new Date();
                const inventoryId = firstAvailableCopy.inventory_id;
                const returnDate = null; // Not returned yet

                // Create the rental in the database
                createRental(
                    rentalDate,
                    inventoryId,
                    customerId,
                    returnDate,
                    staffId,
                    (error, rental) => {
                        if (error) {
                            logger.error('addRental - dao error:', error);
                            return cb(error);
                        }
                        cb(null, rental);
                    }
                );
            });
        });
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
