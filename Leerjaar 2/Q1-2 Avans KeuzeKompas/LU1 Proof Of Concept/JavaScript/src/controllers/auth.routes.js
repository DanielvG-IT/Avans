import express from 'express';
import jwt from 'jsonwebtoken';
import { logger } from '../util/logger.js';
import { updateUserRefreshTokenByUserId } from '../dao/user.js';
import {
    optionalCustomerAuthWeb,
    requireCustomerAuthApi,
    requireCustomerAuthWeb,
} from '../middleware/auth.js';
import {
    register,
    login,
    generateRefreshToken,
    generateAccessToken,
    refreshAccessToken,
    logOut,
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
    res.render('login', { title: 'Login' });
});
authRouter.post('/login', (req, res) => {
    const email = req.body.email;
    const password = req.body.password;

    if (!email || !password) {
        return res.status(400).json({ success: false, error: 'Email and password are required.' });
    }

    login(email, password, (error, user) => {
        if (error) return res.status(400).json({ success: false, error: error.message });

        const accessToken = generateAccessToken(user);
        generateRefreshToken(user, (error, refreshToken) => {
            if (error) return res.status(400).json({ success: false, error: error.message });

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
    res.render('register', { title: 'Register' });
});
authRouter.post('/register', (req, res) => {
    const email = req.body.email;
    const password = req.body.password;

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
    res.json({ error: 'Nein' });
});

/**
 * /logout - logout api endpoint
 */
authRouter.delete('/logout', requireCustomerAuthApi, (req, res) => {
    const userId = req.user?.userId;
    if (!userId) return res.status(401).send({ error: 'User ID not found in request.' });

    logOut(userId, (error, result) => {
        if (error) return res.status(400).json({ success: false, error: error.message });

        console.log(result);
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
