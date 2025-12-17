import { ApiProperty } from '@nestjs/swagger';
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
