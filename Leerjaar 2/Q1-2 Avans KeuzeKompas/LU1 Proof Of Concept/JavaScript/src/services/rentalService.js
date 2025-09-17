import { readRentalById, readActiveRentalByInventoryId, createRental } from '../dao/rental.js';
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
        logger.debug('addRental called with:', { movieId, customerId, staffId, storeId });

        readMovieById(movieId, (error, movie) => {
            if (error) {
                logger.error('addRental - error reading movie:', error);
                return cb(new Error('Failed to read movie data.'));
            }
            logger.debug('Movie retrieved:', movie);

            if (!movie) {
                logger.error('addRental - movie not found for id:', movieId);
                return cb(new Error('Movie not found.'));
            }

            readInventoryByFilmIdAtStore(movie.film_id, storeId, (error, inventory) => {
                if (error) {
                    logger.error('addRental - error reading inventory:', error);
                    return cb(new Error('Failed to read inventory data.'));
                }
                logger.debug('Inventory retrieved for film and store:', {
                    filmId: movie.film_id,
                    storeId,
                    inventory,
                });

                if (!inventory || inventory.length === 0) {
                    logger.error(`No inventory found for "${movie.title}" at store ${storeId}`);
                    return cb(
                        new Error(
                            `No inventory available for "${movie.title}" at the selected store.`
                        )
                    );
                }

                let foundAvailable = false;
                let pending = inventory.length;
                logger.debug(
                    'Checking availability for each inventory item, total items:',
                    pending
                );

                for (const item of inventory) {
                    logger.debug('Checking inventory item:', item.inventory_id);

                    readActiveRentalByInventoryId(item.inventory_id, (error, activeRental) => {
                        if (foundAvailable) return;
                        if (error) {
                            logger.error(
                                'addRental - error reading rentals for inventory:',
                                item.inventory_id,
                                error
                            );
                            foundAvailable = true;
                            return cb(new Error('Failed to check rental status for inventory.'));
                        }

                        logger.debug(
                            'Active rental for inventory',
                            item.inventory_id,
                            activeRental
                        );

                        if (!activeRental) {
                            // ✅ This copy is available
                            logger.debug('Found available copy:', item.inventory_id);
                            foundAvailable = true;
                            const rentalDate = new Date();

                            createRental(
                                rentalDate,
                                item.inventory_id,
                                customerId,
                                null,
                                staffId,
                                (error, rental) => {
                                    if (error) {
                                        logger.error('addRental - error creating rental:', error);
                                        return cb(new Error('Failed to create rental.'));
                                    }
                                    logger.debug('Rental created successfully:', rental);
                                    cb(null, rental);
                                }
                            );
                        } else {
                            pending--;
                            logger.debug('Inventory item rented, pending:', pending);
                            if (pending === 0 && !foundAvailable) {
                                logger.error(
                                    `No available copies of "${movie.title}" at store ${storeId}`
                                );
                                cb(
                                    new Error(
                                        `No available copies of "${movie.title}" at the selected store.`
                                    )
                                );
                            }
                        }
                    });
                }
            });
        });
    } catch (err) {
        logger.error('addRental - unexpected error:', err);
        cb(new Error('Unexpected error occurred while adding rental.'));
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
