import { getCategories } from '../dao/category.js';
import logger from '../util/logger.js';

export const fetchCategories = (callback) => {
    getCategories((err, categories) => {
        if (err) {
            logger.error('Category Error:', err);
            return callback(err);
        }

        const mapped = categories.map((c) => ({
            categoryId: c.category_id,
            name: c.name,
        }));

        callback(null, mapped);
    });
};
