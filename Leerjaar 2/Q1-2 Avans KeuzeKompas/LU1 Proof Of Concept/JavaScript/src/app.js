import express from 'express';
import winston from 'winston';
import dotenv from 'dotenv';

const app = express();
dotenv.config();
const PORT = process.env.PORT || 3000;

// Configure production ready logging
const logger = winston.createLogger({
    level: 'info',
    format: winston.format.json(),
    defaultMeta: { service: 'user-service' },
    transports: [
        new winston.transports.File({ filename: 'error.log', level: 'error' }), // error, fatal, but not other levels
        new winston.transports.File({ filename: 'combined.log', level: 'info' }), // fatal, error, warn, and info
    ],
});

// Log to console if not in production
if (process.env.NODE_ENV !== 'production') {
    logger.add(
        new winston.transports.Console({
            format: winston.format.simple(),
        })
    );
}

// Middleware
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Routes
app.get('/', (req, res) => {
    logger.debug('fucka mensen');
    res.json({ message: 'Hello World!' });
});

// Start server
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});
