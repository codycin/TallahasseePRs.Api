import { buildAuthHeader } from "@/lib/auth/token";
import { API_BASE_URL } from "@/lib/api";
import { RefreshAccessToken } from "@/services/authService";
import { ApiError } from "@/utils/apiError";

export async function apiFetch(
  endpoint: string,
  options: RequestInit = {},
  useRefresh: boolean = true,
  requireAuth: boolean = true,
): Promise<Response> {
  const url = `${API_BASE_URL}/api${endpoint.startsWith("/") ? endpoint : `/${endpoint}`}`;

  const isFormData = options.body instanceof FormData;

  function buildHeaders(): HeadersInit {
    const headers = new Headers(options.headers);

    const authHeader = buildAuthHeader(requireAuth);

    for (const [key, value] of Object.entries(authHeader)) {
      headers.set(key, value);
    }

    if (!isFormData && options.body && !headers.has("Content-Type")) {
      headers.set("Content-Type", "application/json");
    }

    return headers;
  }

  let response = await fetch(url, {
    ...options,
    headers: buildHeaders(),
  });

  if (response.status === 401 && requireAuth && useRefresh) {
    const refreshed = await RefreshAccessToken();

    if (refreshed?.accessToken) {
      response = await fetch(url, {
        ...options,
        headers: buildHeaders(),
      });
    }
  }

  if (!response.ok) {
    let errorBody: unknown = null;
    let message = `API failed with status ${response.status}`;

    try {
      errorBody = await response.json();

      if (
        typeof errorBody === "object" &&
        errorBody !== null &&
        "message" in errorBody &&
        typeof errorBody.message === "string"
      ) {
        message = errorBody.message;
      }
    } catch {
      // response body was not JSON
    }

    throw new ApiError(message, response.status, errorBody);
  }

  return response;
}
