import { Injectable, LoggerService as NestLoggerService } from '@nestjs/common';

type LogLevel = 'log' | 'error' | 'warn' | 'debug' | 'verbose';

@Injectable()
export class LoggerService implements NestLoggerService {
  private name = 'App';

  setContext(name: string) {
    this.name = name;
  }

  private format(level: LogLevel, message: any, context?: string, meta?: any) {
    const payload: Record<string, any> = {
      timestamp: new Date().toISOString(),
      level,
      context: context ?? this.name,
      message: typeof message === 'string' ? message : JSON.stringify(message),
    };

    if (meta) payload.meta = meta;
    return JSON.stringify(payload);
  }

  log(message: any, context?: string) {
    // Info
    // eslint-disable-next-line no-console
    console.info(this.format('log', message, context));
  }

  error(message: any, trace?: string, context?: string) {
    const meta = trace ? { trace } : undefined;
    // eslint-disable-next-line no-console
    console.error(this.format('error', message, context, meta));
  }

  warn(message: any, context?: string) {
    // eslint-disable-next-line no-console
    console.warn(this.format('warn', message, context));
  }

  debug?(message: any, context?: string) {
    // eslint-disable-next-line no-console
    console.debug(this.format('debug', message, context));
  }

  verbose?(message: any, context?: string) {
    // eslint-disable-next-line no-console
    console.debug(this.format('verbose', message, context));
  }
}

export default LoggerService;
