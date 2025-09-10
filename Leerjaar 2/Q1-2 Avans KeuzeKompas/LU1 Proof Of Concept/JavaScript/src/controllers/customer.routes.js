import express from 'express';
import { logger } from '../util/logger.js';
import { requireCustomerAuthWeb } from '../middleware/auth.js';

const customerRouter = express.Router();

customerRouter.get('/', requireCustomerAuthWeb, (req, res, next) => {
    res.json({ test: 'true' });
});

export default customerRouter;
