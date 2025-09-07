import express from 'express';
import { logger } from '../util/logger.js';
import { fetchMovies } from '../services/movieService.js';
import { fetchCategoryNames } from '../services/categoryService.js';

const moviesRouter = express.Router();

moviesRouter.get('/', (req, res, next) => {
    const page = parseInt(req.query.page) - 1 || 0;
    const limit = parseInt(req.query.limit) || 10;
    const search = req.query.search || '';
    let sort = req.query.sort || 'title';
    let genre = req.query.genre || 'All';

    fetchCategoryNames((err, categories) => {
        if (err) {
            next(err);
        }

        // Use the categories to filter or enhance the movie query
        genre === 'All' ? (genre = [...categories]) : (genre = req.query.genre.split(','));
        req.query.sort ? (sort = req.query.sort.split(',')) : (sort = [sort]);

        let sortBy = {};
        if (sort[1]) {
            sortBy[sort[0]] = sort[1];
        } else {
            sortBy[sort[0]] = 'asc';
        }

        fetchMovies({ page, limit, search, sortBy, genre }, (err, movies) => {
            if (err) {
                next(err);
            } else {
                res.json(movies);
            }
        });
    });
});

export default moviesRouter;
