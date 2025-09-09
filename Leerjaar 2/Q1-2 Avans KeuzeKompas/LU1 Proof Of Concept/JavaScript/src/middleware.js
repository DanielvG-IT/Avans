import { verify } from 'jsonwebtoken';

const extractToken = (req) => {
    if (req.cookies && req.cookies.accessToken) {
        return req.cookies.accessToken;
    }
    return null;
};

const verifyToken = (token, callback) => {
    verify(token, process.env.ACCESS_TOKEN_SECRET, callback);
};

export const authenticateOptional = (req, res, next) => {
    const token = extractToken(req);

    if (!token) {
        req.user = null;
        return next();
    }

    verifyToken(token, (error, user) => {
        req.user = error ? null : user;
        next();
    });
};

export const authenticateRequired = (req, res, next) => {
    const token = extractToken(req);

    if (!token) {
        return res.redirect('/auth/login');
    }

    verifyToken(token, (error, user) => {
        if (error) {
            return res.redirect('/auth/login');
        }
        req.user = user;
        next();
    });
};

export const authenticateApiRouteRequired = (req, res, next) => {
    const token = extractToken(req);

    if (!token) {
        return res.status(400).send({ error: 'Authorization token is missing' });
    }

    verifyToken(token, (error, user) => {
        if (error) {
            return res.status(401).send({ error: 'Invalid or expired token' });
        }
        req.user = user;
        next();
    });
};
