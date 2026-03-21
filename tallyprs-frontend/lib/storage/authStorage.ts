// src/lib/storage/authStorage.ts
const ACCESS_TOKEN_KEY = "accessToken";

export function getAccessTokenFromStorage(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(ACCESS_TOKEN_KEY);
}

export function setAccessTokenInStorage(token: string): void {
  localStorage.setItem(ACCESS_TOKEN_KEY, token);
}

export function removeAccessTokenFromStorage(): void {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
}
