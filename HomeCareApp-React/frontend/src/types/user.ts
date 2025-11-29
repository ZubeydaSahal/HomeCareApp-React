export interface User {
  sub: string;
  email?: string;
  role?: string;
  name?: string;
  exp: number;
  [key: string]: any;
}
