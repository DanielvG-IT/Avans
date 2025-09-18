import express from 'express';
import { logger } from '../util/logger.js';
import { requireCustomerAuthWeb } from '../middleware/auth.js';
import { fetchCustomerById, fetchRentalsByCustomerId } from '../services/customerService.js';
import { readCustomerByUserId } from '../dao/customer.js';
import { fetchUser } from '../services/authService.js';

const customerRouter = express.Router();

customerRouter.get('/', requireCustomerAuthWeb, (req, res, next) => {
    readCustomerByUserId(req.user.userId, (err, customer) => {
        if (err) {
            logger.error('Error fetching customer by user ID:', err);
            return next(err);
        }
        if (!customer) {
            return next(new Error('Customer not found'));
        }

        fetchCustomerById(customer.customer_id, (error, customer) => {
            if (error) {
                logger.error('Customer Error:', error);
                return next(error);
            }
            if (!customer) {
                logger.warn('Customer not found:', { customerId });
                return next(new Error('Customer not found'));
            }

            fetchRentalsByCustomerId(customer.customer_id, (rentalError, rentals) => {
                if (rentalError) {
                    logger.error('Rental Retrieval Error:', rentalError);
                    return next(rentalError);
                }

                // Robust categorization and improved metrics (single-pass, defensive)
                const now = new Date();
                const parseDate = (d) => {
                    if (!d) return null;
                    const dd = new Date(d);
                    return isNaN(dd.getTime()) ? null : dd;
                };
                const addDays = (date, days) =>
                    new Date(date.getTime() + days * 24 * 60 * 60 * 1000);

                const rentalsList = Array.isArray(rentals) ? rentals : [];

                const activeRentals = [];
                const futureRentals = [];
                const pastRentals = [];
                let overdueCount = 0;

                for (let i = 0; i < rentalsList.length; i++) {
                    const r = rentalsList[i];

                    const rentalDate = parseDate(r.rental_date);
                    const returnDate = parseDate(r.return_date);

                    // Normalize duration once
                    const durationDays = Number(r.rental_duration) || 0;
                    const normalizedDuration = durationDays > 0 ? Math.floor(durationDays) : 0;
                    r.rental_duration = normalizedDuration;

                    // Normalize numeric/monetary fields
                    r.amount =
                        r.amount !== undefined
                            ? Number(r.amount)
                            : r.rental_rate
                            ? Number(r.rental_rate)
                            : 0;
                    r.rental_rate = r.rental_rate !== undefined ? Number(r.rental_rate) : 0;

                    r.due_date = rentalDate ? addDays(rentalDate, normalizedDuration) : null;
                    r.isReturned = !!returnDate;
                    r.isOverdue = !r.isReturned && r.due_date ? now > r.due_date : false;

                    // Skip invalid rental_date entries
                    if (!rentalDate) continue;

                    // Categorize
                    if (r.rental_status === 'Future' || (rentalDate && rentalDate > now)) {
                        r.rental_status = 'Future';
                        futureRentals.push(r);
                    } else if (r.rental_status === 'Past' || (returnDate && returnDate <= now)) {
                        pastRentals.push(r);
                    } else {
                        // started and not returned, or return in future
                        activeRentals.push(r);
                    }

                    if (r.isOverdue) overdueCount++;
                }

                // Aggregate metrics
                const metrics = {
                    totalRentals: rentalsList.length,
                    activeRentalsCount: activeRentals.length,
                    overdueRentalsCount: overdueCount,
                };

                fetchUser(req.user.userId, (error, user) => {
                    if (error) {
                        logger.error('User Fetch Error:', error);
                        return next(error);
                    }
                    if (!user) {
                        logger.warn('User not found for userId:', req.user.userId);
                        return next(new Error('User not found'));
                    }
                    req.user = user; // Attach full user details

                    res.render('viewCustomer', {
                        title: 'Customer Details',
                        user: req.user,
                        customer,
                        metrics,
                        activeRentals,
                        futureRentals,
                        pastRentals,
                        returnUrl: '/customer',
                    });
                });
            });
        });
    });
});

export default customerRouter;
