import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

export const getPopularFilms = (limit, callback) => {
    const sql = `
      SELECT f.film_id, f.title, rc.rental_count, c.name as category, fc2.cover_url
      FROM (
          SELECT i.film_id, COUNT(*) AS rental_count
          FROM rental r
          JOIN inventory i ON r.inventory_id = i.inventory_id
          GROUP BY i.film_id
          ORDER BY rental_count DESC
          LIMIT 8
      ) AS rc
      JOIN film f ON f.film_id = rc.film_id
      JOIN film_category fc ON fc.film_id = rc.film_id
      JOIN category c ON c.category_id = fc.category_id
      LEFT JOIN film_cover fc2 ON fc2.film_id = f.film_id
      ORDER BY rc.rental_count DESC;
    `;

    query(sql, [limit], (err, rows) => {
        if (typeof callback !== 'function') return;
        if (err) {
            logger.error('MySQL Error:', err);
            return callback(err);
        }
        callback(null, rows);
    });
};
