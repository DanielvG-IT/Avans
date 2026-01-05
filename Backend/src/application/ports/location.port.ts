import { Location } from '@/domain/locations/location.model';
export interface ILocationService {
  getAllLocation(): Promise<Location[]>;
}
