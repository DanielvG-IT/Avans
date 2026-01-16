import 'dotenv/config';
import { PrismaMariaDb } from '@prisma/adapter-mariadb';
import { Injectable, OnModuleInit, OnModuleDestroy } from '@nestjs/common';
import { PrismaClient } from '@/infrastructure/database/generated/prisma/client';
import { LoggerService } from '@/logger.service';

@Injectable()
export class PrismaService
  extends PrismaClient
  implements OnModuleInit, OnModuleDestroy
{
  constructor(private readonly logger: LoggerService) {
    // In development, use simple connection config. In production, use detailed config with SSL
    const isDevelopment = process.env.NODE_ENV === 'development';
    
    const adapter = isDevelopment
      ? new PrismaMariaDb({
          host: process.env.DB_HOSTNAME || 'localhost',
          port: Number.parseInt(process.env.DB_PORT || '3306', 10),
          user: process.env.DB_USERNAME || 'root',
          password: process.env.DB_PASSWORD!,
          database: process.env.DB_NAME || 'compassgpt',
        })
      : new PrismaMariaDb({
          host: process.env.DB_HOSTNAME || 'compassgptdatabase.mysql.database.azure.com',
          port: Number.parseInt(process.env.DB_PORT || '3306', 10),
          user: process.env.DB_USERNAME || 'dbadmin',
          password: process.env.DB_PASSWORD!,
          database: process.env.DB_NAME || 'compassgpt',
          connectionLimit: Number.parseInt(process.env.DB_CONN_LIMIT || '20', 10),
          connectTimeout: Number.parseInt(process.env.DB_CONNECT_TIMEOUT_MS || '30000', 10),
          ssl: {
            rejectUnauthorized: true,
          },
        });
    super({ adapter });

    this.logger.setContext('PrismaService');

    // Forward Prisma client events to our logger
    try {
      // Prisma client's $on typing may be strict depending on generated client.
      // Use type assertion for the $on method
      const client = this as unknown as {
        $on: (event: string, callback: (e: any) => void) => void;
      };

      const isProd = process.env.NODE_ENV === 'production';

      const maskParams = (params: unknown) => {
        try {
          if (params == null) return params;
          const parsed = typeof params === 'string' ? JSON.parse(params) : params;
          const redact = (obj: any) => {
            if (obj && typeof obj === 'object') {
              for (const k of Object.keys(obj)) {
                if (/password|pass|token|secret|api[_-]?key/i.test(k)) {
                  obj[k] = '***REDACTED***';
                } else if (typeof obj[k] === 'object') {
                  redact(obj[k]);
                }
              }
            }
          };
          redact(parsed);
          return JSON.stringify(parsed);
        } catch {
          return String(params).replace(/("?(password|pass|token|secret|api[_-]?key)"?\s*:\s*)".*?"/gi, '$1"***REDACTED***"');
        }
      };

      client.$on('info', (e) => {
        this.logger.log(e.message, 'Prisma');
      });

      client.$on('warn', (e) => {
        this.logger.warn(e.message, 'Prisma');
      });

      client.$on('error', (e) => {
        if (isProd) {
          // keep messages minimal in production logs
          this.logger.error('Prisma error', undefined, 'Prisma');
        } else {
          this.logger.error(e.message, e.stack ?? undefined, 'Prisma');
        }
      });

      // Detailed query logging only in non-production environments
      if (!isProd) {
        client.$on('query', (e: { query: string; params?: string; duration?: number }) => {
          this.logger.debug(`Prisma Query: ${e.query}`, 'Prisma');
          this.logger.debug(`Prisma Params: ${maskParams(e.params)}`, 'Prisma');
          if (e.duration !== undefined) {
            this.logger.debug(`Prisma Duration: ${e.duration}ms`, 'Prisma');
          }
        });
      }
    } catch {
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
