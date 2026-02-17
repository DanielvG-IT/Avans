import { ApiProperty } from '@nestjs/swagger';

export class LoginResponseDto {
  @ApiProperty()
  name: string;

  @ApiProperty()
  role: string;
}
