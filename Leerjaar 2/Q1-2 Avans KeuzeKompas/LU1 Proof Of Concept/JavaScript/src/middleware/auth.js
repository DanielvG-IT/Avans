import jwt from 'jsonwebtoken';

const extractAccessToken = (req) => {
    if (req.cookies && req.cookies.accessToken) {
        return req.cookies.accessToken;
    }
    return null;
};

const verifyAccessToken = (token, callback) => {
    jwt.verify(token, process.env.ACCESS_TOKEN_SECRET, callback);
};

export const optionalCustomerAuthWeb = (req, res, next) => {
    const token = extractAccessToken(req);

    if (!token) {
        req.user = null;
        return next();
    }

    verifyAccessToken(token, (error, user) => {
        req.user = error ? null : user;
        next();
    });
};

export const requireCustomerAuthWeb = (req, res, next) => {
    const token = extractAccessToken(req);

    if (!token) {
        return res.redirect('/auth/login');
    }

    // TODO: Check if user is customer

    verifyAccessToken(token, (error, user) => {
        if (error) {
            return res.redirect('/auth/login');
        }
        req.user = user;
        next();
    });
};

export const requireStaffAuthWeb = (req, res, next) => {
    const token = extractAccessToken(req);

    if (!token) {
        return res.redirect('/auth/login');
    }

    // TODO: Check if user is staff

    verifyAccessToken(token, (error, user) => {
        if (error) {
            return res.redirect('/auth/login');
        }
        req.user = user;
        next();
    });
};

export const requireCustomerAuthApi = (req, res, next) => {
    const token = extractAccessToken(req);

    if (!token) {
        return res.status(400).send({ error: 'Authorization token is missing' });
    }

    verifyAccessToken(token, (error, user) => {
        if (error) {
            return res.status(401).send({ error: 'Invalid or expired token' });
        }
        req.user = user;
        next();
    });
};

export const requireStaffAuthApi = (req, res, next) => {
    const token = extractAccessToken(req);

    if (!token) {
        return res.status(400).send({ error: 'Authorization token is missing' });
    }

    verifyAccessToken(token, (error, user) => {
        if (error) {
            return res.status(401).send({ error: 'Invalid or expired token' });
        }
        req.user = user;
        next();
    });
};
