import { ApiProperty } from '@nestjs/swagger';
import { IsString, IsNumber, IsNotEmpty, MinLength } from 'class-validator';

export class ModuleTagDTO {
  @ApiProperty()
  @IsNumber()
  @IsNotEmpty()
  id: number;

  @ApiProperty()
  @IsString()
  @IsNotEmpty()
  name: string;
}

export class CreateModuleTagDto {
  @ApiProperty({
    example: 'Web Development',
    description: 'Name of the module tag',
  })
  @IsString()
  @IsNotEmpty({ message: 'Tag name is required' })
  @MinLength(1, { message: 'Tag name cannot be empty' })
  tag!: string;
}
