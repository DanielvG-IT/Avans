import { readCustomerRentalHistory } from '../dao/rental.js';
import { createEmail, readEmail } from '../dao/email.js';
import { makeAddress } from './addressService.js';
import { readAddress } from '../dao/address.js';
import { logger } from '../util/logger.js';
import {
    softDeleteCustomer,
    readCustomerById,
    updateCustomer,
    createCustomer,
    readCustomers,
    readCustomerByUserId,
} from '../dao/customer.js';

/**
 * Ensure a callback is only invoked once.
 */
const onceCallback = (cb) => {
    let called = false;
    return (err, res) => {
        if (called) return;
        called = true;
        try {
            if (typeof cb === 'function') cb(err, res);
        } catch (e) {
            // Guard against exceptions thrown by the caller's callback
            logger.error('Service callback threw an error:', e);
        }
    };
};

export const addCustomer = (data, callback) => {
    const cb = onceCallback(callback);
    // Validate and sanitize input data
    const {
        firstName,
        lastName,
        email,
        address,
        province,
        postalCode,
        phone,
        city,
        country,
        active,
        storeId = 1, // Default storeId if not provided
    } = data || {};
    const newData = {
        firstName: String(firstName).trim(),
        lastName: String(lastName).trim(),
        email: String(email).trim(),
        address: String(address).trim(),
        province: String(province).trim(),
        postalCode: String(postalCode).trim(),
        phone: String(phone).trim(),
        city: String(city).trim(),
        country: String(country).trim(),
        active: Boolean(active),
    };

    makeAddress(
        newData.address,
        newData.province,
        newData.postalCode,
        newData.phone,
        newData.country,
        newData.city,
        (err, { country, city, address } = {}) => {
            if (err) {
                logger.error('Address Creation Error:', err);
                return cb(err);
            }

            const proceedWithCustomerCreation = (emailId) => {
                const userId = null; // No user linked initially
                createCustomer(
                    firstName,
                    lastName,
                    address.address_id,
                    userId,
                    emailId,
                    storeId,
                    (error, result) => {
                        if (error) {
                            logger.error('Customer Creation Error:', error);
                            return cb(error);
                        }
                        cb(null, result.insertId);
                    }
                );
            };

            readEmail(newData.email, (error, emailRow) => {
                if (error) {
                    logger.error('Email Lookup Error:', error);
                    return cb(error);
                }
                const emailId = emailRow?.email_id;

                if (!emailId) {
                    createEmail(newData.email, (emailErr, newEmailRow) => {
                        if (emailErr) {
                            logger.error('Email Creation Error:', emailErr);
                            return cb(emailErr);
                        }
                        proceedWithCustomerCreation(newEmailRow.insertId);
                    });
                } else {
                    proceedWithCustomerCreation(emailId);
                }
            });
        }
    );
};

export const fetchCustomers = (params = {}, callback) => {
    const cb = onceCallback(callback);
    if (typeof params !== 'object' || params === null) {
        logger.warn('fetchCustomers called with invalid params:', params);
        return cb(new Error('Invalid parameters'));
    }
    const { search, active, storeId, sort, page, limit } = params;
    readCustomers({ search, active, storeId, sort, page, pageSize: limit }, (error, result) => {
        if (error) {
            logger.error('Customer Error:', error);
            return cb(error);
        }
        const { total, page, pageSize, totalPages, customers } = result || {};

        const mapped = Array.isArray(customers)
            ? customers.map((r) => {
                  const firstName = r.first_name ?? r.firstName ?? '';
                  const lastName = r.last_name ?? r.lastName ?? '';
                  const name = [firstName, lastName]
                      .map((s) => String(s).trim())
                      .filter(Boolean)
                      .join(' ');
                  const active = r.active ?? 0;
                  const toIso = (v) => (v instanceof Date ? v.toISOString() : v ?? null);

                  return {
                      customerId: r.customer_id ?? r.customerId ?? null,
                      store_name: r.store_name ?? null,
                      firstName,
                      lastName,
                      name,
                      email: r.email ?? null,
                      address: r.address ?? null,
                      postalCode: r.postal_code ?? r.postalCode ?? null,
                      phone: r.phone ?? null,
                      city: r.city ?? null,
                      country: r.country ?? null,
                      active,
                      createDate: toIso(r.create_date ?? r.createDate),
                      lastUpdate: toIso(r.last_update ?? r.lastUpdate),
                      hasUserLinked: r.userId ?? true ?? false,
                  };
              })
            : [];
        cb(null, { total, page, pageSize, totalPages, customers: mapped });
    });
};

export const fetchCustomerById = (customerId, callback) => {
    const cb = onceCallback(callback);
    if (
        customerId === null ||
        customerId === undefined ||
        isNaN(Number(customerId)) ||
        Number(customerId) <= 0
    ) {
        logger.warn('fetchCustomerById called with invalid customerId:', customerId);
        return cb(new Error('Invalid customerId'));
    }

    readCustomerById(customerId, (error, result) => {
        if (error) {
            logger.error('Customer Error:', error);
            return cb(error);
        }
        if (!result) {
            return cb(new Error('Customer not found'));
        }
        cb(null, result);
    });
};

export const fetchCustomerByUserId = (userId, callback) => {
    const cb = onceCallback(callback);
    if (userId === null || userId === undefined) {
        logger.warn('fetchCustomerByUserId called with invalid userId:', userId);
        return cb(new Error('Invalid userId'));
    }

    readCustomerByUserId(userId, (error, result) => {
        if (error) {
            logger.error('Customer Error:', error);
            return cb(error);
        }
        if (!result) {
            return cb(new Error('Customer not found'));
        }
        cb(null, result);
    });
};

export const updateCustomerById = (customerId, data, callback) => {
    const cb = onceCallback(callback);

    if (!Number.isInteger(Number(customerId)) || Number(customerId) <= 0) {
        logger.warn('updateCustomerById called with invalid customerId:', customerId);
        return cb(new Error('Invalid customerId'));
    }

    const {
        firstName,
        lastName,
        email,
        address,
        province,
        postalCode,
        phone,
        city,
        country,
        active,
    } = data || {};

    logger.info('updateCustomerById called', { customerId, data });

    const performUpdate = (updateData) => {
        logger.info('Calling updateCustomer with', { customerId, updateData });
        updateCustomer(customerId, updateData, (error, result) => {
            if (error) {
                logger.error('Customer Update Error:', error);
                return cb(error);
            }
            logger.info('Customer update result', { result });
            cb(null, result);
        });
    };

    const buildBaseUpdate = (emailId, addressId) => {
        const updateData = {};
        if (firstName) updateData.firstName = String(firstName).trim();
        if (lastName) updateData.lastName = String(lastName).trim();
        if (addressId !== undefined) updateData.addressId = addressId; // can be null if creation failed
        if (active !== undefined) updateData.active = Boolean(active);
        if (emailId !== undefined) updateData.emailId = emailId;
        return updateData;
    };

    const proceed = (emailId) => {
        // If no address fields supplied, just update the simple fields
        const anyAddressField = address || province || postalCode || phone || city || country;
        if (!anyAddressField) {
            const updateData = buildBaseUpdate(emailId, undefined);
            if (Object.keys(updateData).length === 0) {
                return cb(new Error('No data provided for update'));
            }
            return performUpdate(updateData);
        }

        // We have some address-related change; create or locate address record
        makeAddress(
            String(address || '').trim(),
            String(province || '').trim(),
            String(postalCode || '').trim(),
            String(phone || '').trim(),
            String(country || '').trim(),
            String(city || '').trim(),
            (err, { address: addrRow } = {}) => {
                if (err) {
                    logger.error('Address Creation Error:', err);
                    return cb(err);
                }
                const updateData = buildBaseUpdate(emailId, addrRow?.address_id);
                if (Object.keys(updateData).length === 0) {
                    return cb(new Error('No data provided for update'));
                }
                performUpdate(updateData);
            }
        );
    };

    // Handle email lookups / creation
    if (email) {
        readEmail(email, (error, emailRow) => {
            if (error) {
                logger.error('Email Lookup Error:', error);
                return cb(error);
            }
            const existingId = emailRow?.email_id;
            if (!existingId) {
                return createEmail(String(email).trim(), (emailErr, newEmailRow) => {
                    if (emailErr) {
                        logger.error('Email Creation Error:', emailErr);
                        return cb(emailErr);
                    }
                    proceed(newEmailRow.email_id);
                });
            }
            proceed(existingId);
        });
    } else {
        proceed(undefined);
    }
};

export const deleteCustomerById = (customerId, callback) => {
    const cb = onceCallback(callback);
    if (
        customerId === null ||
        customerId === undefined ||
        isNaN(Number(customerId)) ||
        Number(customerId) <= 0
    ) {
        logger.warn('deleteCustomerById called with invalid customerId:', customerId);
        return cb(new Error('Invalid customerId'));
    }

    softDeleteCustomer(Number(customerId), (error, result) => {
        if (error) {
            logger.error('Customer Deletion Error:', error);
            return cb(error);
        }
        cb(null, result);
        logger.info('deleteCustomerById is not implemented. CustomerId:', customerId);
    });
};

export const fetchRentalsByCustomerId = (customerId, callback) => {
    const cb = onceCallback(callback);
    if (
        customerId === null ||
        customerId === undefined ||
        isNaN(Number(customerId)) ||
        Number(customerId) <= 0
    ) {
        logger.warn('fetchRentalsByCustomerId called with invalid customerId:', customerId);
        return cb(new Error('Invalid customerId'));
    }

    readCustomerRentalHistory(customerId, (error, rentals) => {
        if (error) {
            logger.error('Rental Retrieval Error:', error);
            return cb(error);
        }
        cb(null, rentals || []);
    });
};
