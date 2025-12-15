import { Result } from "../../domain/result";

//* Login
export type loginRequest = {
  email: string;
  password: string;
};

export type loginResponse = {
  accessToken: string;
};

//* Auth Service Interface
export interface IAuthService {
  login(req: loginRequest): Promise<Result<loginResponse>>;
}
