import express from 'express';
import { logger } from '../util/logger';
import { verifyToken } from '../services/authService';
import { register, login, generateRefreshToken } from '../services/authService';
import { readUserByRefreshToken, updateUserRefreshTokenByUserId } from '../dao/user';

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
authRouter.post('/login', (req, res, next) => {
    const email = req.body.email;
    const password = req.body.password;

    if (!email || !password) {
        // TODO Handle Error nicely!
        return;
    }

    login(email, password, (error, user) => {
        if (error) return next(error);

        const accessToken = generateAccessToken(user);
        generateRefreshToken(user, (error, refreshToken) => {
            if (error) return next(error);
            console.log(accessToken);
            console.log(refreshToken);

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
            res.redirect(redirectUrl);
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
authRouter.post('/register', (req, res, next) => {
    const email = req.body.email;
    const password = req.body.password;

    if (!email || !password) {
        // TODO Handle Error!
        return;
    }

    register(email, password, (error, result) => {
        if (error) return next(error);

        // TODO Add success feedback to user
        res.redirect('/auth/login');
    });
});

/**
 * /profile - profile page
 */
authRouter.get('/profile', (req, res, next) => {
    res.json({ error: 'Nein' });
});

/**
 * /logout - logout api endpoint
 */
authRouter.delete('/logout', (req, res, next) => {
    const userId = req.user.userId;
    if (!userId) return res.status(401).send({ error: 'User ID not found in request.' });

    updateUserRefreshTokenByUserId(userId, null, (error, result) => {
        if (error) return res.status(500).send({ error: error });

        console.log(result);

        res.clearCookie('accessToken');
        res.clearCookie('refreshToken');
        res.sendStatus(204);
    });
});

/**
 * /token - token api endpoint
 */
authRouter.post('/token', (req, res, next) => {
    const refreshToken = req.cookies?.refreshToken;
    if (!refreshToken) return res.status(400).send({ error: 'Refresh token missing' });

    readUserByRefreshToken(refreshToken, (error, user) => {
        if (error) return res.status(500).send({ error });
        if (!user) return res.status(401).send({ error: 'Invalid refresh token' });

        verify(refreshToken, process.env.REFRESH_TOKEN_SECRET, (error, jwtInfo) => {
            if (error) return res.status(400).send({ error: error });

            const accessToken = generateAccessToken(user);
            res.cookie('accessToken', accessToken, { httpOnly: true, sameSite: 'strict' });
            res.sendStatus(201);
        });
    });
});

export default authRouter;
