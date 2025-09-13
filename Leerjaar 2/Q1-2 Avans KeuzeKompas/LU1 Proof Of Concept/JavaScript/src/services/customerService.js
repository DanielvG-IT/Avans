import { readCustomers } from '../dao/customer.js';
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
    const { search = '', active = '1', storeId = null, sort = 'createDate,asc' } = params || {};
    const filters = { search, active, storeId, sort };

    readCustomers(filters, (error, result) => {
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
