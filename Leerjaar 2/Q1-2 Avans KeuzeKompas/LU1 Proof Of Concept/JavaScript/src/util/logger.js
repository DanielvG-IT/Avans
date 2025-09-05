import winston from 'winston';
const { combine, timestamp, printf, colorize, align } = winston.format;

// Configure production ready logging
const logger = winston.createLogger({
    level: process.env.LOG_LEVEL || 'debug',
    format: combine(
        colorize({ all: true }),
        timestamp({
            format: 'YYYY-MM-DD hh:mm:ss A',
        }),
        align(),
        printf((info) => `[${info.timestamp}] ${info.level}: ${info.message}`)
    ),
    defaultMeta: { service: 'expressjs' },
    transports: [],
});

// Log to console if not in production
if (process.env.NODE_ENV !== 'production') {
    logger.add(
        new winston.transports.Console({
            format: winston.format.simple(),
        })
    );
}
// Log to error files when in production
else {
    logger.add(
        new winston.transports.File({ filename: 'error.log', level: 'error' }), // error, fatal, but not other levels
        new winston.transports.File({ filename: 'combined.log', level: 'info' }) // fatal, error, warn, and info
    );
}

export { logger };
