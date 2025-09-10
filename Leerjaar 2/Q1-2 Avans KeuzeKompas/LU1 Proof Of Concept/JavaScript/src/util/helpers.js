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
};
