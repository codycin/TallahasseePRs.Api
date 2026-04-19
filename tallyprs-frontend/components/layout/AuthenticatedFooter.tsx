"use client";

import { useEffect, useState } from "react";
import { getAccessTokenFromStorage } from "@/lib/storage/authStorage";
import Footer from "./Footer";
import { useAuth } from "@/lib/auth/authContext";

export default function AuthenticatedFooter() {
  const { isLoggedIn, logout } = useAuth();

  if (!isLoggedIn) return null;

  return <Footer />;
}
