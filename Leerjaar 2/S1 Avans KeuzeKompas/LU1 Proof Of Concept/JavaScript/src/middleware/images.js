import multer from 'multer';

// Store file in memory as buffer
const storage = multer.memoryStorage();
export const uploadAvatar = multer({
    storage,
    limits: { fileSize: 2 * 1024 * 1024 }, // max 2MB
    fileFilter: (req, file, cb) => {
        const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/webp'];
        if (allowedTypes.includes(file.mimetype)) {
            cb(null, true);
        } else {
            cb(new Error('Invalid file type. Only JPG, JPEG, PNG, WEBP allowed.'));
        }
    },
});
