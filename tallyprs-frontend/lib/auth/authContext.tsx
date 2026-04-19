"use client";

import React, { createContext, useContext, useState, useEffect } from "react";

// 1. Define the shape of our context
type AuthContextType = {
  isLoggedIn: boolean;
  login: (token: string) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// 2. Create the Provider component
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  // Check for the token when the app first loads
  useEffect(() => {
    // Replace "access_token" with whatever key you use in storage
    const token = localStorage.getItem("access_token");
    setIsLoggedIn(!!token);
  }, []);

  // 3. Create actions to update state AND local storage simultaneously
  const login = (token: string) => {
    localStorage.setItem("access_token", token);
    setIsLoggedIn(true); // This triggers the instant re-render!
  };

  const logout = () => {
    localStorage.removeItem("access_token");
    setIsLoggedIn(false); // This triggers the instant re-render!
  };

  return (
    <AuthContext.Provider value={{ isLoggedIn, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

// 4. Create a custom hook for easy access
export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
