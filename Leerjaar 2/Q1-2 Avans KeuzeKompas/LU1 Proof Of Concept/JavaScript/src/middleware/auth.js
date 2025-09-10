import jwt from 'jsonwebtoken';
import { refreshAccessToken } from '../services/authService.js';

const extractAccessToken = (req) => {
    if (req.cookies && req.cookies.accessToken) {
        return req.cookies.accessToken;
    }
    return null;
};

const extractRefreshToken = (req) => {
    if (req.cookies && req.cookies.refreshToken) {
        return req.cookies.refreshToken;
    }
    return null;
};

const verifyAccessToken = (token, callback) => {
    jwt.verify(token, process.env.ACCESS_TOKEN_SECRET, callback);
};

/**
 * Optionaly verify customer auth for web routes
 * - Sets user to null when not logged in
 * - Automatically refreshes access token if expired and refresh token is valid
 */
export const optionalCustomerAuthWeb = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    // Access token invalid or expired
    if (!accessToken) {
        req.user = null;
        return next();
    }

    verifyAccessToken(accessToken, (error, user) => {
        // If everything is valid
        if (!error) {
            req.user = user;
            return next();
        }

        // Refresh token invalid or expired
        if (!refreshToken) {
            req.user = null;
            return next();
        }

        // Access Token expired, Refresh token vaild = Get new accessToken
        refreshAccessToken(refreshToken, (error, newAccessToken) => {
            // If refresh went wrong
            if (error || !newAccessToken) {
                req.user = null;
                return next();
            }

            // Save new token in cookies
            res.cookie('accessToken', newAccessToken, { httpOnly: true, sameSite: 'strict' });

            // Verify the new access token to attach user info
            verifyAccessToken(newAccessToken, (verifyErr, newUser) => {
                req.user = verifyErr ? null : newUser;
                return next();
            });
        });
    });
};

/**
 * Require valid customer auth for web routes
 * - Redirects to /auth/login on failure
 * - Automatically refreshes access token if expired and refresh token is valid
 */
export const requireCustomerAuthWeb = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    // No access token at all → force login
    if (!accessToken) {
        return res.redirect('/auth/login');
    }

    // TODO: Check if user is customer after verifying token

    verifyAccessToken(accessToken, (error, user) => {
        if (!error) {
            req.user = user;
            return next();
        }

        // Access token expired → check refresh
        if (!refreshToken) {
            return res.redirect('/auth/login');
        }

        refreshAccessToken(refreshToken, (err, newAccessToken) => {
            if (err || !newAccessToken) {
                return res.redirect('/auth/login');
            }

            // Save new token
            res.cookie('accessToken', newAccessToken, { httpOnly: true, sameSite: 'strict' });

            verifyAccessToken(newAccessToken, (verifyErr, newUser) => {
                if (verifyErr) {
                    return res.redirect('/auth/login');
                }
                req.user = newUser;
                return next();
            });
        });
    });
};

/**
 * Require valid staff auth for web routes
 * - Redirects to /auth/login on failure
 * - Automatically refreshes access token if expired and refresh token is valid
 */
export const requireStaffAuthWeb = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    // No access token → login required
    if (!accessToken) {
        return res.redirect('/auth/login');
    }

    // TODO: Check if user is staff after verifying token

    verifyAccessToken(accessToken, (error, user) => {
        if (!error) {
            req.user = user;
            return next();
        }

        if (!refreshToken) {
            return res.redirect('/auth/login');
        }

        refreshAccessToken(refreshToken, (err, newAccessToken) => {
            if (err || !newAccessToken) {
                return res.redirect('/auth/login');
            }

            res.cookie('accessToken', newAccessToken, { httpOnly: true, sameSite: 'strict' });

            verifyAccessToken(newAccessToken, (verifyErr, newUser) => {
                if (verifyErr) {
                    return res.redirect('/auth/login');
                }
                req.user = newUser;
                return next();
            });
        });
    });
};

/**
 * Require valid customer auth for API routes
 * - Returns 400 if no token
 * - Returns 401 if token invalid/expired and refresh also fails
 * - Automatically refreshes access token if expired and refresh token is valid
 */
export const requireCustomerAuthApi = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    if (!accessToken) {
        return res.status(400).send({ error: 'Authorization token is missing' });
    }

    // TODO: Check if user is customer after verifying token

    verifyAccessToken(accessToken, (error, user) => {
        if (!error) {
            req.user = user;
            return next();
        }

        if (!refreshToken) {
            return res.status(401).send({ error: 'Invalid or expired token' });
        }

        refreshAccessToken(refreshToken, (err, newAccessToken) => {
            if (err || !newAccessToken) {
                return res.status(401).send({ error: 'Invalid or expired refresh token' });
            }

            res.cookie('accessToken', newAccessToken, { httpOnly: true, sameSite: 'strict' });

            verifyAccessToken(newAccessToken, (verifyErr, newUser) => {
                if (verifyErr) {
                    return res.status(401).send({ error: 'Invalid new access token' });
                }
                req.user = newUser;
                return next();
            });
        });
    });
};

/**
 * Require valid staff auth for API routes
 * - Returns 400 if no token
 * - Returns 401 if token invalid/expired and refresh also fails
 * - Automatically refreshes access token if expired and refresh token is valid
 */
export const requireStaffAuthApi = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    if (!accessToken) {
        return res.status(400).send({ error: 'Authorization token is missing' });
    }

    // TODO: Check if user is staff after verifying token

    verifyAccessToken(accessToken, (error, user) => {
        if (!error) {
            req.user = user;
            return next();
        }

        if (!refreshToken) {
            return res.status(401).send({ error: 'Invalid or expired token' });
        }

        refreshAccessToken(refreshToken, (err, newAccessToken) => {
            if (err || !newAccessToken) {
                return res.status(401).send({ error: 'Invalid or expired refresh token' });
            }

            res.cookie('accessToken', newAccessToken, { httpOnly: true, sameSite: 'strict' });

            verifyAccessToken(newAccessToken, (verifyErr, newUser) => {
                if (verifyErr) {
                    return res.status(401).send({ error: 'Invalid new access token' });
                }
                req.user = newUser;
                return next();
            });
        });
    });
};
