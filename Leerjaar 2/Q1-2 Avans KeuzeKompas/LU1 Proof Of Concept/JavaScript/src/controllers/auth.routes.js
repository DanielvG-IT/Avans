import express from 'express';
import { logger } from '../util/logger.js';
import { requireCustomerAuthApi, requireCustomerAuthWeb } from '../middleware/auth.js';
import {
    register,
    login,
    generateRefreshToken,
    generateAccessToken,
    refreshAccessToken,
    logOut,
    fetchCustomer,
    resetPassword,
} from '../services/authService.js';

const authRouter = express.Router();
const redirectUrl = '/auth/profile';

/**
 * /login - login page
 */
authRouter.get('/login', (req, res, next) => {
    // If user is already authenticated, redirect to dashboard
    if (req.user) {
        return res.redirect(redirectUrl);
    }
    res.render('auth/login', { title: 'Login' });
});
authRouter.post('/login', (req, res) => {
    const email = req.body.email;
    const password = req.body.password;

    logger.debug(`Login attempt for email: ${email}`);

    if (!email || !password) {
        logger.debug('Login failed: Email and password are required.');
        return res.status(400).json({ success: false, error: 'Email and password are required.' });
    }

    login(email, password, (error, user) => {
        if (error) {
            logger.debug(`Login failed for email ${email}: ${error.message}`);
            return res.status(400).json({ success: false, error: error.message });
        }

        const accessToken = generateAccessToken(user);
        generateRefreshToken(user, (error, refreshToken) => {
            if (error) {
                logger.debug(
                    `Refresh token generation failed for email ${email}: ${error.message}`
                );
                return res.status(400).json({ success: false, error: error.message });
            }

            logger.debug(`Login successful for email: ${email}`);
            res.cookie('accessToken', accessToken, {
                httpOnly: true,
                secure: process.env.NODE_ENV !== 'development', // If development then not secure
                sameSite: 'strict',
            });
            res.cookie('refreshToken', refreshToken, {
                httpOnly: true,
                secure: process.env.NODE_ENV !== 'development',
                sameSite: 'strict',
            });
            res.status(200).json({ success: true });
        });
    });
});

/**
 * /register - register page
 */
authRouter.get('/register', (req, res, next) => {
    if (req.user) {
        return res.redirect(redirectUrl);
    }
    res.render('auth/register', { title: 'Register' });
});
authRouter.post('/register', (req, res) => {
    const email = req.body.email;
    const password = req.body.password;

    // TODO Collect more user info (for stalking)

    if (!email || !password) {
        return res.status(400).json({ success: false, error: 'Email and password are required.' });
    }

    register(email, password, (error, result) => {
        if (error) return res.status(400).json({ success: false, error: error.message });

        res.status(201).json({ success: true });
    });
});

/**
 * /profile - profile page
 */
authRouter.get('/profile', requireCustomerAuthWeb, (req, res, next) => {
    fetchCustomer(req.user?.userId, (error, { user, customer }) => {
        if (error) return next(error);
        res.render('auth/profile', { title: 'Profile', user, customer });
    });
});

authRouter.put('/reset-password', requireCustomerAuthApi, (req, res) => {
    const userId = req.user?.userId;
    const newPassword = req.body.newPassword;
    // const oldPassword = req.body.oldPassword;

    if (!newPassword || !userId) {
        return res.status(400).json({
            success: false,
            error: 'User ID and new password are required.',
        });
    }

    // Call the password reset service
    resetPassword(userId, newPassword, (error, result) => {
        if (error) {
            return res.status(400).json({ success: false, error: error.message });
        }
        res.status(200).json({ success: true });
    });
});

/**
 * /logout - logout api endpoint
 */
authRouter.delete('/logout', requireCustomerAuthApi, (req, res) => {
    const userId = req.user?.userId;
    if (!userId) return res.status(401).send({ error: 'User ID not found in request.' });

    logOut(userId, (error, result) => {
        if (error) return res.status(400).json({ success: false, error: error.message });

        if (result.changedRows !== 1) {
            logger.warn(`Logout: No rows updated for userId ${userId}`);
        }

        res.clearCookie('accessToken');
        res.clearCookie('refreshToken');
        res.sendStatus(204);
    });
});

/**
 * /token - token api endpoint
 */
authRouter.post('/token', (req, res) => {
    const refreshToken = req.cookies?.refreshToken;
    if (!refreshToken)
        return res.status(400).send({ success: false, error: 'Refresh token missing' });

    refreshAccessToken(refreshToken, (error, accessToken) => {
        if (error) return res.status(400).send({ error: error });

        res.cookie('accessToken', accessToken, { httpOnly: true, sameSite: 'strict' });
        res.sendStatus(201);
    });
});

export default authRouter;
