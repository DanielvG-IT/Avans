import express from 'express';
import { logger } from '../util/logger.js';
import { fetchCategoryNames, fetchRatingNames } from '../services/filterService.js';
import { fetchMovieById, fetchMovies } from '../services/movieService.js';

const moviesRouter = express.Router();

/**
 * Normalize a query param into an array.
 * Accepts:
 * - undefined/null -> []
 * - array -> normalized strings
 * - "a,b" -> ['a','b']
 * - "single" -> ['single']
 */
const normalizeMulti = (val) => {
    if (val == null) return [];
    if (Array.isArray(val)) return val.map((v) => String(v).trim()).filter(Boolean);
    if (typeof val === 'string') {
        return val
            .split(',')
            .map((s) => s.trim())
            .filter(Boolean);
    }
    return [];
};

/**
 * Helper to read int with fallback
 */
const parsePositiveInt = (v, fallback) => {
    const n = Number.parseInt(v, 10);
    return Number.isFinite(n) && n > 0 ? n : fallback;
};

moviesRouter.get('/', (req, res, next) => {
    // done() guard prevents double next()/res.send()
    let finished = false;
    const done = (err) => {
        if (finished) return;
        finished = true;
        if (err) return next(err);
    };

    try {
        const page = Math.max(parseInt(req.query.page, 10) || 1, 1);
        const limit = parsePositiveInt(req.query.limit, 25);
        const search = (req.query.search || '').trim();

        // parse sort param supporting "field,dir", "field:dir" or "field dir"
        const rawSort = (req.query.sort || 'title,asc').toString();
        let sortField;
        let sortDirRaw;
        if (rawSort.includes(',')) {
            [sortField, sortDirRaw] = rawSort.split(',');
        } else if (rawSort.includes(':')) {
            [sortField, sortDirRaw] = rawSort.split(':');
        } else {
            [sortField, sortDirRaw] = rawSort.split(/\s+/);
        }
        sortField = (sortField || 'title').trim();
        sortDirRaw = (sortDirRaw || 'asc').trim();
        const sortDir = sortDirRaw.toLowerCase() === 'desc' ? 'desc' : 'asc';
        const sortBy = { [sortField]: sortDir };

        // Normalize category & rating (accept single string, repeated params or CSV)
        const categoryArr = normalizeMulti(req.query.category);
        const ratingArr = normalizeMulti(req.query.rating);

        // fetch categories and ratings in parallel (callbacks)
        let categories = [];
        let ratings = [];
        let pending = 2;
        let errored = false;

        const checkAndContinue = () => {
            if (errored) return;
            pending -= 1;
            if (pending === 0) {
                // Now call fetchMovies
                fetchMovies(
                    {
                        page: page - 1, // service expects zero-based page
                        limit,
                        search,
                        sortBy,
                        category: categoryArr,
                        rating: ratingArr,
                    },
                    (err, result) => {
                        if (err) {
                            errored = true;
                            logger.error('Movies Error:', err);
                            return done(err);
                        }

                        // Support both shapes: { movies, total } or legacy array
                        let rows = [];
                        let total = 0;
                        if (Array.isArray(result)) {
                            rows = result;
                            total = result.length;
                        } else if (result && typeof result === 'object') {
                            rows = result.movies || [];
                            total =
                                typeof result.total === 'number'
                                    ? result.total
                                    : typeof result.totalCount === 'number'
                                    ? result.totalCount
                                    : rows.length;
                        } else {
                            rows = [];
                            total = 0;
                        }

                        const totalPages = Math.max(Math.ceil(total / limit) || 1, 1);

                        // Prepare values for template; keep category & rating UX-friendly
                        const selectedCategory = categoryArr.length ? categoryArr.join(',') : '';
                        const selectedRating = ratingArr.length ? ratingArr.join(',') : '';

                        console.log(ratings);

                        // Render and finish
                        res.render('movies', {
                            title: 'Movies',
                            movies: rows,
                            categories,
                            ratings,
                            search,
                            category: selectedCategory,
                            rating: selectedRating,
                            sort: sortField,
                            sortDir,
                            page,
                            totalPages,
                            limit,
                        });
                    }
                );
            }
        };

        // kick off parallel fetches
        fetchCategoryNames((err, cats) => {
            if (finished) return;
            if (err) {
                errored = true;
                logger.error('Category names error:', err);
                return done(err);
            }
            categories = cats || [];
            checkAndContinue();
        });

        fetchRatingNames((err, rts) => {
            if (finished) return;
            if (err) {
                errored = true;
                logger.error('Rating names error:', err);
                return done(err);
            }
            ratings = rts || [];
            checkAndContinue();
        });
    } catch (err) {
        logger.error('Movies route sync error:', err);
        return done(err);
    }
});

moviesRouter.get('/:id', (req, res, next) => {
    let finished = false;
    const done = (err) => {
        if (finished) return;
        finished = true;
        if (err) return next(err);
    };

    try {
        const { id } = req.params;
        if (!id) return done(new Error('Invalid movie id'));

        const intId = parseInt(id, 10);
        if (!Number.isInteger(intId) || intId <= 0) return done(new Error('Invalid movie id'));

        logger.debug(`Getting information for movie with ID: ${intId}`);

        fetchMovieById(intId, (err, movie) => {
            if (finished) return;
            if (err) {
                logger.error('Movie Error:', err);
                return done(err);
            }

            if (!movie) {
                // Not found -> 404
                finished = true;
                return res
                    .status(404)
                    .render('404', { title: 'Movie not found', message: 'Movie not found' });
            }

            res.render('movie', { title: movie.title, movie });
        });
    } catch (err) {
        logger.error('Movie route sync error:', err);
        return done(err);
    }
});

export default moviesRouter;
