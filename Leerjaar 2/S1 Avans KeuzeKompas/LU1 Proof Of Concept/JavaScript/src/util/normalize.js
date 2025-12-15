export const normalizeId = (id) => {
    if (id === null || id === undefined) return null;
    if (typeof id === 'number' && Number.isInteger(id)) return id;
    const s = String(id).trim();
    if (!/^-?\d+$/.test(s)) return null;
    return parseInt(s, 10);
};
export const normalizeEmail = (email) =>
    String(email || '')
        .trim()
        .toLowerCase();
export const normalizeRole = (role) =>
    String(role || '')
        .trim()
        .toUpperCase();
export const normalizeFormat = (format) =>
    String(format || '')
        .trim()
        .toUpperCase();
export const normalizeUserId = (id) => String(id || '').trim();
export const normalizeBase64 = (input) =>
    String(input || '')
        .replace(/^data:[^;]+;base64,/, '')
        .trim();

export const normalizeName = (name) =>
    String(name || '')
        .trim()
        .replace(/\s+/g, ' ')
        .replace(/[^a-zA-Z\s'-]/g, '')
        .replace(/\b\w/g, (char) => char.toUpperCase());
