import { ApiProperty } from '@nestjs/swagger';
import { Expose } from 'class-transformer';
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
 * Uses camelCase (TypeScript best practice) and maps to snake_case for Python API
 */
export class PredictionDto {
  @ApiProperty({
    example: 'Informatica',
    description: 'Current study program of the student',
  })
  @Expose({ name: 'current_study' })
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
  @Expose({ name: 'wanted_study_credit_range' })
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
  @Expose({ name: 'location_preference' })
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
  @Expose({ name: 'learning_goals' })
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
  @Expose({ name: 'level_preference' })
  @IsArray()
  @ArrayMinSize(1, { message: 'At least one level preference is required' })
  @IsString({ each: true })
  @IsNotEmpty({ each: true })
  levelPreference!: string[];

  @ApiProperty({
    example: 'Nederlands',
    description: 'Preferred teaching language',
  })
  @Expose({ name: 'preferred_language' })
  @IsString()
  @IsNotEmpty()
  preferredLanguage!: string;
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
