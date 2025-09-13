import express from 'express';
import { logger } from '../util/logger.js';
import { requireStaffAuthWeb } from '../middleware/auth.js';

const staffRouter = express.Router();

staffRouter.get('/', requireStaffAuthWeb, (req, res, next) => {
    res.render('staff/overview', { title: 'Staff Overview' });
});

staffRouter.get('/dashboard', requireStaffAuthWeb, (req, res, next) => {
    res.render('staff/dashboard', { title: 'Staff Dashboard' });
});

staffRouter.get('/crm', requireStaffAuthWeb, (req, res, next) => {
    res.render('staff/crm', { title: 'Customer Relationship Management' });
});

export default staffRouter;
