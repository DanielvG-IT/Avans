import { requireStaffAuthApi, requireStaffAuthWeb } from '../middleware/auth.js';
import { readStores } from '../dao/store.js';
import { logger } from '../util/logger.js';
import express from 'express';
import {
    fetchRentalsByCustomerId,
    updateCustomerById,
    fetchCustomerById,
    fetchCustomers,
    addCustomer,
} from '../services/customerService.js';
import { fetchStaff } from '../services/authService.js';

const staffRouter = express.Router();

// Simple helper to parse ints
const parsePositiveInt = (v, fallback) => {
    const n = parseInt(v, 10);
    return Number.isInteger(n) && n > 0 ? n : fallback;
};

staffRouter.get('/', requireStaffAuthWeb, (req, res, next) => {
    res.render('staff/overview', { title: 'Staff Overview' });
});

staffRouter.get('/dashboard', requireStaffAuthWeb, (req, res, next) => {
    res.render('staff/dashboard', { title: 'Staff Dashboard' });
});

staffRouter.get('/crm', requireStaffAuthWeb, (req, res, next) => {
    const page = parsePositiveInt(req.query.page, 1);
    const limit = parsePositiveInt(req.query.limit, 25);
    const search = (req.query.search || '').trim();
    const active = String(req.query.active).toLowerCase();
    const storeId = req.query.storeId || null;

    let [sortField, sortDir] = (req.query.sort || 'title,asc').split(',');
    sortField = (sortField || 'title').trim();
    sortDir = (sortDir || 'asc').toLowerCase() === 'desc' ? 'desc' : 'asc';

    const filters = { search, active, storeId, sort: `${sortField},${sortDir}`, page, limit };

    readStores((storeError, stores) => {
        if (storeError) {
            logger.error('Store Error:', storeError);
            return next(storeError);
        }

        fetchCustomers(filters, (error, result) => {
            if (error) {
                logger.error('Customer Error:', error);
                return next(error);
            }

            // Normalize result values and avoid shadowing outer `page`
            const {
                total = 0,
                page: resultPage = page,
                pageSize = limit,
                totalPages = Math.max(
                    1,
                    Math.ceil(result && result.total ? result.total / pageSize : 1)
                ),
                customers = [],
            } = result || {};

            const currentPage = parsePositiveInt(resultPage, page);

            // Build pageQueryPrefix (path + existing querystring without page param) so templates can append page numbers
            const qp = { ...req.query };
            delete qp.page;
            const params = new URLSearchParams(qp);
            const pathPrefix = (req.baseUrl || '') + req.path;
            const pageQueryPrefix = params.toString()
                ? `${pathPrefix}?${params.toString()}&page=`
                : `${pathPrefix}?page=`;

            // Pagination window helper (produces array with page/active or ellipsis)
            const buildPagesToShow = (current, totalP, maxButtons = 7) => {
                const pages = [];
                if (totalP <= maxButtons) {
                    for (let i = 1; i <= totalP; i++)
                        pages.push({ page: i, active: i === current });
                    return pages;
                }

                const leftEdge = 1;
                const rightEdge = totalP;
                const windowSize = maxButtons - 2; // reserve first and last
                let start = Math.max(2, current - Math.floor(windowSize / 2));
                let end = Math.min(totalP - 1, current + Math.floor(windowSize / 2));

                // Adjust window if it spills
                if (current - start < Math.floor(windowSize / 2)) {
                    end = Math.min(
                        totalP - 1,
                        end + (Math.floor(windowSize / 2) - (current - start))
                    );
                }
                if (end - current < Math.floor(windowSize / 2)) {
                    start = Math.max(2, start - (Math.floor(windowSize / 2) - (end - current)));
                }

                // First page
                pages.push({ page: leftEdge, active: leftEdge === current });

                if (start > 2) pages.push({ ellipsis: true });

                for (let i = start; i <= end; i++) pages.push({ page: i, active: i === current });

                if (end < totalP - 1) pages.push({ ellipsis: true });

                // Last page
                pages.push({ page: rightEdge, active: rightEdge === current });

                return pages;
            };

            const pagesToShow = buildPagesToShow(currentPage, totalPages, 6);
            const prevPage = currentPage > 1 ? currentPage - 1 : null;
            const nextPage = currentPage < totalPages ? currentPage + 1 : null;

            const filtersForRender = {
                search,
                active,
                storeId: storeId ? Number(storeId) : 0,
                sort: `${sortField},${sortDir}`,
                page: currentPage,
                limit,
            };

            // RENDER
            logger.debug('Rendering Staff CRM', {
                user: req.user?.id,
                params: {
                    search,
                    active,
                    storeId,
                    sort: `${sortField},${sortDir}`,
                    page: currentPage,
                    limit,
                },
            });

            res.render('staff/crm', {
                title: 'Customer Relationship Management',
                customers,
                total,
                page: currentPage,
                pageSize,
                totalPages,
                filters: filtersForRender,
                stores,
                // Pagination helpers for the template you provided
                pagesToShow,
                prevPage,
                nextPage,
                pageQueryPrefix,
            });
        });
    });
});

staffRouter.get('/crm/search', requireStaffAuthApi, (req, res) => {
    const q = (req.query.q || '').trim();
    if (!q) return res.json([]);
    const params = {
        search: q,
        active: null,
        storeId: null,
        sort: 'title,asc',
        page: 1,
        limit: 12,
    };
    fetchCustomers(params, (err, result) => {
        if (err) {
            logger.error('Customer search error:', err);
            return res.status(500).json([]);
        }
        const rows = result && result.customers ? result.customers : [];
        // map to compact shape for client
        const out = rows.map((r) => ({
            customer_id: r.customerId ?? r.customer_id,
            first_name: r.firstName ?? r.first_name,
            last_name: r.lastName ?? r.last_name,
            email: r.email,
            phone: r.phone,
        }));
        res.json(out);
    });
});

staffRouter.get('/crm/new', requireStaffAuthWeb, (req, res, next) => {
    readStores((storeError, stores) => {
        if (storeError) {
            logger.error('Store Error:', storeError);
            return next(storeError);
        }
        res.render('addOrEditCustomer', {
            title: 'New Customer',
            customer: {},
            stores,
            isEdit: false,
            actionUrl: '/staff/crm/new',
            actionMethod: 'POST',
            returnUrl: '/staff/crm',
        });
    });
});
staffRouter.post('/crm/new', requireStaffAuthApi, (req, res, next) => {
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
        active = 1,
        storeId = 1, // Default storeId if not provided
    } = req.body;

    // Basic validation
    if (
        !firstName ||
        !lastName ||
        !email ||
        !address ||
        !city ||
        !country ||
        !postalCode ||
        !phone ||
        !province ||
        !storeId
    ) {
        readStores((storeError, stores) => {
            if (storeError) {
                logger.error('Store Error:', storeError);
                return next(storeError);
            }
            return res.render('addOrEditCustomer', {
                title: 'New Customer',
                customer: {
                    firstName,
                    lastName,
                    email,
                    address,
                    city,
                    country,
                    postalCode,
                    phone,
                    province,
                    active,
                    storeId,
                },
                stores,
                isEdit: false,
                actionUrl: '/staff/crm/new',
                actionMethod: 'POST',
                returnUrl: '/staff/crm',
                errorMessage: 'All fields are required.',
            });
        });
    }

    const customerData = {
        firstName,
        lastName,
        email,
        address,
        city,
        country,
        postalCode,
        phone,
        province,
        active: active ? 1 : 0,
        storeId: parsePositiveInt(storeId, 1),
    };

    addCustomer(customerData, (error, customerId) => {
        if (error) {
            logger.error('Add Customer Error:', error);
            readStores((storeError, stores) => {
                if (storeError) {
                    logger.error('Store Error:', storeError);
                    return next(storeError);
                }
                // Determine a user-friendly error message, specially handling duplicate email errors.
                let renderErrorMessage =
                    error && error.message
                        ? error.message
                        : 'An error occurred while adding the customer.';
                if (
                    error &&
                    (error.code === 'ER_DUP_ENTRY' ||
                        (typeof error.message === 'string' &&
                            /Duplicate entry .* for key ['"]?customer\.email_id['"]?/.test(
                                error.message
                            )))
                ) {
                    renderErrorMessage = 'Email already in use.';
                }

                return res.render('addOrEditCustomer', {
                    title: 'New Customer',
                    customer: customerData,
                    stores,
                    isEdit: false,
                    actionUrl: '/staff/crm/new',
                    actionMethod: 'POST',
                    returnUrl: '/staff/crm',
                    errorMessage: renderErrorMessage,
                });
            });
        } else {
            return res.redirect('/staff/crm/' + customerId);
        }
    });
});

staffRouter.get('/crm/:customerId', requireStaffAuthWeb, (req, res, next) => {
    const customerId = parsePositiveInt(req.params.customerId, null);

    fetchCustomerById(customerId, (error, customer) => {
        if (error) {
            logger.error('Customer Error:', error);
            return next(error);
        }
        if (!customer) {
            logger.warn('Customer not found:', { customerId });
            return next(new Error('Customer not found'));
        }

        fetchRentalsByCustomerId(customerId, (rentalError, rentals) => {
            if (rentalError) {
                logger.error('Rental Retrieval Error:', rentalError);
                return next(rentalError);
            }

            // Robust categorization and improved metrics (single-pass, defensive)
            const now = new Date();
            const parseDate = (d) => {
                if (!d) return null;
                const dd = new Date(d);
                return isNaN(dd.getTime()) ? null : dd;
            };
            const addDays = (date, days) => new Date(date.getTime() + days * 24 * 60 * 60 * 1000);

            const rentalsList = Array.isArray(rentals) ? rentals : [];

            const activeRentals = [];
            const futureRentals = [];
            const pastRentals = [];
            let overdueCount = 0;

            for (let i = 0; i < rentalsList.length; i++) {
                const r = rentalsList[i];

                const rentalDate = parseDate(r.rental_date);
                const returnDate = parseDate(r.return_date);

                // Normalize duration once
                const durationDays = Number(r.rental_duration) || 0;
                const normalizedDuration = durationDays > 0 ? Math.floor(durationDays) : 0;
                r.rental_duration = normalizedDuration;

                // Normalize numeric/monetary fields
                r.amount =
                    r.amount !== undefined
                        ? Number(r.amount)
                        : r.rental_rate
                        ? Number(r.rental_rate)
                        : 0;
                r.rental_rate = r.rental_rate !== undefined ? Number(r.rental_rate) : 0;

                r.due_date = rentalDate ? addDays(rentalDate, normalizedDuration) : null;
                r.isReturned = !!returnDate;
                r.isOverdue = !r.isReturned && r.due_date ? now > r.due_date : false;

                // Skip invalid rental_date entries
                if (!rentalDate) continue;

                // Categorize
                if (r.rental_status === 'Future' || (rentalDate && rentalDate > now)) {
                    r.rental_status = 'Future';
                    futureRentals.push(r);
                } else if (r.rental_status === 'Past' || (returnDate && returnDate <= now)) {
                    pastRentals.push(r);
                } else {
                    // started and not returned, or return in future
                    activeRentals.push(r);
                }

                if (r.isOverdue) overdueCount++;
            }

            // Aggregate metrics
            const metrics = {
                totalRentals: rentalsList.length,
                activeRentalsCount: activeRentals.length,
                overdueRentalsCount: overdueCount,
            };

            res.render('viewCustomer', {
                title: 'Customer Details',
                user: req.user,
                customer,
                metrics,
                activeRentals,
                futureRentals,
                pastRentals,
                returnUrl: '/staff/crm',
            });
        });
    });
});
staffRouter.get('/crm/:customerId/edit', requireStaffAuthWeb, (req, res, next) => {
    const customerId = parsePositiveInt(req.params.customerId, null);
    fetchCustomerById(customerId, (error, customer) => {
        if (error) {
            logger.error('Customer Error:', error);
            return next(error);
        }

        res.render('addOrEditCustomer', {
            title: 'Edit Customer',
            customer: customer,
            isEdit: true,
            actionUrl: '/staff/crm/' + customerId + '/edit',
            actionMethod: 'POST',
            returnUrl: '/staff/crm',
        });
    });
});
staffRouter.post('/crm/:customerId/edit', requireStaffAuthWeb, (req, res, next) => {
    logger.info('staffRouter.post /crm/:customerId/edit triggered', {
        body: req.body,
        params: req.params,
    });
    const customerId = parsePositiveInt(req.params.customerId, null);
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
        active = 1,
        storeId = 1, // Default storeId if not provided
    } = req.body;

    // Basic validation
    if (
        !firstName ||
        !lastName ||
        !email ||
        !address ||
        !city ||
        !country ||
        !postalCode ||
        !phone ||
        !province ||
        !storeId
    ) {
        readStores((storeError, stores) => {
            if (storeError) {
                logger.error('Store Error:', storeError);
                return next(storeError);
            }
            return res.render('addOrEditCustomer', {
                title: 'New Customer',
                customer: {
                    firstName,
                    lastName,
                    email,
                    address,
                    city,
                    country,
                    postalCode,
                    phone,
                    province,
                    active,
                    storeId,
                },
                stores,
                isEdit: false,
                actionUrl: '/staff/crm/' + customerId + '/edit',
                actionMethod: 'POST',
                returnUrl: '/staff/crm',
                errorMessage: 'All fields are required.',
            });
        });
    }

    const customerData = {
        firstName,
        lastName,
        email,
        address,
        city,
        country,
        postalCode,
        phone,
        province,
        active: active ? 1 : 0,
        storeId: parsePositiveInt(storeId, 1),
    };

    updateCustomerById(customerId, customerData, (error, success) => {
        if (error) {
            logger.error('Add Customer Error:', error);
            readStores((storeError, stores) => {
                if (storeError) {
                    logger.error('Store Error:', storeError);
                    return next(storeError);
                }
                // Determine a user-friendly error message, specially handling duplicate email errors.
                let renderErrorMessage =
                    error && error.message
                        ? error.message
                        : 'An error occurred while adding the customer.';
                if (
                    error &&
                    (error.code === 'ER_DUP_ENTRY' ||
                        (typeof error.message === 'string' &&
                            /Duplicate entry .* for key ['"]?customer\.email_id['"]?/.test(
                                error.message
                            )))
                ) {
                    renderErrorMessage = 'Email already in use.';
                }

                return res.render('addOrEditCustomer', {
                    title: 'New Customer',
                    customer: customerData,
                    stores,
                    isEdit: false,
                    actionUrl: '/staff/crm/new',
                    actionMethod: 'POST',
                    returnUrl: '/staff/crm',
                    errorMessage: renderErrorMessage,
                });
            });
        } else {
            return res.redirect('/staff/crm/' + customerId);
        }
    });
});

staffRouter.get('/crm/:customerId/rent', requireStaffAuthWeb, (req, res, next) => {
    const customerId = parsePositiveInt(req.params.customerId, null);
    if (!customerId) return next(new Error('Invalid customer ID'));

    readStores((storeError, stores) => {
        if (storeError) {
            logger.error('Store Error:', storeError);
            return next(storeError);
        }
        if (!stores || stores.length === 0) {
            logger.error('No stores found for rent page');
            return next(new Error('No stores available'));
        }

        const staffUserId = req.user?.id ?? req.user?.userId ?? req.user?.staff_id;

        fetchStaff(staffUserId, (staffError, staff) => {
            if (staffError) {
                logger.warn(
                    'Staff fetch error for rent page; proceeding with req.user fallback',
                    staffError
                );
            }
            // proceed even if staff is null; template falls back to user.id
            fetchCustomerById(customerId, (err, customer) => {
                if (err) {
                    logger.error('Customer fetch error for rent page:', err);
                    return next(err);
                }
                if (!customer) {
                    return next(new Error('Customer not found'));
                }

                const preselectedCustomer = {
                    customer_id: customer.customer_id ?? customer.customerId,
                    first_name: customer.first_name ?? customer.firstName,
                    last_name: customer.last_name ?? customer.lastName,
                    email: customer.email,
                    phone: customer.phone,
                    address: customer.address,
                };
                const preselectedCustomerB64 = Buffer.from(
                    JSON.stringify(preselectedCustomer)
                ).toString('base64');

                res.render('staff/rentMovie', {
                    title: 'Rent Movie',
                    preselectedCustomer,
                    preselectedMovie: null,
                    // NEW: safe bootstrap values (no Handlebars in script tags)
                    preselectedCustomerB64,
                    preselectedMovieB64: '',
                    actionUrl: '/rentals/new',
                    actionMethod: 'POST',
                    returnUrl: '/staff/crm/' + customerId,
                    stores,
                    staff: staff || null,
                    user: req.user || null,
                });
            });
        });
    });
});

export default staffRouter;
