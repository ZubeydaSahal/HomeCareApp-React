export interface User {
    sub: string;        // brukernavn
    email?: string;
    jti?: string;
    exp: number;        // utløpstid (viktig!)
    iat: number;        // issued at
    role?: string | string[];  // hvis du vil lese roller senere
  }
  