import winston from 'winston';
import DailyRotateFile from 'winston-daily-rotate-file';
const { combine, timestamp, printf, colorize, align, json } = winston.format;

// Configure production-ready logging with structured JSON for better parsing
const logger = winston.createLogger({
    level: process.env.LOG_LEVEL || 'info', // Default to 'info' for production
    format: combine(
        timestamp({
            format: 'YYYY-MM-DDTHH:mm:ss.SSSZ', // ISO 8601 for better log aggregation
        }),
        json() // Use JSON for structured logging
    ),
    defaultMeta: { service: 'expressjs' },
    transports: [],
    exitOnError: false, // Prevent logger from exiting on error
});

// Handle uncaught exceptions and unhandled rejections
logger.exceptions.handle(new winston.transports.File({ filename: 'exceptions.log' }));
logger.rejections.handle(new winston.transports.File({ filename: 'rejections.log' }));

// Log to console in development with colors
if (process.env.NODE_ENV !== 'production') {
    logger.add(
        new winston.transports.Console({
            format: combine(
                colorize({ all: true }),
                timestamp({
                    format: 'YYYY-MM-DD hh:mm:ss A',
                }),
                align(),
                printf((info) => `[${info.timestamp}] ${info.level}: ${info.message}`)
            ),
        })
    );
}
// Log to rotating files in production
else {
    logger.add(
        new DailyRotateFile({
            filename: 'logs/error-%DATE%.log',
            datePattern: 'YYYY-MM-DD',
            level: 'error',
            maxSize: '20m',
            maxFiles: '14d',
        }),
        new DailyRotateFile({
            filename: 'logs/combined-%DATE%.log',
            datePattern: 'YYYY-MM-DD',
            level: 'info',
            maxSize: '20m',
            maxFiles: '14d',
        })
    );
}

export { logger };
