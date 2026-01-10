import { Controller, Get } from '@nestjs/common';
import {
  HealthCheck,
  HealthCheckService,
  HttpHealthIndicator,
  MemoryHealthIndicator,
} from '@nestjs/terminus';

@Controller()
export class AppController {
  constructor(
    private http: HttpHealthIndicator,
    private health: HealthCheckService,
    private memory: MemoryHealthIndicator,
  ) {}

  @Get('health')
  @HealthCheck()
  public async check() {
    return this.health.check([
      // 1. Check if API can hit Google (Internet connectivity)
      () => this.http.pingCheck('google', 'https://google.com'),

      // 2. Check if the server is running out of RAM (Heap limit 150MB)
      () => this.memory.checkHeap('memory_heap', 150 * 1024 * 1024),
    ]);
  }
}
