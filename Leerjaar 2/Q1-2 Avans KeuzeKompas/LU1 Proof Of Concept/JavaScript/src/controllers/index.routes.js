import express from 'express';
import { logger } from '../util/logger.js';
import { fetchPopularMovies } from '../services/movieService.js';

const indexRouter = express.Router();

/**
 * GET / - get homepage single item
 */
indexRouter.get('/', (req, res, next) => {
    try {
        fetchPopularMovies(14, (error, result) => {
            if (error) {
                return next(error);
            }
            res.render('index', { title: 'Homepage', model: { movies: result } });
        });
    } catch (err) {
        next(err);
    }
});

/**
 * GET /about - get about page
 */
indexRouter.get('/about', (req, res, next) => {
    try {
        res.render('about', { title: 'About This Project' });
    } catch (err) {
        next(err);
    }
});

export default indexRouter;
