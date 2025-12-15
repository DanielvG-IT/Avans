import 'dotenv/config';
import { defineConfig, env } from 'prisma/config';

export default defineConfig({
  schema: 'src/infrastructure/database/schema/schema.prisma',
  migrations: {
    path: 'src/infrastructure/database/migrations',
    seed: 'tsx src/infrastructure/database/seed.ts',
  },
  datasource: {
    url: env('DATABASE_URL'),
  },
});
