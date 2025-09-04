import dotenv from 'dotenv';
dotenv.config();

import express from 'express';
import logger from './util/logger.js';

const app = express();
const PORT = process.env.PORT || 3000;

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
