import express from 'express';
import { logger } from '../util/logger.js';
import { requireStaffAuthWeb } from '../middleware/auth.js';

const staffRouter = express.Router();

staffRouter.get('/', requireStaffAuthWeb, (req, res, next) => {
    res.render('staff/overview', { title: 'Staff Overview' });
});

export default staffRouter;
