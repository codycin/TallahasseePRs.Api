"use client";

import React, { createContext, useContext, useState, useEffect } from "react";
import {
  getAccessTokenFromStorage,
  setAccessTokenInStorage,
  removeAccessTokenFromStorage,
  removeRefreshTokenFromStorage,
} from "@/lib/storage/authStorage";
import { RefreshAccessToken } from "@/services/authService";

type AuthContextType = {
  isLoggedIn: boolean;
  isAuthLoading: boolean;
  login: (accessToken: string, refreshToken: string) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isAuthLoading, setIsAuthLoading] = useState(true);

  useEffect(() => {
    async function checkAuth() {
      try {
        const accessToken = getAccessTokenFromStorage();

        if (accessToken) {
          setIsLoggedIn(true);
          return;
        }

        const data = await RefreshAccessToken();
        if (!data) throw new Error("Refresh Token did not load");
        setAccessTokenInStorage(data.accessToken);

        setIsLoggedIn(true);
      } catch (error) {
        removeAccessTokenFromStorage();
        removeRefreshTokenFromStorage();
        setIsLoggedIn(false);
      } finally {
        setIsAuthLoading(false);
      }
    }

    checkAuth();
  }, []);

  const login = () => {
    setIsLoggedIn(true);
  };

  const logout = () => {
    removeAccessTokenFromStorage();
    removeRefreshTokenFromStorage();

    localStorage.removeItem("currentUserId");
    localStorage.removeItem("username");
    localStorage.removeItem("email");
    localStorage.removeItem("role");

    setIsLoggedIn(false);
  };

  return (
    <AuthContext.Provider
      value={{
        isLoggedIn,
        isAuthLoading,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }

  return context;
}
