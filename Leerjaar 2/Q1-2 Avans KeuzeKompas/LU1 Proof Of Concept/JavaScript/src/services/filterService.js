import { getCategories } from '../dao/category.js';
import { getRatings } from '../dao/movie.js';
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
    const safeCallback = onceCallback(callback);
    getCategories((error, categories) => {
        if (error) {
            logger.error('Category Error:', error);
            return safeCallback(error);
        }
        if (!categories) {
            logger.error('Category Error:', error);
            return safeCallback(new Error('Categories not found.'));
        }

        const names = categories.map((c) => c.name);
        safeCallback(null, names);
    });
};

export const fetchRatingNames = (callback) => {
    const safeCallback = onceCallback(callback);
    getRatings((error, ratings) => {
        if (error) {
            logger.error('Rating Error:', error);
            return safeCallback(error);
        }
        if (!ratings) {
            logger.error('Rating Error:', error);
            return safeCallback(new Error('Ratings not found.'));
        }

        const names = ratings.map((r) => r.name);
        safeCallback(null, names);
    });
};
