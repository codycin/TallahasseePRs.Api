"use client";

import { useEffect, useState } from "react";
import { getAccessTokenFromStorage } from "@/lib/storage/authStorage";
import Navbar from "./Navbar";
import NavbarGuest from "./NavbarGuest";

export default function AuthenticatedFooter() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const token = getAccessTokenFromStorage();
    setIsAuthenticated(!!token);
  }, []);

  if (!isAuthenticated) return <NavbarGuest />;

  return <Navbar />;
}
