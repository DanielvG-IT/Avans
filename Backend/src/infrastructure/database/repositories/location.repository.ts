import { Injectable } from '@nestjs/common';
import { ILocationRepository } from '@/domain/locations/location-repository.interface';
import { Location } from '@/domain/locations/location.model';
import { PrismaService } from '../prisma';

@Injectable()
export class LocationRepository implements ILocationRepository {
  constructor(private readonly prisma: PrismaService) {}

  async getAllLocation(): Promise<Location[]> {
    const locations = await this.prisma.location.findMany();
    return locations.map((loc) => ({
      id: loc.id,
      name: loc.name,
    }));
  }
}
