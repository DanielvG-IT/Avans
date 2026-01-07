import { ApiProperty } from '@nestjs/swagger';
import {
  IsString,
  IsArray,
  IsNumber,
  ArrayMinSize,
  ArrayMaxSize,
} from 'class-validator';

export class PredictionDto {
  @ApiProperty({ example: 'Informatica' })
  @IsString()
  current_study!: string;

  @ApiProperty({ example: ['AI', 'Data Science'] })
  @IsString({ each: true })
  interests!: string[];

  @ApiProperty({ example: [30, 60] })
  @IsArray()
  @ArrayMinSize(2)
  @ArrayMaxSize(2)
  @IsNumber({}, { each: true })
  wanted_study_credit_range!: [number, number];

  @ApiProperty({ example: ['Breda', 'Tilburg'] })
  @IsString({ each: true })
  location_preference!: string[];

  @ApiProperty({ example: ['Research skills', 'Teamwork'] })
  @IsString({ each: true })
  learning_goals!: string[];

  @ApiProperty({ example: ['NLQF5', 'NLQF6'] })
  @IsString({ each: true })
  level_preference!: string[];

  @ApiProperty({ example: 'English' })
  @IsString()
  preferred_language!: string;
}
