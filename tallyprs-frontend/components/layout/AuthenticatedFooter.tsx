"use client";

import { useEffect, useState } from "react";
import { getAccessTokenFromStorage } from "@/lib/storage/authStorage";
import Footer from "./Footer";

export default function AuthenticatedFooter() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const token = getAccessTokenFromStorage();
    setIsAuthenticated(!!token);
  }, []);

  if (!isAuthenticated) return null;

  return <Footer />;
}
