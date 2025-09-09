import { createUser, readUserByEmail, readUserById } from '../dao/user.js';
import { compareSync, hashSync } from 'bcrypt';
import { logger } from '../util/logger.js';
import { v4 as uuid } from 'uuid';

export const login = (email, password, callback) => {
    if (!email || !password) {
        return callback(new Error('Invalid input.'));
    }

    readUserByEmail(email, (error, user) => {
        if (error) {
            logger.error('Category Error:', error);
            return callback(error);
        }

        const correctPassword = compareSync(password, user.passwordHash);
        if (correctPassword) {
            callback(null, user);
        } else {
            return callback(new Error('Incorrect email or password.'));
        }
    });
};

export const register = (email, password, callback) => {
    if (!email || !password) {
        return callback(new Error('Invalid input.'));
    }

    // Minimal requirements: at least 6 characters, one letter, one number
    const passwordRequirements = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$/;
    if (!passwordRequirements.test(password)) {
        // ERROR: Password does not meet minimal requirements
        return callback(new Error('Password does not meet minimal requirements.'));
    }

    const userId = uuid();
    const passwordHash = hashSync(password, 10);

    createUser(userId, email, passwordHash, 'CUSTOMER', callback);
};

// JWT Functions
export const generateAccessToken = (user) => {
    const payload = { userId: user.userId, role: user.role };
    return sign(payload, process.env.ACCESS_TOKEN_SECRET, { expiresIn: '10m' });
};

export const generateRefreshToken = (user, callback) => {
    const payload = { userId: user.userId, role: user.role };
    const refreshToken = sign(payload, process.env.REFRESH_TOKEN_SECRET);
    updateUserRefreshTokenByUserId(user.userId, refreshToken, (error, result) => {
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }
        // TODO Check what is in result
        return callback(null, refreshToken);
    });
};
