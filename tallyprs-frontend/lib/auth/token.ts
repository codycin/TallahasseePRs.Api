import { getAccessTokenFromStorage } from "../storage/authStorage";

export function getAccessToken(): string | null {
  return getAccessTokenFromStorage();
}

export function buildAuthHeader(): HeadersInit {
  const token = getAccessToken();

  if (!token) return {};

  return {
    Authorization: `Bearer ${token}`,
  };
}
