import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

export const getActors = (callback) => {
    const sql = `
        SELECT * FROM actor
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

export const getActorsInMovie = (movieId, callback) => {
    const sql = `
        SELECT a.first_name, a.last_name FROM actor a
        JOIN film_actor fa ON fa.actor_id = a.actor_id
        JOIN film f ON f.film_id = fa.film_id
        WHERE f.film_id = ?
    `;
    query(sql, [movieId], (err, rows) => {
        if (err) {
            logger.error('MySQL Error:', err);
            return callback(err);
        }
        callback(err, rows);
    });
};
