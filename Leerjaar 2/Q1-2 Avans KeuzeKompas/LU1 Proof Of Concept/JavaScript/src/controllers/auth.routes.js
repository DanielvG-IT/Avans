import express from 'express';
// import logger from '../utils/logger.js';

const indexRouter = express.Router();

/**
 * GET /login - get login page
 */
indexRouter.get('/login', (req, res, next) => {
    try {
        // If user is already authenticated, redirect to dashboard
        if (req.isAuthenticated()) {
            return res.redirect('/dashboard');
        }
        res.render('login', { title: 'Login', model: {} });
    } catch (err) {
        next(err);
    }
});
indexRouter.post('/login', (req, res, next) => {
    try {
        req.logIn(user, (err) => {
            if (err) {
                return next(err);
            }
            // Handle login logic here
            res.redirect('/dashboard');
        });
    } catch (err) {
        next(err);
    }
});

/**
 * GET /register - get register page
 */
indexRouter.get('/register', (req, res, next) => {
    try {
        // If user is already authenticated, redirect to dashboard
        if (req.isAuthenticated()) {
            return res.redirect('/dashboard');
        }
        res.render('register', { title: 'Register', model: {} });
    } catch (err) {
        next(err);
    }
});
indexRouter.post('/register', (req, res, next) => {
    try {
        // Handle registration logic here
        res.redirect('/login');
    } catch (err) {
        next(err);
    }
});

export default indexRouter;
