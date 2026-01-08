import { ApiProperty } from '@nestjs/swagger';
import {
  IsString,
  IsArray,
  IsNumber,
  ArrayMinSize,
  ArrayMaxSize,
  IsNotEmpty,
  MinLength,
} from 'class-validator';

/**
 * DTO for AI prediction requests
 * Accepts camelCase from frontend, will be converted to snake_case for Python service
 */
export class PredictionDto {
  @ApiProperty({
    example: 'Informatica',
    description: 'Current study program of the student',
  })
  @IsString()
  @IsNotEmpty()
  @MinLength(2)
  currentStudy!: string;

  @ApiProperty({
    example: ['AI', 'Machine Learning', 'Data Science'],
    description: 'Student interests (at least 1 required)',
    type: [String],
  })
  @IsArray()
  @ArrayMinSize(1, { message: 'At least one interest is required' })
  @IsString({ each: true })
  @IsNotEmpty({ each: true })
  interests!: string[];

  @ApiProperty({
    example: [15, 25],
    description: 'Study credit range [min, max] in EC',
    type: [Number],
  })
  @IsArray()
  @ArrayMinSize(2, { message: 'Credit range must have min and max values' })
  @ArrayMaxSize(2, { message: 'Credit range must have exactly 2 values' })
  @IsNumber({}, { each: true })
  wantedStudyCreditRange!: [number, number];

  @ApiProperty({
    example: ['Tilburg', 'Breda'],
    description: 'Preferred study locations (at least 1 required)',
    type: [String],
  })
  @IsArray()
  @ArrayMinSize(1, { message: 'At least one location preference is required' })
  @IsString({ each: true })
  @IsNotEmpty({ each: true })
  locationPreference!: string[];

  @ApiProperty({
    example: [
      'Verbeteren van programmeervaardigheden',
      'Projectervaring opdoen',
    ],
    description: 'Learning goals for the module (at least 1 required)',
    type: [String],
  })
  @IsArray()
  @ArrayMinSize(1, { message: 'At least one learning goal is required' })
  @IsString({ each: true })
  @IsNotEmpty({ each: true })
  learningGoals!: string[];

  @ApiProperty({
    example: ['NLQF5', 'NLQF6'],
    description: 'Preferred education level (NLQF) (at least 1 required)',
    type: [String],
  })
  @IsArray()
  @ArrayMinSize(1, { message: 'At least one level preference is required' })
  @IsString({ each: true })
  @IsNotEmpty({ each: true })
  levelPreference!: string[];

  @ApiProperty({
    example: 'Nederlands',
    description: 'Preferred teaching language',
  })
  @IsString()
  @IsNotEmpty()
  preferredLanguage!: string;

  @ApiProperty({
    example: ['P1', 'P2', 'P3', 'P4'],
    description: 'Preferred study periods (at least 1 required)',
    type: [String],
  })
  @IsArray()
  @ArrayMinSize(1, { message: 'At least one period preference is required' })
  @IsString({ each: true })
  @IsNotEmpty({ each: true })
  preferredPeriod!: string[];
}

/**
 * DTO for a single module prediction match
 */
export class ModulePredictionDto {
  @ApiProperty({
    description: 'Full module details',
  })
  module!: {
    id: string;
    name: string;
    shortdescription: string;
    studyCredits: number;
    level: string;
    location: { id: string; name: string }[];
    startDate: string;
  };

  @ApiProperty({
    example: 0.87,
    description: 'Similarity score between 0 and 1 indicating match quality',
    minimum: 0,
    maximum: 1,
  })
  @IsNumber()
  score!: number;
}

/**
 * Response DTO for prediction endpoint
 */
export class PredictionResponseDto {
  @ApiProperty({
    type: [ModulePredictionDto],
    description: 'List of predicted module matches',
  })
  predictions!: ModulePredictionDto[];
}
