import dotenv from 'dotenv';
import { defineConfig } from 'cypress';

dotenv.config({ path: '.env.test' });

export default defineConfig({
    e2e: {
        baseUrl: process.env.CYPRESS_BASE_URL || 'http://localhost:3000',
        setupNodeEvents(on, config) {
            return config;
        },
    },
    env: {
        TEST_EMAIL: process.env.CYPRESS_TEST_EMAIL,
        TEST_PASSWORD: process.env.CYPRESS_TEST_PASSWORD,
    },
});
