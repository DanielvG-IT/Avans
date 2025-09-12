import { requireCustomerAuthApi, requireUserAuthWeb } from '../middleware/auth.js';
import { getBase64, getFormatFromMime } from '../util/image.js';
import { fetchAllStores } from '../services/storeService.js';
import { uploadAvatar } from '../middleware/images.js';
import { updateUserAvatarById } from '../dao/user.js';
import { logger } from '../util/logger.js';
import {
    generateRefreshToken,
    generateAccessToken,
    refreshAccessToken,
    changePassword,
    fetchCustomer,
    register,
    logOut,
    login,
} from '../services/authService.js';
import express from 'express';

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
    logger.debug('Rendering registration page, fetching stores...');
    fetchAllStores((err, stores) => {
        if (err) return next(err);
        if (!stores || stores.length === 0) {
            return next(new Error('No stores found. Cannot register without a store.'));
        }
        logger.debug(`Fetched ${stores.length} stores for registration page.`);
        res.render('auth/register', { title: 'Register', stores: stores });
    });
});
authRouter.post('/register', (req, res) => {
    const email = req.body.email;
    const password = req.body.password;
    const confirmPassword = req.body.confirmPassword;
    const firstName = req.body.firstName;
    const lastName = req.body.lastName;
    const address = req.body.address;
    // Accept either 'district' or 'province' from the client
    const district = req.body.district || req.body.province;
    const postalCode = req.body.postalCode;
    const storeId = req.body.storeId ? Number(req.body.storeId) : null;
    const phone = req.body.phone;
    const countryName = req.body.countryName;
    // Accept either 'city' or 'cityName'
    const cityName = req.body.city || req.body.cityName;
    const termsAccepted = req.body.terms === true || req.body.terms === 'true';
    // const location = req.body.location; --- IGNORE ---

    logger.debug(`Registration attempt for email: ${email}`);

    if (
        !email ||
        !password ||
        !firstName ||
        !lastName ||
        !address ||
        !district ||
        !postalCode ||
        !storeId ||
        !phone ||
        !countryName ||
        !cityName ||
        !termsAccepted
    ) {
        return res.status(400).json({ success: false, error: 'All fields are required.' });
    }

    register(
        email,
        password,
        firstName,
        lastName,
        address,
        district,
        postalCode,
        storeId,
        phone,
        countryName,
        cityName,
        (error, result) => {
            if (error) return res.status(400).json({ success: false, error: error.message });

            res.status(201).json({ success: true });
        }
    );
});

/**
 * /profile - profile page
 */
authRouter.get('/profile', requireUserAuthWeb, (req, res, next) => {
    if (req.user.role === 'CUSTOMER') {
        fetchCustomer(req.user?.userId, (error, result) => {
            if (error) return next(error);
            res.render('auth/profile', {
                title: 'Profile',
                user: result.user,
                customer: result.customer,
            });
        });
    } else if (req.user.role === 'STAFF') {
        res.render('auth/profile', { title: 'Profile', user: req.user });
    } else {
        return next(new Error('Unauthorized: Unknown user role.'));
    }
});

authRouter.post(
    '/profile/avatar',
    requireUserAuthWeb,
    uploadAvatar.single('avatar'),
    (req, res) => {
        if (!req.file) return res.status(400).send({ error: 'No file uploaded.' });

        const base64 = getBase64(req.file.buffer);
        const format = getFormatFromMime(req.file.mimetype);

        updateUserAvatarById(req.user.userId, base64, format, (err, result) => {
            if (err) return res.status(500).send({ error: 'Failed to update avatar.' });

            return res.status(200).send({ message: 'Avatar updated successfully.' });
        });
    }
);

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
    changePassword(userId, newPassword, (error, result) => {
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
