import { requireStaffAuthApi } from '../middleware/auth.js';
import { addRental } from '../services/rentalService.js';
import { logger } from '../util/logger.js';
import express from 'express';

const rentalRouter = express.Router();

rentalRouter.post('/new', requireStaffAuthApi, (req, res, next) => {
    const { movieId, customerId, staffId, storeId } = req.body;
    if (!movieId || !customerId || !staffId || !storeId) {
        return res.status(400).send('Missing required fields');
    }

    addRental(movieId, customerId, staffId, storeId, (err, rental) => {
        if (err) {
            logger.error('Error adding rental:', err);
            return res.status(500).send('Error adding rental');
        }
        res.status(201).send(rental);
    });
});

export default rentalRouter;
