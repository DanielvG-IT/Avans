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

    query(sql, [limit], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows);
    });
};

export const getMovies = (filters, callback) => {
    let { page = 0, limit = 10, search = '', sortBy = { title: 'asc' }, category } = filters;

    // Normalize numeric params
    page = Number(page) || 0;
    limit = Number(limit) || 10;
    search = search ? String(search) : '';

    // Whitelist of sortable columns to avoid SQL injection
    const sortWhitelist = {
        title: 'f.title',
        year: 'f.release_year',
        length: 'f.length',
        rate: 'f.rental_rate',
    };

    // Determine sort key and direction from the provided sortBy object
    const sortKey = Object.keys(sortBy || {})[0] || 'title';
    const rawDir = (sortBy && sortBy[sortKey]) || 'asc';
    const sortDir = String(rawDir).toUpperCase() === 'DESC' ? 'DESC' : 'ASC';
    const orderBy = sortWhitelist[sortKey] || 'f.title';

    let sql = `
        SELECT f.film_id, f.title, f.description, f.release_year, f.language_id,
               f.rental_duration, f.rental_rate, f.length, f.replacement_cost,
               f.rating, f.special_features, f.last_update,
               c.name as category, fc2.cover_url
        FROM film f
        JOIN film_category fc ON f.film_id = fc.film_id
        JOIN category c ON c.category_id = fc.category_id
        LEFT JOIN film_cover fc2 ON fc2.film_id = f.film_id
        WHERE 1=1
    `;

    const params = [];

    if (search) {
        sql += ` AND (f.title LIKE ? OR f.description LIKE ?)`;
        params.push(`%${search}%`, `%${search}%`);
    }

    if (category && category.length > 0) {
        sql += ` AND c.name IN (${[]
            .concat(category)
            .map(() => '?')
            .join(',')})`;
        params.push(...[].concat(category));
    }

    sql += ` ORDER BY ${orderBy} ${sortDir}`;
    sql += ` LIMIT ?, ?`;

    params.push(page * limit, limit);

    query(sql, params, (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows);
    });
};

export const getMoviesCount = (callback) => {
    const sql = `SELECT COUNT(*) as count FROM film`;
    // TODO: When SELECTing it has to take the filters aswell!
    query(sql, [], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows[0].count);
    });
};

export const getMovieById = (id, callback) => {
    const sql = `SELECT f.film_id, f.title, f.description, f.release_year, l.name as language,
                   f.rental_duration, f.rental_rate, f.length, f.replacement_cost,
                   f.rating, f.special_features, f.last_update,
                   c.name as category, fc2.cover_url
            FROM film f
            JOIN film_category fc ON f.film_id = fc.film_id
            JOIN category c ON c.category_id = fc.category_id
            JOIN language l ON l.language_id = f.language_id 
            LEFT JOIN film_cover fc2 ON fc2.film_id = f.film_id
            WHERE f.film_id = ?`;

    query(sql, [id], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (err) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows[0]);
    });
};

export const getMovieAvailability = (id, callback) => {
    const sql = `
        SELECT 
            s.store_id,
            s.name AS store_name,
            COUNT(i.inventory_id) AS copies
        FROM store s
        JOIN inventory i ON i.store_id = s.store_id
        JOIN film f ON f.film_id = i.film_id
        WHERE f.film_id = ?
        GROUP BY s.store_id, s.name
        ORDER BY s.store_id;
    `;

    query(sql, [id], (error, rows) => {
        if (typeof callback !== 'function') return;
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        callback(null, rows);
    });
};
