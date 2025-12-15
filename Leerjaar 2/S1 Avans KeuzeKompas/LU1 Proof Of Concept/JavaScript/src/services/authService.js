import { createEmail, readEmail } from '../dao/email.js';
import { makeAddress } from './addressService.js';
import { compareSync, hashSync } from 'bcrypt';
import { logger } from '../util/logger.js';
import { v4 as uuid } from 'uuid';
import jwt from 'jsonwebtoken';
import {
    updateUserPasswordById,
    updateUserAvatarById,
    readUserByEmail,
    createUser,
} from '../dao/user.js';
import {
    updateUserRefreshTokenById,
    readUserByRefreshToken,
    deleteUserById,
    readUserById,
} from '../dao/user.js';
import {
    unlinkCustomerFromUser,
    readCustomerByEmailId,
    readCustomerByUserId,
    linkCustomerToUser,
    softDeleteCustomer,
    createCustomer,
} from '../dao/customer.js';
import { readStaffByUserId } from '../dao/staff.js';

// TODO : move to util
const onceCallback = (cb) => {
    let called = false;
    return (err, res) => {
        if (called) return;
        called = true;
        try {
            if (typeof cb === 'function') cb(err, res);
        } catch (e) {
            logger.error('Callback threw an error:', e);
        }
    };
};

export const login = (email, password, callback) => {
    if (!email || !password) {
        return callback(new Error('Invalid input.'));
    }

    logger.debug(`login() called with email=${email}`);
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
    phone,
    countryName,
    cityName,
    callback
) => {
    const cb = onceCallback(callback);

    if (!email || !password) return cb(new Error('Email and password required.'));

    const passwordRequirements =
        /^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]{6,}$/;
    if (!passwordRequirements.test(password))
        return cb(new Error('Password does not meet requirements.'));

    const normalizedEmail = email.toLowerCase().trim();
    const userId = uuid();
    const passwordHash = hashSync(password, 10);

    // Helper: proceed to create/link user & customer
    const proceedCreateOrLinkUser = (emailId, customerToLink) => {
        createUser(userId, emailId, passwordHash, 'CUSTOMER', null, null, (err) => {
            if (err) return cb(err);

            if (customerToLink) {
                // Link existing customer to new user
                linkCustomerToUser(customerToLink.customer_id, userId, (linkErr) => {
                    if (linkErr) return cb(linkErr);
                    logger.debug(
                        `Linked existing customer ${customerToLink.customer_id} to new user ${userId}`
                    );
                    cb(null, { userId, email: normalizedEmail });
                });
            } else {
                // No customer exists → create address & customer
                makeAddress(
                    address,
                    district,
                    postalCode,
                    phone,
                    countryName,
                    cityName,
                    (addrErr, addrObj) => {
                        if (addrErr) return cb(addrErr);

                        createCustomer(
                            firstName,
                            lastName,
                            addrObj.address_id,
                            userId,
                            emailId,
                            storeId,
                            (createCustErr) => {
                                if (createCustErr) return cb(createCustErr);
                                logger.debug(`Created new user + customer ${userId}`);
                                cb(null, { userId, email: normalizedEmail });
                            }
                        );
                    }
                );
            }
        });
    };

    // 1️⃣ Check if user exists
    readUserByEmail(normalizedEmail, (err, user) => {
        if (err) return cb(err);

        if (user) {
            // User exists → check if customer exists
            return readCustomerByUserId(user.userId, (custErr, customer) => {
                if (custErr) return cb(custErr);
                if (customer) return cb(new Error('Account already exists for this email.'));

                // User exists but no customer → create address & customer
                makeAddress(
                    address,
                    district,
                    postalCode,
                    phone,
                    countryName,
                    cityName,
                    (addrErr, addrObj) => {
                        if (addrErr) return cb(addrErr);

                        createCustomer(
                            firstName,
                            lastName,
                            addrObj.address_id,
                            user.userId,
                            user.emailId,
                            storeId,
                            (createErr) => {
                                if (createErr) return cb(createErr);
                                logger.debug(`Created customer for existing user ${user.userId}`);
                                cb(null, { userId: user.userId, email: normalizedEmail });
                            }
                        );
                    }
                );
            });
        }

        // 2️⃣ User does NOT exist → resolve email ID first
        readEmail(normalizedEmail, (emailErr, emailRow) => {
            if (emailErr) return cb(emailErr);

            const emailId = emailRow ? emailRow.email_id : null;

            const handleCustomerCheck = (emailIdToUse) => {
                readCustomerByEmailId(emailIdToUse, (custErr, existingCustomer) => {
                    if (custErr) return cb(custErr);
                    proceedCreateOrLinkUser(emailIdToUse, existingCustomer);
                });
            };

            if (!emailRow) {
                // Email doesn't exist → create it
                createEmail(normalizedEmail, (createEmailErr, result) => {
                    if (createEmailErr) return cb(createEmailErr);
                    handleCustomerCheck(result.insertId);
                });
            } else {
                handleCustomerCheck(emailId);
            }
        });
    });
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

export const fetchUser = (userId, callback) => {
    readUserById(userId, (error, user) => {
        if (error) return callback(error);
        if (!user) return callback(new Error('User not found.'));
        return callback(null, user);
    });
};

export const fetchCustomer = (userId, callback) => {
    fetchUser(userId, (error, user) => {
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

export const fetchStaff = (userId, callback) => {
    fetchUser(userId, (error, user) => {
        if (error) return callback(error);
        if (!user) return callback(new Error('User not found.'));
        if (user.role !== 'STAFF') return callback(new Error('User is not staff.'));

        logger.debug('Fetching staff for userId:', userId);

        readStaffByUserId(userId, (error, staff) => {
            if (error) return callback(error);
            return callback(null, staff);
        });
    });
};

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

        logger.debug(`Password updated for userId ${userId}`);
        return callback(null, result);
    });
};

export const deleteAccount = (userId, callback) => {
    if (!userId) {
        return callback(new Error('User ID is required.'));
    }

    readUserById(userId, (error, user) => {
        if (error) return callback(error);
        if (!user) return callback(new Error('User not found.'));
        if (user.role !== 'CUSTOMER')
            return callback(new Error('Only customers can delete accounts.'));

        readCustomerByUserId(userId, (customerError, customer) => {
            if (customerError) return callback(customerError);
            if (!customer) return callback(new Error('Customer not found.'));

            unlinkCustomerFromUser(customer.customer_id, (unlinkError, result) => {
                if (unlinkError || result.affectedRows === 0) {
                    logger.error('Error occurred while unlinking customer from user:', unlinkError);
                }

                // Perform the hard delete of the user
                deleteUserById(userId, (error, result) => {
                    if (error) return callback(error);
                    if (result.affectedRows === 0) {
                        logger.warn(`No rows deleted for userId ${userId} when deleting account`);
                        return callback(new Error('Account deletion failed.'));
                    }

                    softDeleteCustomer(customer.customer_id, (softDeleteError, result) => {
                        if (softDeleteError) {
                            logger.error(
                                'Error occurred while soft deleting customer:',
                                softDeleteError
                            );
                        }
                        if (result.affectedRows === 0) {
                            logger.warn(
                                `No customer rows soft deleted for userId ${userId} when deleting account`
                            );
                        }
                        return callback(null, result);
                    });
                });
            });
        });
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
    updateUserRefreshTokenById(user.userId, refreshToken, (error, result) => {
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
