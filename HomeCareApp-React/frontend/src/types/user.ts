export interface User {
    sub: string; // Subject (username)
    email: string;
    nameid: string; // User ID
    jti: string;
    iat: number;
    exp: number;
    iss: string;
    aud:string;
}