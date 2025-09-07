import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

export const getCategories = (limit, callback) => {
    const sql = `
        SELECT * FROM categories
        ORDER BY name
    `;
    query(sql, [], (err, rows) => {
        callback(err, rows);
    });
};
