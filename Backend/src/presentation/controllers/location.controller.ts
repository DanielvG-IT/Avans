import { Controller, Get } from '@nestjs/common';
import { ILocationService } from '@/application/ports/location.port';
import { Inject } from '@nestjs/common';
import { Location } from '@/domain/locations/location.model';

@Controller('locations')
export class LocationController {
  private readonly locationService: ILocationService;
  constructor(@Inject('SERVICE.LOCATION') locationService: ILocationService) {
    this.locationService = locationService;
  }
  @Get()
  async getAllLocation(): Promise<Location[]> {
    return await this.locationService.getAllLocation();
  }
}
