import { readCategories } from '../dao/category.js';
import { readRatings } from '../dao/movie.js';
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

export const fetchCategoryNames = (callback) => {
    const cb = onceCallback(callback);
    readCategories((error, categories) => {
        if (error) {
            logger.error('Category Error:', error);
            return cb(error);
        }
        if (!categories) {
            logger.error('Category Error:', error);
            return cb(new Error('Categories not found.'));
        }

        const names = categories.map((c) => c.name);
        cb(null, names);
    });
};

export const fetchRatingNames = (callback) => {
    const cb = onceCallback(callback);
    readRatings((error, ratings) => {
        if (error) {
            logger.error('Rating Error:', error);
            return cb(error);
        }
        if (!ratings) {
            logger.error('Rating Error:', error);
            return cb(new Error('Ratings not found.'));
        }
        const names = ratings.map((r) => r.rating);
        cb(null, names);
    });
};
