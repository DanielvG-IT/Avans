import { getCategories } from '../dao/category.js';
import { logger } from '../util/logger.js';

export const fetchCategories = (callback) => {
    getCategories((err, categories) => {
        if (err) {
            logger.error('Category Error:', err);
            return callback(err);
        }

        callback(null, categories);
    });
};

export const fetchCategoryNames = (callback) => {
    fetchCategories((err, categories) => {
        if (err) {
            return callback(err);
        }

        const names = categories.map((c) => c.name);
        callback(null, names);
    });
};
