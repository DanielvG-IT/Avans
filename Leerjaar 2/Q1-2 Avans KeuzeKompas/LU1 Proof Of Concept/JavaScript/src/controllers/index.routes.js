import express from 'express';
import { logger } from '../util/logger.js';
import { fetchPopularFilms } from '../services/filmService.js';

const indexRouter = express.Router();

/**
 * GET / - get homepage single item
 */
indexRouter.get('/', (req, res, next) => {
    try {
        fetchPopularFilms(8, (error, movies) => {
            if (error) {
                return next(error);
            }
            res.json(movies);
            // res.render('index', { title: 'Homepage', model: { movies } });
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
