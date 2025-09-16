import path from 'path';
import express from 'express';
import { fileURLToPath } from 'url';
import createError from 'http-errors';
import cookieParser from 'cookie-parser';
import { create } from 'express-handlebars';

import { logger } from './util/logger.js';
import { expressHelpers } from './util/helpers.js';
import authRouter from './controllers/auth.routes.js';
import indexRouter from './controllers/index.routes.js';
import staffRouter from './controllers/staff.routes.js';
import moviesRouter from './controllers/movies.routes.js';
import rentalRouter from './controllers/rental.routes.js';
import customerRouter from './controllers/customer.routes.js';
import { optionalCustomerAuthWeb } from './middleware/auth.js';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

var app = express();
const PORT = process.env.PORT || 3000;
const hbs = create({
    extname: '.hbs', // or '.handlebars'
    helpers: expressHelpers,
    layoutsDir: path.join(__dirname, 'views', 'layouts'),
    partialsDir: path.join(__dirname, 'views', 'partials'),
    defaultLayout: 'main',
});

// General Middleware
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

// Authentication Middleware
app.use(optionalCustomerAuthWeb);
app.use((req, res, next) => {
    res.locals.user = req.user;
    next();
});

// View engine
app.engine('hbs', hbs.engine);
app.set('view engine', 'hbs');
app.set('views', path.join(__dirname, 'views')); // absolute path

// Import Routes
app.use('/', indexRouter);
app.use('/auth', authRouter);
app.use('/staff', staffRouter);
app.use('/movies', moviesRouter);
app.use('/rentals', rentalRouter);
app.use('/customer', customerRouter);
app.get('/about', (req, res) => res.render('about'));

// 404 Not Found handler
app.use(function (req, res, next) {
    next(createError(404));
});

// Error handler
app.use(function (err, req, res, next) {
    // set locals, only providing error in development
    res.locals.message = err.message;
    res.locals.error = process.env.NODE_ENV === 'development' ? err : {};

    const errorObj = {
        title: res.locals.message,
        statusCode: err.status || 500,
        message: process.env.NODE_ENV === 'development' ? err.stack : null,
    };

    logger.error(errorObj);

    // render the error page
    res.status(err.status || 500);
    res.render('error', { title: 'Error', error: errorObj });
});

// Start the server
app.listen(PORT, () => {
    const env = process.env.NODE_ENV || 'Node';
    logger.info(`${env.charAt(0).toUpperCase() + env.slice(1)} server is running on port ${PORT}`);
});
