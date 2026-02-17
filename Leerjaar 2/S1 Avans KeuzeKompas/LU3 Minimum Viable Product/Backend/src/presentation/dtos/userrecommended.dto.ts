import { ApiProperty } from '@nestjs/swagger';
import { Type } from 'class-transformer';
import {
  ValidateNested,
  IsOptional,
  IsString,
  IsArray,
  IsInt,
  Min,
} from 'class-validator';

export class RecommendedModuleDto {
  @ApiProperty({
    type: Number,
    example: 12,
    description: 'Module ID',
  })
  @IsInt()
  @Min(1)
  moduleId!: number;

  @ApiProperty({
    type: String,
    example: 'Deze module ondersteunt jouw ambitie om...',
    description: 'AI-generated motivation for why this module was recommended',
    required: false,
  })
  @IsString()
  @IsOptional()
  motivation?: string;
}

export class SubmitRecommendedDto {
  @ApiProperty({
    type: [RecommendedModuleDto],
    example: [{ moduleId: 12, motivation: 'Based on your interest...' }],
    description: 'List of recommended modules with their motivations',
  })
  @IsArray()
  @ValidateNested({ each: true })
  @Type(() => RecommendedModuleDto)
  modules!: RecommendedModuleDto[];
}
