import express from 'express';
import { logger } from '../util/logger.js';
import { fetchMovieById, fetchMovieCount, fetchMovies } from '../services/movieService.js';
import { fetchCategoryNames } from '../services/categoryService.js';

const moviesRouter = express.Router();

moviesRouter.get('/', (req, res, next) => {
    const page = Math.max(parseInt(req.query.page) || 1, 1);
    const limit = parseInt(req.query.limit) || 25;

    const search = (req.query.search || '').trim();
    const category = typeof req.query.category === 'string' ? req.query.category : '';

    // Parse sort param supporting "field,dir", "field:dir" or "field dir"
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

    // Build service filters
    const sortBy = { [sortField]: sortDir };
    const categoryFilter = category ? { category } : {};

    fetchCategoryNames((err, categories) => {
        if (err) return next(err);

        fetchMovies({ page: page - 1, limit, search, sortBy, ...categoryFilter }, (err, movies) => {
            if (err) return next(err);

            fetchMovieCount((err, count) => {
                if (err) return next(err);
                const totalPages = Math.max(Math.ceil(count / limit), 1);

                logger.debug(
                    `Movies page=${page} limit=${limit} search="${search}" category="${category}" sort=${sortField},${sortDir}`
                );

                res.render('movies', {
                    title: 'Movies',
                    movies,
                    categories,
                    search,
                    category,
                    sort: sortField,
                    sortDir,
                    page,
                    totalPages,
                    limit,
                });
            });
        });
    });
});

moviesRouter.get('/:id', (req, res, next) => {
    const { id } = req.params;
    if (!id) return next(new Error('Invalid movie id'));

    const intId = parseInt(id);
    if (intId < 0) return next(new Error('Invalid movie id'));

    logger.debug(`Getting information for movie with ID: ${intId}`);

    fetchMovieById(intId, (err, movie) => {
        if (err) return next(err);

        res.render('movie', { title: movie.title, movie });
    });
});

export default moviesRouter;
