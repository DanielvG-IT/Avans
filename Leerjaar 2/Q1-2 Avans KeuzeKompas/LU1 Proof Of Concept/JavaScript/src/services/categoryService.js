import { getCategories } from '../dao/category.js';
import { logger } from '../util/logger.js';

export const fetchCategories = (callback) => {
    getCategories((error, categories) => {
        if (error) {
            logger.error('Category Error:', error);
            callback(error);
        }

        callback(null, categories);
    });
};

export const fetchCategoryNames = (callback) => {
    fetchCategories((error, categories) => {
        if (error) {
            logger.error('Category Error:', error);
            callback(error);
        }

        const names = categories.map((c) => c.name);
        callback(null, names);
    });
};
