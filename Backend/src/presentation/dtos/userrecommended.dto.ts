import { ApiProperty } from '@nestjs/swagger';
import { ArrayUnique, IsArray, IsInt, Min } from 'class-validator';

export class SubmitRecommendedDto {
  @ApiProperty({
    type: [Number],
    example: [12, 34, 56],
    description: 'List of ChoiceModule IDs the user accepts as recommended',
  })
  @IsArray()
  @ArrayUnique()
  @IsInt({ each: true })
  @Min(1, { each: true })
  moduleIds!: number[];
}
