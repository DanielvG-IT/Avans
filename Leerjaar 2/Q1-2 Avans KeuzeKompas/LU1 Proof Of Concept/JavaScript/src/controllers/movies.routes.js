import { fetchCategoryNames, fetchRatingNames } from '../services/filterService.js';
import { requireStaffAuthApi, requireStaffAuthWeb } from '../middleware/auth.js';
import { fetchMovieById, fetchMovies } from '../services/movieService.js';
import { logger } from '../util/logger.js';
import express from 'express';

const moviesRouter = express.Router();

// Simple helper to normalize query params into arrays
const normalizeMulti = (val) => {
    if (!val) return [];
    if (Array.isArray(val)) return val;
    if (typeof val === 'string')
        return val
            .split(',')
            .map((s) => s.trim())
            .filter(Boolean);
    return [];
};

// Simple helper to parse ints
const parsePositiveInt = (v, fallback) => {
    const n = parseInt(v, 10);
    return Number.isInteger(n) && n > 0 ? n : fallback;
};

moviesRouter.get('/', (req, res, next) => {
    const page = parsePositiveInt(req.query.page, 1);
    const limit = parsePositiveInt(req.query.limit, 25);
    const search = (req.query.search || '').trim();

    // Sorting (only "field,dir" supported for simplicity)
    let [sortField, sortDir] = (req.query.sort || 'title,asc').split(',');
    sortField = (sortField || 'title').trim();
    sortDir = (sortDir || 'asc').toLowerCase() === 'desc' ? 'desc' : 'asc';

    // TODO: Fix sort (length, releaseYear, rentalRate) actually working but not rendering correctly in the view after

    const categories = normalizeMulti(req.query.category);
    const ratings = normalizeMulti(req.query.rating);

    // Fetch categories and ratings first
    fetchCategoryNames((err, categoryList) => {
        if (err) return next(err);

        fetchRatingNames((err, ratingList) => {
            if (err) return next(err);

            // Now fetch movies
            fetchMovies(
                {
                    page: page - 1, // service expects zero-based
                    limit,
                    search,
                    sortBy: { [sortField]: sortDir },
                    category: categories,
                    rating: ratings,
                },
                (err, result) => {
                    if (err) return next(err);

                    const rows = result.movies || result || [];
                    const total = result.total || rows.length;
                    const totalPages = Math.max(Math.ceil(total / limit), 1);

                    // build encoded base query (without page)
                    const params = new URLSearchParams();
                    if (search) params.set('search', search);
                    if (categories && categories.length)
                        params.set('category', categories.join(','));
                    if (ratings && ratings.length) params.set('rating', ratings.join(','));
                    if (sortField) params.set('sort', `${sortField},${sortDir}`);
                    if (limit && limit !== 25) params.set('limit', String(limit));
                    const baseQuery = params.toString(); // e.g. "search=term&category=Comedy"
                    const pageQueryPrefix = baseQuery ? `?${baseQuery}&page=` : '?page=';

                    // create pagesToShow array (objects: {page: n, active: bool} or {ellipsis: true})
                    const makePagesToShow = (current, total, maxButtons = 7) => {
                        const pages = [];

                        if (total <= maxButtons) {
                            for (let i = 1; i <= total; i++)
                                pages.push({ page: i, active: i === current });
                            return pages;
                        }

                        // always show first
                        pages.push({ page: 1, active: current === 1 });

                        const windowSize = maxButtons - 2; // reserve spots for first and last
                        let start = Math.max(2, current - Math.floor(windowSize / 2));
                        let end = Math.min(total - 1, start + windowSize - 1);

                        // re-calc start if we're near the end
                        start = Math.max(2, end - windowSize + 1);

                        if (start > 2) {
                            pages.push({ ellipsis: true });
                        }

                        for (let i = start; i <= end; i++) {
                            pages.push({ page: i, active: i === current });
                        }

                        if (end < total - 1) {
                            pages.push({ ellipsis: true });
                        }

                        // always show last
                        pages.push({ page: total, active: current === total });

                        return pages;
                    };

                    const pagesToShow = makePagesToShow(page, totalPages, 6);

                    // prev/next page numbers (null if disabled)
                    const prevPage = page > 1 ? page - 1 : null;
                    const nextPage = page < totalPages ? page + 1 : null;

                    res.render('movies/movies', {
                        title: 'Movies',
                        movies: rows,
                        categories: categoryList || [],
                        ratings: ratingList || [],
                        search,
                        category: categories.join(','),
                        rating: ratings.join(','),
                        sort: sortField,
                        sortDir,
                        page,
                        total,
                        totalPages,
                        limit,

                        // pagination helpers for the template:
                        pagesToShow,
                        pageQueryPrefix, // use: href="{{pageQueryPrefix}}{{this.page}}"
                        prevPage,
                        nextPage,
                    });
                }
            );
        });
    });
});

// Placeholder for creating a new movie (not implemented)
moviesRouter.get('/new', requireStaffAuthWeb, (req, res, next) => {
    res.render('movies/createOrEdit', { title: 'Add New Movie' });
});
moviesRouter.post('/new', requireStaffAuthApi, (req, res, next) => {
    res.json({ success: false, error: 'Not implemented' });
});

// Movie details
moviesRouter.get('/:id', (req, res, next) => {
    const id = parseInt(req.params.id, 10);
    if (!Number.isInteger(id) || id <= 0) {
        return next(new Error('Invalid movie id'));
    }

    fetchMovieById(id, (err, movie) => {
        if (err) return next(err);
        console.log(movie);
        res.render('movies/movie', { title: movie.title, movie });
    });
});

// Placeholder for editing a movie (not implemented)
moviesRouter.get('/:id/edit', requireStaffAuthWeb, (req, res, next) => {
    const movieId = parseInt(req.params.id, 10);
    if (!Number.isInteger(movieId) || movieId <= 0) {
        logger.warn('Edit movie: invalid movie id', { movieId });
        return next(new Error('Invalid movie id'));
    }
    res.render('movies/createOrEdit', { title: 'Edit Movie' });
});
moviesRouter.post('/:id/edit', requireStaffAuthApi, (req, res, next) => {
    res.json({ success: false, error: 'Not implemented' });
});

export default moviesRouter;
