import dotenv from 'dotenv';
dotenv.config();

import express from 'express';
import logger from './util/logger.js';
import createError from 'http-errors';
import cookieParser from 'cookie-parser';
import { create } from 'express-handlebars';

var app = express();
const PORT = process.env.PORT || 3000;
const hbs = create({
    partialsDir: './views/partials',
    layoutsDir: './views/layouts',
    defaultLayout: 'main',
    extname: '.hbs',
});

// Middleware
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static('./public'));

// View engine
app.engine('handlebars', hbs.engine);
app.set('view engine', 'handlebars');
app.set('views', './views');

// Import Routes
app.get('/', (req, res) => {
    logger.debug('/ route called');
    res.render('main.hbs');
});
app.get('/login', (req, res) => {
    logger.debug('/login route called');
    res.render('login.hbs');
});
app.get('/register', (req, res) => {
    logger.debug('/register route called');
    res.render('register.hbs');
});

// 404 Not Found handler
app.use(function (req, res, next) {
    next(createError(404));
});

// Error handler
app.use(function (err, req, res, next) {
    // set locals, only providing error in development
    res.locals.message = err.message;
    res.locals.error = process.env.NODE_ENV === 'development' ? err : {};

    // render the error page
    res.status(err.status || 500);
    res.json({ error: res.locals.message });
});

// Start the server
app.listen(PORT, () => {
    logger.info(`Server is running on port ${PORT}`);
});

export default app;
