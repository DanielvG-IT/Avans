import 'dotenv/config';
import { defineConfig } from 'prisma/config';

export default defineConfig({
  schema: 'src/infrastructure/database/schema/schema.prisma',
  migrations: {
    path: 'src/infrastructure/database/migrations',
    seed: 'tsx src/infrastructure/database/seed.ts',
  },
  datasource: {
    url:
      process.env.DATABASE_URL ||
      'postgresql://user:password@localhost:5432/db',
  },
});
