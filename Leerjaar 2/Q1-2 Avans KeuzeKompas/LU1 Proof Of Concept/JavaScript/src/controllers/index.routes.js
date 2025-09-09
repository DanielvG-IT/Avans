import express from 'express';
import { logger } from '../util/logger.js';
import { fetchPopularMovies } from '../services/movieService.js';

const indexRouter = express.Router();

/**
 * GET / - get homepage single item
 */
indexRouter.get('/', (req, res, next) => {
    logger.debug('GET / - Fetching popular movies');
    fetchPopularMovies(14, (error, result) => {
        if (error) return next(error);

        logger.debug('Fetched popular movies successfully');
        res.render('index', { title: 'Homepage', model: { movies: result } });
    });
});

/**
 * GET /about - get about page
 */
indexRouter.get('/about', (req, res, next) => {
    logger.debug('GET /about - Rendering about page');
    res.render('about', { title: 'About This Project' });
});

export default indexRouter;
