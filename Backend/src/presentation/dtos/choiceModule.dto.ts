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
} from 'class-validator';

export class CreateModuleDTO {
  @ApiProperty()
  @IsString()
  @IsNotEmpty()
  @MinLength(3)
  name: string;

  @ApiProperty()
  @IsString()
  @IsNotEmpty()
  @MinLength(10)
  description: string;

  @ApiProperty()
  @IsString()
  @IsNotEmpty()
  content: string;

  @ApiProperty()
  @IsString()
  @IsNotEmpty()
  level: string;

  @ApiProperty()
  @IsNumber()
  @Min(0)
  studyCredits: number;

  @ApiProperty()
  @IsArray()
  @Type(() => LocationDTO)
  location: { id: string; name: string }[];

  @ApiProperty()
  @IsArray()
  @Type(() => ModuleTagDTO)
  moduleTags: { id: string; name: string }[];

  @ApiProperty()
  @IsString()
  @IsNotEmpty()
  learningOutcomes: string;

  @ApiProperty()
  @IsNumber()
  @Min(0)
  availableSpots: number;

  @ApiProperty()
  @IsDateString()
  startDate: string;

  @ApiProperty()
  @IsString()
  @IsNotEmpty()
  shortdescription: string;
}

export class ChoiceModuleDTO {
  @ApiProperty()
  id: string;

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
