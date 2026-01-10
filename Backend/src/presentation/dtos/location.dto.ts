import { ApiProperty } from '@nestjs/swagger';
import { IsString, IsNumber, IsNotEmpty } from 'class-validator';

export class LocationDTO {
  @ApiProperty()
  @IsNumber()
  @IsNotEmpty()
  id: number;
  @ApiProperty()
  @IsString()
  @IsNotEmpty()
  name: string;
}
