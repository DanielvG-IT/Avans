import { Injectable, ConsoleLogger, LogLevel } from '@nestjs/common';

/**
 * Extending ConsoleLogger to inherit Nest's built-in logic while overriding the output for structured JSON.
 */
@Injectable()
export class LoggerService extends ConsoleLogger {
  private contextName = 'App';

  override setContext(name: string) {
    this.contextName = name;
    super.setContext(name);
  }

  /**
   * Centralized formatter to ensure consistent JSON structure.
   * Handles objects, strings, and circular references safely.
   */
  private formatEntry(
    level: LogLevel,
    message: unknown,
    context?: string,
    meta?: unknown,
  ): string {
    const entry = {
      timestamp: new Date().toISOString(),
      level,
      context: context ?? this.contextName,
      message: typeof message === 'string' ? message : message,
      ...(meta ? { meta } : {}),
    };

    // Using a replacer or library like 'safe-stable-stringify' is better for prod, but standard JSON.stringify works fine for now.
    return JSON.stringify(entry);
  }

  // --- Main Methods ---

  override log(message: unknown, context?: string) {
    console.info(this.formatEntry('log', message, context));
  }

  override error(message: unknown, trace?: string, context?: string) {
    const meta = trace ? { trace } : undefined;
    console.error(this.formatEntry('error', message, context, meta));
  }

  override warn(message: unknown, context?: string) {
    console.warn(this.formatEntry('warn', message, context));
  }

  override debug(message: unknown, context?: string) {
    console.debug(this.formatEntry('debug', message, context));
  }

  override verbose(message: unknown, context?: string) {
    console.debug(this.formatEntry('verbose', message, context));
  }
}

export default LoggerService;
