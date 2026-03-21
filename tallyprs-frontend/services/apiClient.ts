import { buildAuthHeader } from "@/lib/auth/token";
import { API_BASE_URL } from "@/lib/api";

export async function apiFetch(
  endpoint: string,
  options: RequestInit = {},
): Promise<Response> {
  const authHeader = buildAuthHeader();

  const headers: HeadersInit = {
    ...authHeader,
    ...(options.headers ?? {}),
  };

  const isFormData = options.body instanceof FormData;

  if (!isFormData && !("Content-Type" in (headers as Record<string, string>))) {
    (headers as Record<string, string>)["Content-Type"] = "application/json";
  }

  return fetch(`${API_BASE_URL}/api${endpoint}`, {
    ...options,
    headers,
  });
}
