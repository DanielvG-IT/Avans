import { readStores } from '../dao/store.js';
import { requireStaffAuthWeb } from '../middleware/auth.js';
import { fetchCustomers } from '../services/customerService.js';
import { logger } from '../util/logger.js';
import express from 'express';

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

            const pagesToShow = buildPagesToShow(currentPage, totalPages, 7);
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

export default staffRouter;
