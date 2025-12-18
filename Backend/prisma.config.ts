import 'dotenv/config';
import { defineConfig } from 'prisma/config';

export default defineConfig({
  schema: 'src/infrastructure/database/schema/schema.prisma',
  migrations: {
    path: 'src/infrastructure/database/migrations',
    seed: 'tsx src/infrastructure/database/seed.ts',
  },
  datasource: {
    url: (() => {
      if (!process.env.DATABASE_URL) {
        throw new Error('DATABASE_URL environment variable must be set');
      }
      return process.env.DATABASE_URL;
    })(),
  },
});
