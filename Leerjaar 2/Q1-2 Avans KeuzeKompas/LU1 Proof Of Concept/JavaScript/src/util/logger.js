import fs from 'fs';
import path from 'path';
import winston from 'winston';
import DailyRotateFile from 'winston-daily-rotate-file';

const { combine, timestamp, printf, colorize, align, json } = winston.format;

// Create logs directory if it doesn't exist
const logsDir = path.resolve(process.cwd(), 'logs');
try {
    fs.mkdirSync(logsDir, { recursive: true });
} catch (err) {
    console.error('Could not create logs directory:', err);
}

// Helper: colored console format for development
const devFormat = combine(
    colorize({ all: true }),
    timestamp({ format: 'YYYY-MM-DD hh:mm:ss A' }),
    align(),
    printf(({ timestamp, level, message, ...meta }) => {
        const metaStr = Object.keys(meta).length ? JSON.stringify(meta) : '';
        return `[${timestamp}] ${level}: ${message} ${metaStr}`;
    })
);

// Common JSON format for files
const fileFormat = combine(timestamp({ format: 'YYYY-MM-DDTHH:mm:ss.SSSZ' }), json());

// Create the Winston logger
const logger = winston.createLogger({
    level: process.env.LOG_LEVEL || 'info',
    format: fileFormat,
    defaultMeta: { service: 'expressjs' },
    transports: [],
    exitOnError: false, // prevent Winston from exiting on exceptions
});

// Console transport for development
if (process.env.NODE_ENV !== 'production') {
    logger.add(new winston.transports.Console({ format: devFormat }));
}

// Daily rotated files for production
logger.add(
    new DailyRotateFile({
        filename: path.join(logsDir, 'combined-%DATE%.log'),
        datePattern: 'YYYY-MM-DD',
        maxSize: '20m',
        maxFiles: '30d',
        zippedArchive: true,
        level: 'info',
        format: fileFormat,
    })
);

logger.add(
    new DailyRotateFile({
        filename: path.join(logsDir, 'error-%DATE%.log'),
        datePattern: 'YYYY-MM-DD',
        maxSize: '20m',
        maxFiles: '30d',
        zippedArchive: true,
        level: 'error',
        format: fileFormat,
    })
);

// Exception handling
const exceptionTransport = new DailyRotateFile({
    filename: path.join(logsDir, 'exceptions-%DATE%.log'),
    datePattern: 'YYYY-MM-DD',
    maxSize: '20m',
    maxFiles: '30d',
    zippedArchive: true,
    utc: true,
    format: fileFormat,
});

exceptionTransport.on('rotate', (oldPath, newPath) => {
    logger.info(`Exceptions log rotated from ${oldPath} to ${newPath}`);
});

exceptionTransport.on('error', (err) => {
    console.error('Exception transport error:', err);
});

logger.exceptions.handle(exceptionTransport);

// Rejection handling
const rejectionTransport = new DailyRotateFile({
    filename: path.join(logsDir, 'rejections-%DATE%.log'),
    datePattern: 'YYYY-MM-DD',
    maxSize: '20m',
    maxFiles: '30d',
    zippedArchive: true,
    utc: true,
    level: 'error',
    format: fileFormat,
});

rejectionTransport.on('rotate', (oldPath, newPath) => {
    logger.info(`Rejections log rotated from ${oldPath} to ${newPath}`);
});

rejectionTransport.on('error', (err) => {
    console.error('Rejection transport error:', err);
});

logger.rejections.handle(rejectionTransport);

// Graceful shutdown
process.on('SIGINT', () => {
    logger.info('Process exiting: closing logger...');
    logger.end?.();
    logger.close?.();
    process.exit(0);
});

export { logger };
