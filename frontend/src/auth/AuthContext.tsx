import React, {
  createContext,
  useState,
  useContext,
  useEffect,
  ReactNode,
} from "react";
import { jwtDecode } from "jwt-decode";
import { User } from "../types/user";
import { LoginDto } from "../types/auth";
import * as authService from "./AuthService";

interface AuthContextType {
  user: User | null;
  token: string | null;
  login: (credentials: LoginDto) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(
    localStorage.getItem("token")
  );
  const [isLoading, setIsLoading] = useState<boolean>(true);

  const roleClaim =
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
  const nameClaim =
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

  // Hjelpefunksjon: plukk ut en rolle fra claim(s)
  const mapPrimaryRole = (rawRole: any): string | undefined => {
    const roles: string[] = Array.isArray(rawRole)
      ? rawRole
      : rawRole
        ? [rawRole]
        : [];

    if (roles.includes("Admin")) return "Admin";
    if (roles.includes("Personnel")) return "Personnel";
    if (roles.includes("Patient")) return "Patient";
    return roles[0];
  };

  const mapUser = (decodedRaw: any): User => {
    const primaryRole = mapPrimaryRole(decodedRaw[roleClaim]);

    const mappedUser: User = {
      ...decodedRaw,
      role: primaryRole,
      name:
        decodedRaw[nameClaim] ??
        decodedRaw.name ??
        (decodedRaw.email
          ? decodedRaw.email.split("@")[0]
          : decodedRaw.sub
            ? decodedRaw.sub.split("@")[0]
            : undefined),
    };

    return mappedUser;
  };

  useEffect(() => {
    if (token) {
      try {
        const decodedRaw: any = jwtDecode(token);
        const mappedUser = mapUser(decodedRaw);

        if (mappedUser.exp * 1000 > Date.now()) {
          setUser(mappedUser);
        } else {
          console.warn("Token expired");
          localStorage.removeItem("token");
          setUser(null);
          setToken(null);
        }
      } catch (error) {
        console.error("Invalid token", error);
        localStorage.removeItem("token");
        setUser(null);
        setToken(null);
      }
    } else {
      setUser(null);
    }

    setIsLoading(false);
  }, [token]);

  const login = async (credentials: LoginDto) => {
    const { token } = await authService.login(credentials);
    localStorage.setItem("token", token);

    const decodedRaw: any = jwtDecode(token);
    const mappedUser = mapUser(decodedRaw);

    setUser(mappedUser);
    setToken(token);
  };

  const logout = () => {
    localStorage.removeItem("token");
    setUser(null);
    setToken(null);
  };

  return (
    <AuthContext.Provider value={{ user, token, login, logout, isLoading }}>
      {!isLoading && children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
