import { createUser, readUserByEmail, updateUserPasswordById } from '../dao/user.js';
import { readCustomerByUserId } from '../dao/customer.js';
import { compareSync, hashSync } from 'bcrypt';
import { logger } from '../util/logger.js';
import { v4 as uuid } from 'uuid';
import jwt from 'jsonwebtoken';
import {
    readUserById,
    readUserByRefreshToken,
    updateUserRefreshTokenByUserId,
} from '../dao/user.js';

export const login = (email, password, callback) => {
    if (!email || !password) {
        return callback(new Error('Invalid input.'));
    }

    readUserByEmail(email, (error, user) => {
        if (error) {
            logger.error('Category Error:', error);
            return callback(error);
        }

        if (!user) {
            return callback(new Error('Incorrect email or password.'));
        }

        const correctPassword = compareSync(password, user.passwordHash);
        if (correctPassword) {
            return callback(null, user);
        } else {
            return callback(new Error('Incorrect email or password.'));
        }
    });
};

export const register = (email, password, callback) => {
    if (!email || !password) {
        return callback(new Error('Invalid input.'));
    }

    // Minimal requirements: At least 6 characters, one letter, one number, one special character
    const passwordRequirements =
        /^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]{6,}$/;
    if (!passwordRequirements.test(password)) {
        // ERROR: Password does not meet minimal requirements
        return callback(new Error('Password does not meet minimal requirements.'));
    }

    const userId = uuid();
    const passwordHash = hashSync(password, 10);

    createUser(userId, email, passwordHash, 'CUSTOMER', callback);
};

export const refreshAccessToken = (refreshToken, callback) => {
    readUserByRefreshToken(refreshToken, (error, user) => {
        if (error) return callback(error);
        if (!user) return callback(new Error('Invalid refresh token'));

        jwt.verify(refreshToken, process.env.REFRESH_TOKEN_SECRET, (error) => {
            if (error) return callback(error);

            const accessToken = generateAccessToken(user);
            return callback(null, accessToken);
        });
    });
};

export const logOut = (userId, callback) => {
    updateUserRefreshTokenByUserId(userId, null, (error, result) => {
        if (error) return callback(error);
        else return callback(null, result);
    });
};

export const fetchCustomer = (userId, callback) => {
    readUserById(userId, (error, user) => {
        if (error) return callback(error);
        if (!user) return callback(new Error('User not found.'));

        readCustomerByUserId(userId, (error, customer) => {
            if (error) return callback(error);
            if (!customer) return callback(new Error('Customer not found.'));
            return callback(null, { user, customer });
        });
    });
};

// export const fetchStaff = (userId, callback) => {
//     readUserById(userId, (error, user) => {
//         if (error) return callback(error);
//         if (!user) return callback(new Error('User not found.'));

//         readStaffByUserId(userId, (error, staff) => {
//             if (error) return callback(error);
//             return callback(null, staff);
//         });
//     });
// };

export const resetPassword = (userId, newPassword, callback) => {
    if (!userId || !newPassword) {
        return callback(new Error('User ID and new password are required.'));
    }

    // const passwordsMatch = compareSync(oldPassword, user.passwordHash);
    const passwordHash = hashSync(newPassword, 10);
    updateUserPasswordById(userId, passwordHash, (error, result) => {
        if (error) return callback(error);
        if (result.affectedRows === 0) {
            logger.warn(`No rows updated for userId ${userId}`);
            return callback(new Error('Password update failed.'));
        }

        logger.info(`Password updated for userId ${userId}`);
        return callback(null, result);
    });
};

// JWT Functions
export const generateAccessToken = (user) => {
    const payload = { userId: user.userId, role: user.role };
    return jwt.sign(payload, process.env.ACCESS_TOKEN_SECRET, { expiresIn: '10m' });
};

export const generateRefreshToken = (user, callback) => {
    const payload = { userId: user.userId, role: user.role };
    const refreshToken = jwt.sign(payload, process.env.REFRESH_TOKEN_SECRET);
    updateUserRefreshTokenByUserId(user.userId, refreshToken, (error, result) => {
        if (error) {
            logger.error('MySQL Error:', error);
            return callback(error);
        }

        if (result.affectedRows === 0) {
            logger.warn(`No rows updated for userId ${user.userId}`);
        }

        return callback(null, refreshToken);
    });
};
