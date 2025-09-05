import bcrypt from 'bcrypt';
const saltRounds = 10;

const hashPassword = (plainPassword, cb) => {
    bcrypt.hash(plainPassword, saltRounds, (err, hash) => {
        if (err) return cb(err);
        cb(null, hash);
    });
};

const verifyPassword = (plainPassword, hash, cb) => {
    bcrypt.compare(plainPassword, hash, (err, result) => {
        if (err) return cb(err);
        cb(null, result); // result = true if match
    });
};

export { hashPassword, verifyPassword };
