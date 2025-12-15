import { type IAuthService } from "src/application/ports/auth.port";
import { type loginDto } from "../dtos/login.dto";
import { ApiTags } from "@nestjs/swagger";
import { SERVICES } from "src/di-tokens";
import { type Response } from "express";
import { nodeEnv } from "src/constants";
import {
  UnauthorizedException,
  HttpStatus,
  Controller,
  HttpCode,
  Logger,
  Inject,
  Body,
  Post,
  Res,
} from "@nestjs/common";

@ApiTags("auth")
@Controller("auth")
export class AuthController {
  private readonly logger: Logger;

  constructor(
    @Inject(SERVICES.AUTH)
    private readonly authService: IAuthService,
  ) {
    this.logger = new Logger("AuthController");
  }

  @HttpCode(HttpStatus.OK)
  @Post("login")
  public async login(
    @Body() dto: loginDto,
    @Res({ passthrough: true }) res: Response,
  ): Promise<{ accessToken: string }> {
    const result = await this.authService.login(dto);

    if (!result.ok) {
      this.logger.warn(`Login failed: ${result.error.code} - ${result.error.message}`);
      throw new UnauthorizedException(result.error.message || "Authentication failed");
    }

    res.cookie("ACCESSTOKEN", result.data.accessToken, {
      httpOnly: nodeEnv === "production",
      secure: nodeEnv === "production",
      sameSite: "lax",
      maxAge: 3600000, // 1 hour
    });
    return { accessToken: result.data.accessToken };
  }
}
