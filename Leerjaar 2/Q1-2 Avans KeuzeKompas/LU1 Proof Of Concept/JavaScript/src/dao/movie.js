import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

export const getPopularMovies = (limit, callback) => {
    const sql = `
        SELECT f.film_id, f.title, rc.rental_count, c.name as category, fc2.cover_url
        FROM (
            SELECT i.film_id, COUNT(*) AS rental_count
            FROM rental r
            JOIN inventory i ON r.inventory_id = i.inventory_id
            GROUP BY i.film_id
            ORDER BY rental_count DESC
            LIMIT ?
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

export const getMovies = (filters, callback) => {
    let { page, limit, search, sortBy, genre } = filters;

    // Construct the SQL query with filters
    let sql = `
        SELECT f.film_id, f.title, f.description, f.release_year, f.language_id, f.rental_duration, f.rental_rate, f.length, f.replacement_cost, f.rating, f.special_features, f.last_update, c.name as category, fc2.cover_url
        FROM film f
        JOIN film_category fc ON f.film_id = fc.film_id
        JOIN category c ON c.category_id = fc.category_id
        LEFT JOIN film_cover fc2 ON fc2.film_id = f.film_id
        WHERE 1=1
    `;

    // Add search filter
    if (search) {
        sql += ` AND (f.title LIKE ? OR f.description LIKE ?)`;
    }

    // Add genre filter
    if (genre && genre.length > 0) {
        sql += ` AND c.name IN (${genre.map(() => '?').join(',')})`;
    }

    // Add sorting
    sql += ` ORDER BY ${Object.keys(sortBy)
        .map((key) => `f.${key} ${sortBy[key]}`)
        .join(', ')}`;
    sql += ` LIMIT ?, ?`;

    // Execute the query
    query(
        sql,
        [...(search ? [`%${search}%`, `%${search}%`] : []), ...(genre || []), page * limit, limit],
        (err, rows) => {
            if (typeof callback !== 'function') return;
            if (err) {
                logger.error('MySQL Error:', err);
                return callback(err);
            }
            callback(null, rows);
        }
    );
};

export const getMovieById = (id, callback) => {
    const sql = `
        SELECT f.film_id, f.title, f.description, f.release_year, f.language_id, f.original_language_id, f.rental_duration, f.rental_rate, f.length, f.replacement_cost, f.rating, f.special_features, f.last_update, c.name as category
        FROM film f
        JOIN film_category fc ON f.film_id = fc.film_id
        JOIN category c ON c.category_id = fc.category_id
        WHERE f.film_id = ?
    `;

    query(sql, [id], (err, rows) => {
        if (typeof callback !== 'function') return;
        if (err) {
            logger.error('MySQL Error:', err);
            return callback(err);
        }
        callback(null, rows[0]);
    });
};
