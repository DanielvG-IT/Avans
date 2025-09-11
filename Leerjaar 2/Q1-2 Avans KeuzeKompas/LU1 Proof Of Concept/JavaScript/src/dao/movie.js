import { query } from '../data/db.js';
import { logger } from '../util/logger.js';

/**
 * Helper to ensure a callback is only called once.
 * Returns a wrapper around the original callback.
 */
const onceCallback = (cb) => {
    let called = false;
    return (err, res) => {
        if (called) return;
        called = true;
        try {
            if (typeof cb === 'function') cb(err, res);
        } catch (e) {
            // In case the caller throws inside their callback, log it but don't crash
            logger.error('Callback threw an error:', e);
        }
    };
};

/**
 * Small helper to normalise "maybe-array" inputs
 */
const toArray = (v) => {
    if (v == null) return [];
    return Array.isArray(v) ? v : [v];
};

export const getPopularMovies = (limit = 10, callback) => {
    const cb = onceCallback(callback);
    try {
        limit = Number(limit) || 10;
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
            if (error) {
                logger.error('getPopularMovies MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('getPopularMovies sync error:', err);
        cb(err);
    }
};

export const getLongestMovies = (limit = 10, callback) => {
    const cb = onceCallback(callback);
    try {
        limit = Number(limit) || 10;
        const sql = `
      SELECT f.film_id, f.title, f.length, c.name AS category, fc2.cover_url
      FROM film f
      JOIN film_category fc ON fc.film_id = f.film_id
      JOIN category c ON c.category_id = fc.category_id
      LEFT JOIN film_cover fc2 ON fc2.film_id = f.film_id
      ORDER BY f.length DESC
      LIMIT ?;
    `;
        query(sql, [limit], (error, rows) => {
            if (error) {
                logger.error('getLongestMovies MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('getLongestMovies sync error:', err);
        cb(err);
    }
};

export const getCheapestMovies = (limit = 10, callback) => {
    const cb = onceCallback(callback);
    try {
        limit = Number(limit) || 10;
        const sql = `
      SELECT f.film_id, f.title, f.rental_rate, c.name AS category, fc2.cover_url
      FROM film f
      JOIN film_category fc ON fc.film_id = f.film_id
      JOIN category c ON c.category_id = fc.category_id
      LEFT JOIN film_cover fc2 ON fc2.film_id = f.film_id
      ORDER BY f.rental_rate ASC
      LIMIT ?;
    `;
        query(sql, [limit], (error, rows) => {
            if (error) {
                logger.error('getCheapestMovies MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows);
        });
    } catch (err) {
        logger.error('getCheapestMovies sync error:', err);
        cb(err);
    }
};

export const getMovies = (filters = {}, callback) => {
    const cb = onceCallback(callback);
    try {
        let {
            page = 0,
            limit = 10,
            search = '',
            sortBy = { title: 'asc' },
            category,
            rating,
        } = filters;

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

        // Determine sort key and direction
        const sortKey = Object.keys(sortBy || {})[0] || 'title';
        const rawDir = (sortBy && sortBy[sortKey]) || 'asc';
        const sortDir = String(rawDir).toUpperCase() === 'DESC' ? 'DESC' : 'ASC';
        const orderBy = sortWhitelist[sortKey] || 'f.title';

        // Build WHERE clauses and params, and ensure we append them to SQL
        let whereSql = ' WHERE 1=1';
        const params = [];
        const countParams = [];

        if (search) {
            whereSql += ' AND (f.title LIKE ? OR f.description LIKE ?)';
            params.push(`%${search}%`, `%${search}%`);
            countParams.push(`%${search}%`, `%${search}%`);
        }

        if (category) {
            const catList = toArray(category).filter(Boolean);
            if (catList.length > 0) {
                whereSql += ` AND c.name IN (${catList.map(() => '?').join(',')})`;
                params.push(...catList);
                countParams.push(...catList);
            }
        }

        if (rating) {
            const ratingList = toArray(rating).filter(Boolean);
            if (ratingList.length > 0) {
                whereSql += ` AND f.rating IN (${ratingList.map(() => '?').join(',')})`;
                params.push(...ratingList);
                countParams.push(...ratingList);
            }
        }

        // SELECT for rows
        const sql = `
      SELECT f.film_id, f.title, f.description, f.release_year, f.language_id,
             f.rental_duration, f.rental_rate, f.length, f.replacement_cost,
             f.rating, f.special_features, f.last_update,
             c.name as category, fc2.cover_url
      FROM film f
      JOIN film_category fc ON f.film_id = fc.film_id
      JOIN category c ON c.category_id = fc.category_id
      LEFT JOIN film_cover fc2 ON fc2.film_id = f.film_id
      ${whereSql}
      ORDER BY ${orderBy} ${sortDir}
      LIMIT ?, ?;
    `;

        // count query
        const countSql = `
      SELECT COUNT(DISTINCT f.film_id) AS total
      FROM film f
      JOIN film_category fc ON f.film_id = fc.film_id
      JOIN category c ON c.category_id = fc.category_id
      ${whereSql};
    `;

        // add pagination params only to the row query's params
        const rowParams = params.slice(); // copy
        rowParams.push(page * limit, limit);

        // execute count first
        query(countSql, countParams, (countErr, countRows) => {
            if (countErr) {
                logger.error('MySQL Count Error:', countErr);
                return cb(countErr);
            }

            const total =
                countRows && countRows[0] && countRows[0].total ? Number(countRows[0].total) : 0;

            // then rows
            query(sql, rowParams, (error, rows) => {
                if (error) {
                    logger.error('MySQL Select Error:', error);
                    return cb(error);
                }
                cb(null, { movies: rows || [], total });
            });
        });
    } catch (err) {
        logger.error('getMovies sync error:', err);
        cb(err);
    }
};

export const getMovieById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
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
            if (error) {
                logger.error('getMovieById MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows && rows[0] ? rows[0] : null);
        });
    } catch (err) {
        logger.error('getMovieById sync error:', err);
        cb(err);
    }
};

export const getMovieAvailability = (id, callback) => {
    const cb = onceCallback(callback);
    try {
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
            if (error) {
                logger.error('getMovieAvailability MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows || []);
        });
    } catch (err) {
        logger.error('getMovieAvailability sync error:', err);
        cb(err);
    }
};

export const getRatings = (callback) => {
    const cb = onceCallback(callback);
    try {
        const sql = `SELECT DISTINCT rating FROM film`;
        query(sql, [], (error, rows) => {
            if (error) {
                logger.error('getRatings MySQL Error:', error);
                return cb(error);
            }
            cb(null, rows || []);
        });
    } catch (err) {
        logger.error('getRatings sync error:', err);
        cb(err);
    }
};
