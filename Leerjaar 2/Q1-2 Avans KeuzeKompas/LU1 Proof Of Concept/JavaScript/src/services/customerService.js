import {
    readCustomerById,
    readCustomers,
    softDeleteCustomer,
    updateCustomer,
} from '../dao/customer.js';
import { logger } from '../util/logger.js';

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

    readCustomerById({ customerId: Number(customerId) }, (error, result) => {
        if (error) {
            logger.error('Customer Error:', error);
            return cb(error);
        }
        const { customers } = result || {};
        cb(null, Array.isArray(customers) && customers.length ? customers[0] : null);
    });
};

export const updateCustomerById = (customerId, data, callback) => {
    const cb = onceCallback(callback);
    if (
        customerId === null ||
        customerId === undefined ||
        isNaN(Number(customerId)) ||
        Number(customerId) <= 0
    ) {
        logger.warn('updateCustomerById called with invalid customerId:', customerId);
        return cb(new Error('Invalid customerId'));
    }

    // Validate and sanitize input data
    const { firstName, lastName, email, address, postalCode, phone, city, country, active } =
        data || {};
    const updateData = {
        firstName: String(firstName).trim(),
        lastName: String(lastName).trim(),
        email: String(email).trim(),
        address: String(address).trim(),
        postalCode: String(postalCode).trim(),
        phone: String(phone).trim(),
        city: String(city).trim(),
        country: String(country).trim(),
        active: Boolean(active),
    };

    // Call the data access layer to update the customer
    updateCustomer(customerId, updateData, (error, result) => {
        if (error) {
            logger.error('Customer Update Error:', error);
            return cb(error);
        }
        cb(null, result);
    });
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
