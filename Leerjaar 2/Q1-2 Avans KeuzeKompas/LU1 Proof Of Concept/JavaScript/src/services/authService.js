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
    phone,
    countryName,
    cityName,
    callback
) => {
    logger.debug(
        `register() called with email=${email}, firstName=${firstName}, lastName=${lastName}, storeId=${storeId}`
    );

    // --------------------------
    // 1. Validate input
    // --------------------------
    if (!email || !password) {
        logger.debug('register() invalid input: missing email or password');
        return callback(new Error('Invalid input.'));
    }

    const passwordRequirements =
        /^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]{6,}$/;
    if (!passwordRequirements.test(password)) {
        logger.debug('register() password does not meet minimal requirements');
        return callback(new Error('Password does not meet minimal requirements.'));
    }

    const normalizedEmail = email.toLowerCase().trim();
    const userId = uuid();
    const passwordHash = hashSync(password, 10);

    logger.debug(`register() normalizedEmail=${normalizedEmail}, generated userId=${userId}`);

    // --------------------------
    // 2. Check if user already exists
    // --------------------------
    readUserByEmail(normalizedEmail, (readUserError, existingUser) => {
        if (readUserError) {
            logger.debug(
                `register() readUserByEmail error for ${normalizedEmail}: ${
                    readUserError.message || readUserError
                }`
            );
            return callback(readUserError);
        }

        if (existingUser) {
            logger.debug(
                `register() user already exists for email ${normalizedEmail}, will check for customer`
            );

            // User exists → check if customer exists
            readCustomerByUserId(existingUser.userId, (customerError, customer) => {
                if (customerError) {
                    logger.debug(
                        `register() readCustomerByUserId error for userId=${existingUser.userId}: ${
                            customerError.message || customerError
                        }`
                    );
                    return callback(customerError);
                }

                if (customer) {
                    logger.debug(
                        `register() customer already exists for userId=${existingUser.userId}`
                    );
                    return callback(new Error('Account already exists for this email.'));
                }

                // User exists but no customer → create customer
                logger.debug(
                    `register() user exists without customer, creating customer for userId=${existingUser.userId}`
                );

                makeAddress(
                    address,
                    district,
                    postalCode,
                    phone,
                    countryName,
                    cityName,
                    (addressError, addressId) => {
                        if (addressError) {
                            logger.debug(
                                `register() makeAddress error for userId=${existingUser.userId}: ${
                                    addressError.message || addressError
                                }`
                            );
                            return callback(addressError);
                        }

                        createCustomer(
                            firstName,
                            lastName,
                            addressId,
                            existingUser.userId,
                            existingUser.emailId,
                            storeId,
                            (customerCreateError) => {
                                if (customerCreateError) {
                                    logger.debug(
                                        `register() createCustomer error for userId=${
                                            existingUser.userId
                                        }: ${customerCreateError.message || customerCreateError}`
                                    );
                                    return callback(customerCreateError);
                                }

                                logger.info(
                                    `Created customer for existing user ${existingUser.userId}`
                                );
                                return callback(null, {
                                    userId: existingUser.userId,
                                    email: normalizedEmail,
                                });
                            }
                        );
                    }
                );
            });

            return;
        }

        // --------------------------
        // 3. User does not exist → check email table
        // --------------------------
        readEmail(normalizedEmail, (readEmailError, rawEmailRows) => {
            if (readEmailError) {
                logger.debug(
                    `register() readEmail error for ${normalizedEmail}: ${
                        readEmailError.message || readEmailError
                    }`
                );
                return callback(readEmailError);
            }

            let emailRow = rawEmailRows && rawEmailRows.length > 0 ? rawEmailRows[0] : null;

            if (!emailRow) {
                logger.debug(
                    `register() email not found, creating new email row for ${normalizedEmail}`
                );
                createEmail(normalizedEmail, (err2, result) => {
                    if (err2) {
                        logger.debug(
                            `register() createEmail error for ${normalizedEmail}: ${
                                err2.message || err2
                            }`
                        );
                        return callback(err2);
                    }
                    const emailId = result.insertId;
                    logger.debug(
                        `register() created email row for ${normalizedEmail}: emailId=${emailId}`
                    );
                    attachUser(emailId);
                });
            } else {
                attachUser(emailRow.email_id);
            }
        });

        // --------------------------
        // 4. Attach user to email / customer
        // --------------------------
        function attachUser(emailId) {
            logger.debug(`attachUser() called with emailId=${emailId} for userId=${userId}`);

            // Check if customer exists with this email
            readCustomerByEmailId(emailId, (readCustomerError, customer) => {
                if (readCustomerError) {
                    logger.debug(
                        `attachUser() readCustomerByEmailId error for emailId=${emailId}: ${
                            readCustomerError.message || readCustomerError
                        }`
                    );
                    return callback(readCustomerError);
                }

                if (customer && customer.userId) {
                    logger.debug(
                        `attachUser() customer already linked to userId=${customer.userId}`
                    );
                    return callback(new Error('Account already exists for this customer.'));
                }

                // Create user if it doesn't exist
                createUser(
                    userId,
                    emailId,
                    passwordHash,
                    'CUSTOMER',
                    null,
                    null,
                    (createUserError) => {
                        if (createUserError) {
                            logger.debug(
                                `attachUser() createUser error for userId=${userId}: ${
                                    createUserError.message || createUserError
                                }`
                            );
                            return callback(createUserError);
                        }

                        logger.debug(
                            `attachUser() created user ${userId}, proceeding with customer creation`
                        );

                        // Create address and customer
                        makeAddress(
                            address,
                            district,
                            postalCode,
                            phone,
                            countryName,
                            cityName,
                            (addressError, { country, city, address }) => {
                                if (addressError) {
                                    logger.debug(
                                        `attachUser() makeAddress error for userId=${userId}: ${
                                            addressError.message || addressError
                                        }`
                                    );
                                    return callback(addressError);
                                }

                                createCustomer(
                                    firstName,
                                    lastName,
                                    address.address_id,
                                    userId,
                                    emailId,
                                    storeId,
                                    (customerCreateError) => {
                                        if (customerCreateError) {
                                            logger.debug(
                                                `attachUser() createCustomer error for userId=${userId}: ${
                                                    customerCreateError.message ||
                                                    customerCreateError
                                                }`
                                            );
                                            return callback(customerCreateError);
                                        }

                                        logger.info(`Created new customer + user ${userId}`);
                                        return callback(null, { userId, email: normalizedEmail });
                                    }
                                );
                            }
                        );
                    }
                );
            });
        }
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
