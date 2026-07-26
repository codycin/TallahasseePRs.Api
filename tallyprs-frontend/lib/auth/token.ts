import { getAccessTokenFromStorage } from "@/lib/storage/authStorage";

export function getAccessToken(): string | null {
  return getAccessTokenFromStorage();
}

export function buildAuthHeader(requireAuth: boolean = true): HeadersInit {
  if (!requireAuth) return {};

  const token = getAccessToken();

  if (!token) return {};

  return {
    Authorization: `Bearer ${token}`,
  };
}
