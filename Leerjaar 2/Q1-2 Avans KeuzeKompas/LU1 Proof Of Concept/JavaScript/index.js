import express from 'express';
import { v4 } from 'uuid';
import mysql from 'mysql2/promise';
import dotenv from 'dotenv';

const app = express();
const port = 3000;
dotenv.config();

app.use(express.json());

const pool = mysql.createPool({
    host: process.env.DB_HOST || 'localhost',
    port: Number(process.env.DB_PORT) || 3306,
    user: process.env.DB_USER || 'root',
    password: process.env.DB_PASSWORD || '',
    database: process.env.DB_NAME || 'test',
});

// Helper: store session record
const storeSession = (sessionId, userId) => {
    pool.execute('INSERT INTO sessions (id, user_id, created_at) VALUES (?, ?, NOW())', [
        sessionId,
        userId,
    ]);
};

app.get('/', (_req, res) => {
    res.send('Hello World!');
});

app.post('/login', async (req, res) => {
    const { username, password } = req.body || {};

    if (!username || !password) {
        return res.status(400).send({ error: 'username and password required' });
    }

    // TODO: Hash password

    // TODO: Check database for user
    if (username !== 'admin' || password !== 'password') {
        return res.status(401).send({ error: 'User not found' });
    }

    const sessionId = v4();
    try {
        storeSession(sessionId, 1);
        res.cookie('session', sessionId, { httpOnly: true }); // TODO: Make 'secure' and 'sameSite' in Prod
        return res.status(200).send({ message: 'Login successful' });
    } catch (err) {
        console.error('Error storing session:', err);
        return res.status(500).send({ error: 'Internal Server Error' });
    }
});

app.listen(port, () => {
    console.log(`Express app listening on port ${port}`);
});
