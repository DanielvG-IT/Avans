export interface AccessToken {
  sub: string;
  email: string;
  first: string;
  last: string;
  role: "student" | "teacher" | "admin";
  iat: number;
  exp: number;
}
