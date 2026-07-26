import type { AuthResponse, LoginRequest, RegisterRequest } from "@/types/auth";
import { API_BASE_URL } from "@/lib/api";
import {
  getRefreshTokenFromStorage,
  setAccessTokenInStorage,
  setRefreshTokenInStorage,
} from "@/lib/storage/authStorage";

let refreshPromise: Promise<AuthResponse | null> | null = null;

export async function registerUser(
  request: RegisterRequest,
): Promise<AuthResponse> {
  const response = await fetch(`${API_BASE_URL}/api/auth/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Registration Failed");
  }

  const result = (await response.json()) as AuthResponse;

  localStorage.setItem("currentUserId", result.user.id);
  localStorage.setItem("username", result.user.userName);
  localStorage.setItem("email", result.user.email);
  localStorage.setItem("role", result.user.role);

  setAccessTokenInStorage(result.accessToken);
  setRefreshTokenInStorage(result.refreshToken);

  return result;
}

export async function loginUser(request: LoginRequest): Promise<AuthResponse> {
  const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Login Failed");
  }

  const result = (await response.json()) as AuthResponse;

  localStorage.setItem("currentUserId", result.user.id);
  localStorage.setItem("username", result.user.userName);
  localStorage.setItem("email", result.user.email);
  localStorage.setItem("role", result.user.role);

  setAccessTokenInStorage(result.accessToken);
  setRefreshTokenInStorage(result.refreshToken);

  return result;
}

async function refreshAccessTokenInternal(): Promise<AuthResponse | null> {
  const refreshToken = getRefreshTokenFromStorage();

  if (!refreshToken) return null;

  const response = await fetch(`${API_BASE_URL}/api/auth/refresh`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ refreshToken }),
  });

  const text = await response.text();

  if (!response.ok) {
    return null;
  }

  const token = JSON.parse(text) as AuthResponse;

  setAccessTokenInStorage(token.accessToken);
  setRefreshTokenInStorage(token.refreshToken);

  return token;
}

export async function RefreshAccessToken(): Promise<AuthResponse | null> {
  if (!refreshPromise) {
    refreshPromise = refreshAccessTokenInternal().finally(() => {
      refreshPromise = null;
    });
  }

  return refreshPromise;
}
