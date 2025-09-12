import { readUserById, readUserByRefreshToken, updateUserRefreshTokenById } from '../dao/user.js';
import { createEmail, readEmail } from '../dao/email.js';
import { compareSync, hashSync } from 'bcrypt';
import { logger } from '../util/logger.js';
import { v4 as uuid } from 'uuid';
import jwt from 'jsonwebtoken';
import {
    readCustomerByEmailId,
    readCustomerByUserId,
    linkCustomerToUser,
    createCustomer,
} from '../dao/customer.js';
import {
    updateUserPasswordById,
    updateUserAvatarById,
    readUserByEmail,
    createUser,
} from '../dao/user.js';
import { makeAddress } from './addressService.js';

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

export const register = (
    email,
    password,
    firstName,
    lastName,
    address,
    district,
    postalCode,
    storeId,
    callback
) => {
    if (!email || !password) return callback(new Error('Invalid input.'));

    const passwordRequirements =
        /^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]{6,}$/;
    if (!passwordRequirements.test(password)) {
        return callback(new Error('Password does not meet minimal requirements.'));
    }

    const normalizedEmail = email.toLowerCase().trim();
    const userId = uuid();
    const passwordHash = hashSync(password, 10);

    // 1. Check email table
    readEmail(normalizedEmail, (err, emailRow) => {
        if (err) return callback(err);

        if (!emailRow) {
            // Email does not exist → create one
            createEmail(normalizedEmail, (err2, newEmailRow) => {
                if (err2) return callback(err2);
                attachUser(newEmailRow.email_id);
            });
        } else {
            attachUser(emailRow.email_id);
        }
    });

    function attachUser(emailId) {
        // 2. Check if customer already exists with this email
        readCustomerByEmailId(emailId, (err, customer) => {
            if (err) return callback(err);

            if (customer) {
                if (customer.userId) {
                    return callback(new Error('Account already exists for this customer.'));
                }

                // Customer exists but no user → create user and link
                createUser(userId, emailId, passwordHash, 'CUSTOMER', null, null, (err2) => {
                    if (err2) return callback(err2);

                    linkCustomerToUser(customer.customer_id, userId, (err3) => {
                        if (err3) return callback(err3);
                        logger.info(
                            `Linked existing customer ${customer.customer_id} to new user ${userId}`
                        );
                        return callback(null, { userId, email: normalizedEmail });
                    });
                });
            } else {
                // No customer → create new customer + user
                createUser(userId, emailId, passwordHash, 'CUSTOMER', null, null, (err2) => {
                    if (err2) return callback(err2);

                    makeAddress(address, district, postalCode, (err3, addressId) => {
                        if (err3) return callback(err3);

                        createCustomer(
                            firstName,
                            lastName,
                            addressId,
                            userId,
                            emailId,
                            storeId,
                            (err4) => {
                                if (err4) return callback(err4);
                                logger.info(`Created new customer + user ${userId}`);
                                return callback(null, { userId, email: normalizedEmail });
                            }
                        );
                    });
                });
            }
        });
    }
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
    updateUserRefreshTokenById(userId, null, (error, result) => {
        if (error) return callback(error);
        else return callback(null, result);
    });
};

export const fetchCustomer = (userId, callback) => {
    readUserById(userId, (error, user) => {
        if (error) return callback(error);
        if (!user) return callback(new Error('User not found.'));
        if (user.role !== 'CUSTOMER') return callback(new Error('User is not a customer.'));

        readCustomerByUserId(userId, (error, customer) => {
            if (error) return callback(error);
            if (!customer) return callback(new Error('Customer not found.'));

            const result = { user, customer };
            return callback(null, result);
        });
    });
};

// export const fetchStaff = (userId, callback) => {
//     readUserById(userId, (error, user) => {
//         if (error) return callback(error);
//         if (!user) return callback(new Error('User not found.'));
//         if (user.role !== 'STAFF') return callback(new Error('User is not staff.'));

//         readStaffByUserId(userId, (error, staff) => {
//             if (error) return callback(error);
//             return callback(null, staff);
//         });
//     });
// };

export const updateAvatar = (userId, avatarBase64, avatarFormat, callback) => {
    if (!userId || !avatarBase64 || !avatarFormat) {
        return callback(new Error('User ID, avatar image and format are required.'));
    }
    updateUserAvatarById(userId, avatarBase64, avatarFormat, (error, result) => {
        if (error) return callback(error);
        if (result.affectedRows === 0) {
            logger.warn(`No rows updated for userId ${userId} when updating avatar`);
            return callback(new Error('Avatar update failed.'));
        }
        return callback(null, result);
    });
};

export const changePassword = (userId, newPassword, callback) => {
    if (!userId || !newPassword) {
        return callback(new Error('User ID and new password are required.'));
    }

    // const passwordsMatch = compareSync(oldPassword, user.passwordHash);
    const passwordHash = hashSync(newPassword, 10);
    updateUserPasswordById(userId, passwordHash, (error, result) => {
        if (error) return callback(error);
        if (result.affectedRows === 0) {
            logger.warn(`No rows updated for userId ${userId} when changing password`);
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
            logger.warn(`No rows updated for userId ${user.userId} when setting refresh token`);
        }

        return callback(null, refreshToken);
    });
};
