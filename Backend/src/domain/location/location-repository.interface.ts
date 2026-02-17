import { Location } from './location.model';
export interface ILocationRepository {
  getAllLocations(): Promise<Location[]>;
}
