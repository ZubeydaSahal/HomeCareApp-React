export interface User {
  sub: string;
  email?: string;
  role?: string;
  exp: number;
  [key: string]: any; // beholder alle andre claims
}
