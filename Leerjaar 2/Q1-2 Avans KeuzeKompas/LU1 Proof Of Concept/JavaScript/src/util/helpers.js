import { json } from 'stream/consumers';

export const expressHelpers = {
    chunk: (arr, size) => {
        if (!Array.isArray(arr)) return [];
        size = parseInt(size, 10) || 1;
        const chunks = [];
        for (let i = 0; i < arr.length; i += size) chunks.push(arr.slice(i, i + size));
        return chunks;
    },
    ifEquals: (arg1, arg2, options) => {
        return arg1 === arg2 ? options.fn(this) : options.inverse(this);
    },
    range: (start, end) => {
        const range = [];
        for (let i = start; i <= end; i++) {
            range.push(i);
        }
        return range;
    },
    add: (a, b) => {
        return a + b;
    },
    json: (value) => {
        return JSON.stringify(value);
    },
    eq: (a, b) => {
        return a === b;
    },
    paginationRange: (page, totalPages, options) => {
        const delta = 2;
        const range = [];
        let left = parseInt(page, 10) - delta;
        let right = parseInt(page, 10) + delta;
        totalPages = parseInt(totalPages, 10);

        if (Number.isNaN(left) || Number.isNaN(right) || Number.isNaN(totalPages)) {
            return options.fn({ range, page, totalPages });
        }

        if (left < 1) {
            right += 1 - left;
            left = 1;
        }
        if (right > totalPages) {
            left -= right - totalPages;
            right = totalPages;
        }
        if (left < 1) left = 1;

        for (let i = left; i <= right; i++) {
            range.push(i);
        }

        return options.fn({ range, page: parseInt(page, 10), totalPages });
    },
    gt: (a, b) => a > b,
    lt: (a, b) => a < b,
    last: (arr) => (Array.isArray(arr) ? arr[arr.length - 1] : undefined),
    formatCurrency: (value) => {
        if (typeof value !== 'number') return value;
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
        }).format(value);
    },
    formatDate: (date) => {
        if (!(date instanceof Date)) return date;
        return date.toISOString().split('T')[0];
    },
};
