import { Location } from './location.model';
export interface ILocationRepository {
  getAllLocation(): Promise<Location[]>;
}
