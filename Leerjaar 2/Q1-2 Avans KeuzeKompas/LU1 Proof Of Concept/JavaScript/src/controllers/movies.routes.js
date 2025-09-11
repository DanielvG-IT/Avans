import express from 'express';
import { fetchCategoryNames, fetchRatingNames } from '../services/filterService.js';
import { fetchMovieById, fetchMovies } from '../services/movieService.js';

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
                        totalPages,
                        limit,
                    });
                }
            );
        });
    });
});

moviesRouter.get('/:id', (req, res, next) => {
    const id = parseInt(req.params.id, 10);
    if (!Number.isInteger(id) || id <= 0) {
        return next(new Error('Invalid movie id'));
    }

    fetchMovieById(id, (err, movie) => {
        if (err) return next(err);

        res.render('movies/movie', { title: movie.title, movie });
    });
});

export default moviesRouter;
