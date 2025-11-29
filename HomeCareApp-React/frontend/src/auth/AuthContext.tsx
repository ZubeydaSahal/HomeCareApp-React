import React, { createContext, useState, useContext, useEffect, ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';
import { User } from '../types/user';
import { LoginDto } from '../types/auth';
import * as authService from './AuthService';

interface AuthContextType { // Define the shape of the auth context
    user: User | null;
    token: string | null;
    login: (credentials: LoginDto) => Promise<void>;
    logout: () => void;
    isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);
    const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
    const [isLoading, setIsLoading] = useState<boolean>(true);
    useEffect(() => {
        if (token) {
            try {
                const decodedRaw: any = jwtDecode(token);
    
                const mappedUser: User = {
                    ...decodedRaw,
                    role: decodedRaw["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"], 
                };
    
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
    
        const mappedUser: User = {
            ...decodedRaw,
            role: decodedRaw["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"], 
        };
    
        setUser(mappedUser);
        setToken(token);
    };
    
    
    const logout = () => { // Clear token and user data
        localStorage.removeItem('token');
        setUser(null);
        setToken(null);
    };

    return ( // Provide context to children
        <AuthContext.Provider value={{ user, token, login, logout, isLoading }}>
            {!isLoading && children}
        </AuthContext.Provider>
    );
};

export const useAuth = (): AuthContextType => { // Custom hook to use auth context
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};