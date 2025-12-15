export const getBase64 = (fileBuffer) => fileBuffer.toString('base64');

export const getFormatFromMime = (mimeType) => {
    switch (mimeType) {
        case 'image/jpeg':
            return 'JPEG';
        case 'image/jpg':
            return 'JPG';
        case 'image/png':
            return 'PNG';
        case 'image/webp':
            return 'WEBP';
        default:
            return null;
    }
};
