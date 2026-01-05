import { Injectable, Inject } from '@nestjs/common';
import type { Location } from '@/domain/locations/location.model';
import { ILocationService } from '../ports/location.port';
import { ILocationRepository } from '@/domain/locations/location-repository.interface';
@Injectable()
export class LocationService implements ILocationService {
  private readonly locationRepository: ILocationRepository;
  constructor(
    @Inject('REPO.LOCATION') locationRepository: ILocationRepository,
  ) {
    this.locationRepository = locationRepository;
  }
  async getAllLocation(): Promise<Location[]> {
    return await this.locationRepository.getAllLocation();
  }
}
