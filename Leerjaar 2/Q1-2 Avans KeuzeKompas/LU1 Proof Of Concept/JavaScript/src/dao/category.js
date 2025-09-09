import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

export const getCategories = (callback) => {
    const sql = `
        SELECT * FROM category
        ORDER BY name
    `;
    query(sql, [], (err, rows) => {
        if (err) {
            logger.error('MySQL Error:', err);
            return callback(err);
        }
        callback(err, rows);
    });
};
