import 'dotenv/config';
import { PrismaMariaDb } from '@prisma/adapter-mariadb';
import { Injectable, OnModuleInit, OnModuleDestroy } from '@nestjs/common';
import { PrismaClient } from '@/infrastructure/database/generated/prisma/client';
import { LoggerService } from '@/common/logger.service';

@Injectable()
export class PrismaService
  extends PrismaClient
  implements OnModuleInit, OnModuleDestroy
{
  constructor(private readonly logger: LoggerService) {
    // Parse DATABASE_URL and create mariadb pool config
    const dbUrl = new URL(process.env.DATABASE_URL!);
    const adapter = new PrismaMariaDb({
      host: dbUrl.hostname,
      port: parseInt(dbUrl.port || '3306', 10),
      user: dbUrl.username,
      password: dbUrl.password,
      database: dbUrl.pathname.slice(1), // remove leading /
      connectionLimit: 20,
      connectTimeout: 30000,
    });
    super({ adapter });

    // Forward Prisma client events to our logger
    try {
      // Prisma client's $on typing may be strict depending on generated client.
      // Cast to `any` to attach listeners at runtime while keeping TypeScript happy.
      (this as any).$on('info', (e: any) => {
        this.logger.log(`${e.message}`, 'Prisma');
      });

      (this as any).$on('warn', (e: any) => {
        this.logger.warn(`${e.message}`, 'Prisma');
      });

      (this as any).$on('error', (e: any) => {
        // e is an object with message and perhaps stack
        this.logger.error(
          `${e.message}`,
          (e as any).stack ?? undefined,
          'Prisma',
        );
      });
    } catch (e) {
      // Listening may fail in some runtime builds; safely ignore
    }
  }

  async onModuleInit() {
    const maxRetries = 5;
    const delayMs = 2000;
    let attempt = 0;
    while (attempt < maxRetries) {
      try {
        await this.$connect();
        this.logger.log('Connected to database', 'PrismaService');
        return;
      } catch (err) {
        attempt += 1;
        const msg = `Database connect attempt ${attempt} failed: ${
          err instanceof Error ? err.message : String(err)
        }`;
        this.logger.warn(msg, 'PrismaService');
        if (attempt >= maxRetries) {
          this.logger.error(
            'Failed to connect to database after retries',
            err instanceof Error ? err.stack : String(err),
            'PrismaService',
          );
          throw err;
        }
        // wait before retrying
        await new Promise((r) => setTimeout(r, delayMs));
      }
    }
  }

  async onModuleDestroy() {
    try {
      await this.$disconnect();
      this.logger.log('Disconnected from database', 'PrismaService');
    } catch (e) {
      this.logger.warn(
        `Error during disconnect: ${e instanceof Error ? e.message : String(e)}`,
        'PrismaService',
      );
    }
  }
}

export type Prisma = PrismaClient;
