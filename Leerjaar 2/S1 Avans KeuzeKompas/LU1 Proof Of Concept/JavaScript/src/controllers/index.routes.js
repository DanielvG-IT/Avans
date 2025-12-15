import express from 'express';
import { logger } from '../util/logger.js';
import {
    fetchCheapestMovies,
    fetchLongestMovies,
    fetchPopularMovies,
} from '../services/movieService.js';

const indexRouter = express.Router();

/**
 * GET / - get homepage single item
 */
indexRouter.get('/', (req, res, next) => {
    logger.debug('GET / - Fetching popular movies');
    fetchPopularMovies(12, (error, popularMovies) => {
        if (error) return next(error);

        fetchLongestMovies(12, (error, longestMovies) => {
            if (error) return next(error);

            fetchCheapestMovies(12, (error, cheapestMovies) => {
                if (error) return next(error);

                res.render('index', {
                    title: 'Home',
                    popularMovies,
                    longestMovies,
                    cheapestMovies,
                });
            });
        });
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
