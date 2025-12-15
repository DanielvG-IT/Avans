import { JwtService } from "@nestjs/jwt";
import { SERVICES } from "../../di-tokens";
import { type IUserService } from "../ports/user.port";
import { Inject, Injectable, Logger } from "@nestjs/common";
import { loginResponse, loginRequest, IAuthService } from "../ports/auth.port";
import { Result, ok, err } from "../../domain/result";
import { PasswordUtil } from "../utils/password.util";

@Injectable()
export class AuthService implements IAuthService {
  private readonly logger = new Logger(AuthService.name);

  constructor(
    @Inject(SERVICES.USER)
    private readonly userService: IUserService,
    private jwtService: JwtService,
  ) {}

  public async login(data: loginRequest): Promise<Result<loginResponse>> {
    if (!data || !data.email || !data.password) {
      return err("INVALID_INPUT", "Email and password are required");
    }

    const userResult = await this.userService.getUserByEmail(data.email);
    if (!userResult.ok) {
      return err("INVALID_CREDENTIALS", "Invalid email or password");
    }

    const user = userResult.data;
    const isPasswordValid = await PasswordUtil.compare(data.password, user.passwordHash);
    if (!isPasswordValid) {
      return err("INVALID_CREDENTIALS", "Invalid email or password");
    }

    try {
      const accessToken = await this.jwtService.signAsync({
        sub: user.id,
        email: user.email,
        role: user.role,
        first: user.firstName,
        last: user.lastName,
      });
      return ok({ accessToken });
    } catch (error) {
      this.logger.error("Failed to sign JWT", error);
      return err("JWT_SIGN_ERROR", "Failed to generate access token");
    }
  }
}
