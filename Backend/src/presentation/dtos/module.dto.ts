import { ApiProperty } from '@nestjs/swagger';
import { LocationDTO } from './location.dto';
import { ModuleTagDTO } from './moduleTag.dto';
import { Type } from 'class-transformer';
import {
  IsString,
  IsNumber,
  IsArray,
  IsNotEmpty,
  MinLength,
  Min,
  IsDateString,
  ArrayMinSize,
  IsIn,
} from 'class-validator';

export class CreateModuleDTO {
  @ApiProperty({ description: 'Module name' })
  @IsString()
  @IsNotEmpty({ message: 'Module name is required' })
  @MinLength(3, { message: 'Module name must be at least 3 characters' })
  name!: string;

  @ApiProperty({ description: 'Module description' })
  @IsString()
  @IsNotEmpty({ message: 'Module description is required' })
  @MinLength(10, {
    message: 'Module description must be at least 10 characters',
  })
  description!: string;

  @ApiProperty({ description: 'Module content' })
  @IsString()
  @IsNotEmpty({ message: 'Module content is required' })
  content!: string;

  @ApiProperty({ description: 'NLQF level', enum: ['NLQF5', 'NLQF6'] })
  @IsString()
  @IsIn(['NLQF5', 'NLQF6'], { message: 'Level must be either NLQF5 or NLQF6' })
  @IsNotEmpty({ message: 'Level is required' })
  level!: string;

  @ApiProperty({ description: 'Study credits (15 or 30)', enum: [15, 30] })
  @IsNumber()
  @IsIn([15, 30], { message: 'Study credits must be either 15 or 30' })
  studyCredits!: number;

  @ApiProperty({ description: 'Module locations', type: [LocationDTO] })
  @IsArray()
  @ArrayMinSize(1, { message: 'At least one location is required' })
  @Type(() => LocationDTO)
  location!: { id: number; name: string }[];

  @ApiProperty({ description: 'Module tags', type: [ModuleTagDTO] })
  @IsArray()
  @ArrayMinSize(1, { message: 'At least one module tag is required' })
  @Type(() => ModuleTagDTO)
  moduleTags!: { id: number; name: string }[];

  @ApiProperty({ description: 'Learning outcomes' })
  @IsString()
  @IsNotEmpty({ message: 'At least one learning outcome is required' })
  learningOutcomes!: string;

  @ApiProperty({ description: 'Available spots' })
  @IsNumber()
  @Min(0)
  availableSpots!: number;

  @ApiProperty({ description: 'Start date' })
  @IsDateString()
  startDate!: string;

  @ApiProperty({ description: 'Short description' })
  @IsString()
  @IsNotEmpty({ message: 'Short description is required' })
  shortDescription!: string;
}

export class ModuleDTO {
  @ApiProperty()
  id: number;

  @ApiProperty()
  name: string;

  @ApiProperty()
  shortDescription: string;

  @ApiProperty()
  description: string;

  @ApiProperty()
  content: string;

  @ApiProperty()
  level: string;

  @ApiProperty()
  studentCredits: number;

  @ApiProperty()
  location: string;

  @ApiProperty()
  moduleTags: string[];

  @ApiProperty()
  learningOutcomes: string;

  @ApiProperty()
  availableSpots: number;

  @ApiProperty()
  startDate: string;
}
