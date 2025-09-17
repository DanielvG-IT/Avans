import { getActorsInMovie } from '../dao/actor.js';
import { logger } from '../util/logger.js';
import {
    readMovies,
    readMovieById,
    readPopularMovies,
    readLongestMovies,
    readCheapestMovies,
    readMovieAvailability,
    updateMovie as updateMovieDAO,
    createMovie,
    updateMovie,
    addMovieToCategory, // added
} from '../dao/movie.js';

/**
 * Ensure a callback is only invoked once.
 */
const onceCallback = (cb) => {
    let called = false;
    return (err, res) => {
        if (called) return;
        called = true;
        try {
            if (typeof cb === 'function') cb(err, res);
        } catch (e) {
            // Guard against exceptions thrown by the caller's callback
            logger.error('Service callback threw an error:', e);
        }
    };
};

/* ---------------------------
   Thin service functions
   --------------------------- */

export const addMovie = (
    title,
    description,
    releaseYear,
    languageId,
    categoryId,
    rentalDuration,
    rentalRate,
    length,
    replacementCost,
    rating,
    specialFeatures,
    callback
) => {
    const cb = onceCallback(callback);
    try {
        // Basic validation (keep minimal and let DB enforce detailed constraints)
        if (!title) {
            const err = new Error('Title is required.');
            logger.warn('addMovie - validation error: missing title');
            return cb(err);
        }
        if (!languageId) {
            const err = new Error('Language is required.');
            logger.warn('addMovie - validation error: missing languageId');
            return cb(err);
        }
        if (!rating) {
            const err = new Error('Rating is required.');
            logger.warn('addMovie - validation error: missing rating');
            return cb(err);
        }

        // Sanitize special_features to match Sakila's SET values
        const allowedFeatures = new Set([
            'Trailers',
            'Commentaries',
            'Deleted Scenes',
            'Behind the Scenes',
        ]);
        let sanitizedSpecialFeatures = '';
        if (typeof specialFeatures === 'string' && specialFeatures.trim().length > 0) {
            sanitizedSpecialFeatures = specialFeatures
                .split(',')
                .map((s) => s.trim())
                .filter(Boolean)
                .filter((s) => allowedFeatures.has(s))
                .join(',');
        }

        // Call DAO to create movie
        createMovie(
            title,
            description,
            releaseYear,
            languageId,
            rentalDuration,
            rentalRate,
            length,
            replacementCost,
            rating,
            sanitizedSpecialFeatures,
            (error, result) => {
                if (error) {
                    // Translate MySQL truncation into a friendly validation message
                    if (error && (error.code === 'WARN_DATA_TRUNCATED' || error.errno === 1265)) {
                        const friendly = new Error(
                            'Invalid special features. Allowed: Trailers, Commentaries, Deleted Scenes, Behind the Scenes.'
                        );
                        friendly.statusCode = 400;
                        logger.warn('addMovie - invalid special_features provided');
                        return cb(friendly);
                    }
                    logger.error('addMovie - dao error:', error);
                    return cb(error);
                }
                if (!result || result.insertId === undefined || result.affectedRows !== 1) {
                    logger.error('addMovie - unexpected result:', result);
                    return cb(new Error('Failed to add movie.'));
                }

                // Add the movie to a category
                addMovieToCategory(result.insertId, categoryId, (catError, catResult) => {
                    if (catError) {
                        logger.error('addMovie - addMovieToCategory error:', catError);
                        return cb(catError);
                    }
                    if (!catResult || catResult.affectedRows !== 1) {
                        logger.error('addMovie - addMovieToCategory unexpected result:', catResult);
                        return cb(new Error('Failed to add movie to category.'));
                    }

                    // Success - return the new movie id
                    cb(null, result.insertId);
                });
            }
        );
    } catch (err) {
        logger.error('addMovie sync error:', err);
        cb(err);
    }
};

export const fetchPopularMovies = (limit = 10, callback) => {
    const cb = onceCallback(callback);
    try {
        readPopularMovies(limit, (error, movies) => {
            if (error) {
                logger.error('fetchPopularMovies - dao error:', error);
                return cb(error);
            }

            if (!movies) {
                logger.error('fetchPopularMovies - no movies found');
                return cb(new Error('No movies found.'));
            }

            const mapped = (movies || []).map((f) => ({
                filmId: f.film_id,
                title: f.title,
                rentalCount: f.rental_count,
                category: f.category,
                coverUrl: f.cover_url,
            }));

            cb(null, mapped);
        });
    } catch (err) {
        logger.error('fetchPopularMovies sync error:', err);
        cb(err);
    }
};

export const fetchLongestMovies = (limit = 10, callback) => {
    const cb = onceCallback(callback);
    try {
        readLongestMovies(limit, (error, movies) => {
            if (error) {
                logger.error('fetchLongestMovies - dao error:', error);
                return cb(error);
            }
            if (!movies) {
                logger.error('fetchLongestMovies - no movies found');
                return cb(new Error('No movies found.'));
            }

            const mapped = (movies || []).map((f) => ({
                filmId: f.film_id,
                title: f.title,
                length: f.length,
                category: f.category,
                coverUrl: f.cover_url,
            }));

            cb(null, mapped);
        });
    } catch (err) {
        logger.error('fetchLongestMovies sync error:', err);
        cb(err);
    }
};

export const fetchCheapestMovies = (limit = 10, callback) => {
    const cb = onceCallback(callback);
    try {
        readCheapestMovies(limit, (error, movies) => {
            if (error) {
                logger.error('fetchCheapestMovies - dao error:', error);
                return cb(error);
            }

            if (!movies) {
                logger.error('fetchCheapestMovies - no movies found');
                return cb(new Error('No movies found.'));
            }

            const mapped = (movies || []).map((f) => ({
                filmId: f.film_id,
                title: f.title,
                rentalRate: f.rental_rate,
                category: f.category,
                coverUrl: f.cover_url,
            }));

            cb(null, mapped);
        });
    } catch (err) {
        logger.error('fetchCheapestMovies sync error:', err);
        cb(err);
    }
};

/**
 * fetchMovies: supports the DAO change where readMovies returns:
 * { movies: [...], total: N } OR old-style array (backwards compat).
 * Returns { movies: mappedArray, total }.
 */
export const fetchMovies = (filters = {}, callback) => {
    const cb = onceCallback(callback);
    try {
        readMovies(filters, (error, result) => {
            if (error) {
                logger.error('fetchMovies - dao error:', error);
                return cb(error);
            }

            if (!result) {
                logger.error('fetchMovies - no movies found');
                return cb(new Error('No movies found.'));
            }

            // Support both shapes: { movies, total } or legacy array
            let rows = [];
            let total = 0;
            if (result == null) {
                rows = [];
                total = 0;
            } else if (Array.isArray(result)) {
                rows = result;
                total = result.length;
            } else if (typeof result === 'object') {
                rows = result.movies || [];
                total = typeof result.total === 'number' ? result.total : rows.length || 0;
            }

            const mapped = (rows || []).map((f) => ({
                filmId: f.film_id,
                title: f.title,
                description: f.description,
                releaseYear: f.release_year,
                languageId: f.language_id,
                originalLanguage: f.original_language,
                rentalDuration: f.rental_duration,
                rentalRate: f.rental_rate,
                length: f.length,
                replacementCost: f.replacement_cost,
                rating: f.rating,
                specialFeatures: f.special_features,
                lastUpdate: f.last_update,
                category: f.category,
                coverUrl: f.cover_url,
            }));

            cb(null, { movies: mapped, total });
        });
    } catch (err) {
        logger.error('fetchMovies sync error:', err);
        cb(err);
    }
};

/**
 * fetchMovieById: parallelize actors and availability to reduce latency.
 * Returns mapped movie object or null if not found.
 */
export const fetchMovieById = (id, callback) => {
    const cb = onceCallback(callback);
    try {
        readMovieById(id, (error, movie) => {
            if (error) {
                logger.error('fetchMovieById - dao error:', error);
                return cb(error);
            }

            if (!movie) {
                logger.error('fetchMovieById - no movie found');
                return cb(new Error('No movie found.'));
            }

            // Traditional callback stack: fetch actors, then availability
            try {
                getActorsInMovie(id, (errA, actors) => {
                    if (errA) {
                        logger.error('fetchMovieById - actors dao error:', errA);
                        return cb(errA);
                    }

                    if (!actors) {
                        logger.error('fetchMovieById - no actors found');
                        return cb(new Error('No actors found.'));
                    }

                    const actorArray = (actors || []).map((a) => ({
                        firstName: a.first_name,
                        lastName: a.last_name,
                    }));

                    try {
                        readMovieAvailability(id, (errS, stores) => {
                            if (errS) {
                                logger.error('fetchMovieById - availability dao error:', errS);
                                return cb(errS);
                            }

                            if (!stores) {
                                logger.error('fetchMovieById - no stores found');
                                return cb(new Error('No stores found.'));
                            }

                            const storeArray = (stores || []).map((s) => ({
                                storeName: s.store_name,
                                copies: s.copies,
                            }));

                            const mapped = {
                                filmId: movie.film_id,
                                title: movie.title,
                                description: movie.description,
                                releaseYear: movie.release_year,
                                language: movie.language,
                                rentalDuration: movie.rental_duration,
                                rentalRate: movie.rental_rate,
                                length: movie.length,
                                replacementCost: movie.replacement_cost,
                                rating: movie.rating,
                                specialFeatures: movie.special_features,
                                lastUpdate: movie.last_update,
                                category: movie.category,
                                coverUrl: movie.cover_url,
                                actors: actorArray,
                                availability: storeArray,
                            };

                            cb(null, mapped);
                        });
                    } catch (e) {
                        logger.error('fetchMovieById - readMovieAvailability sync error:', e);
                        cb(e);
                    }
                });
            } catch (e) {
                logger.error('fetchMovieById - getActorsInMovie sync error:', e);
                cb(e);
            }
        });
    } catch (err) {
        logger.error('fetchMovieById sync error:', err);
        cb(err);
    }
};

export const updateMovieById = (
    id,
    title,
    description,
    releaseYear,
    languageId,
    rentalDuration,
    rentalRate,
    length,
    replacementCost,
    rating,
    specialFeatures,
    callback
) => {
    const cb = onceCallback(callback);
    updateMovie(
        id,
        title,
        description,
        releaseYear,
        languageId,
        rentalDuration,
        rentalRate,
        length,
        replacementCost,
        rating,
        specialFeatures,
        (error, result) => {
            if (error) {
                logger.error('updateMovieById - dao error:', error);
                return cb(error);
            }
            if (!result || result.affectedRows !== 1) {
                logger.error('updateMovieById - unexpected result:', result);
                return cb(new Error('Failed to update movie.'));
            }
            // Success
            cb(null, result);
        }
    );
};
