import jwt from 'jsonwebtoken';
import { refreshAccessToken } from '../services/authService.js';

/**
 * Extracts access token from request cookies (if available).
 * Returns `null` if not found.
 */
const extractAccessToken = (req) => {
    if (req.cookies && req.cookies.accessToken) {
        return req.cookies.accessToken;
    }
    return null;
};

/**
 * Extracts refresh token from request cookies (if available).
 * Returns `null` if not found.
 */
const extractRefreshToken = (req) => {
    if (req.cookies && req.cookies.refreshToken) {
        return req.cookies.refreshToken;
    }
    return null;
};

/**
 * Wrapper for jwt.verify to check access token validity.
 * Calls the provided callback with `(error, decodedUser)`.
 */
const verifyAccessToken = (token, callback) => {
    jwt.verify(token, process.env.ACCESS_TOKEN_SECRET, callback);
};

/**
 * Middleware: Optionally attach authenticated customer info for web routes.
 *
 * Behavior:
 * - If no access token → `req.user = null` (continue anonymously).
 * - If access token is valid → attach decoded user to `req.user`.
 * - If access token is expired but refresh token is valid → issue new access token,
 *   set it in cookies, and attach decoded user to `req.user`.
 * - If all checks fail → `req.user = null`.
 *
 * Note:
 * - Never blocks the request. Caller must handle `req.user === null`.
 */
export const optionalCustomerAuthWeb = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    // No access token → treat as unauthenticated, but allow access
    if (!accessToken) {
        req.user = null;
        return next();
    }

    // Validate current access token
    verifyAccessToken(accessToken, (error, user) => {
        if (!error) {
            // Access token valid → attach user
            req.user = user;
            return next();
        }

        // Access token invalid/expired and no refresh token → unauthenticated
        if (!refreshToken) {
            req.user = null;
            return next();
        }

        // Try refreshing access token using refresh token
        refreshAccessToken(refreshToken, (error, newAccessToken) => {
            if (error || !newAccessToken) {
                // Refresh failed → unauthenticated
                req.user = null;
                return next();
            }

            // Store new access token in cookie (secure + strict)
            res.cookie('accessToken', newAccessToken, { httpOnly: true, sameSite: 'strict' });

            // Verify the new token before trusting it
            verifyAccessToken(newAccessToken, (verifyErr, newUser) => {
                req.user = verifyErr ? null : newUser;
                return next();
            });
        });
    });
};

/**
 * Middleware: Require valid customer authentication for web routes.
 *
 * Behavior:
 * - If no/invalid access token → redirect to login page.
 * - If access token is expired but refresh token is valid → issue new token,
 *   set cookie, and attach user.
 * - Otherwise → force login.
 *
 * TODO:
 * - Add check to ensure user role = customer.
 */
export const requireCustomerAuthWeb = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    if (!accessToken) {
        return res.redirect('/auth/login');
    }

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
 * Middleware: Require valid staff authentication for web routes.
 *
 * Same logic as `requireCustomerAuthWeb`, but intended for staff accounts.
 *
 * TODO:
 * - Add role/permissions check to enforce staff-only access.
 */
export const requireStaffAuthWeb = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    if (!accessToken) {
        return res.redirect('/auth/login');
    }

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
 * Middleware: Require valid customer authentication for API routes.
 *
 * Behavior:
 * - If no access token → respond with 400 (bad request).
 * - If access token is valid → attach user.
 * - If access token expired but refresh is valid → issue new token and attach user.
 * - If all checks fail → respond with 401 (unauthorized).
 *
 * TODO:
 * - Add customer role check.
 */
export const requireCustomerAuthApi = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    if (!accessToken) {
        return res.status(400).send({ error: 'Authorization token is missing' });
    }

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
 * Middleware: Require valid staff authentication for API routes.
 *
 * Same as `requireCustomerAuthApi`, but intended for staff accounts.
 *
 * TODO:
 * - Enforce staff role check.
 */
export const requireStaffAuthApi = (req, res, next) => {
    const accessToken = extractAccessToken(req);
    const refreshToken = extractRefreshToken(req);

    if (!accessToken) {
        return res.status(400).send({ error: 'Authorization token is missing' });
    }

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
