import express from 'express';
// import logger from '../util/logger';

const indexRouter = express.Router();

/**
 * GET / - get homepage single item
 */
indexRouter.get('/', (req, res, next) => {
    try {
        res.render('index', { title: 'Homepage', model: { movies: [] } });
    } catch (err) {
        next(err);
    }
});

export default indexRouter;
