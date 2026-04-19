"use client";

import { useEffect, useState } from "react";
import { getAccessTokenFromStorage } from "@/lib/storage/authStorage";
import Navbar from "./Navbar";
import NavbarGuest from "./NavbarGuest";
import { useAuth } from "@/lib/auth/authContext";

export default function AuthenticatedNavbar() {
  const { isLoggedIn, logout } = useAuth();

  if (!isLoggedIn) return <NavbarGuest />;

  return <Navbar />;
}
