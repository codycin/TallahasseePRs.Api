import type { AuthResponse, LoginRequest, RegisterRequest } from "@/types/auth";
import { apiFetch } from "./apiClient";
import { setAccessTokenInStorage } from "@/lib/storage/authStorage";

export async function registerUser(
  request: RegisterRequest,
): Promise<AuthResponse> {
  const response = await apiFetch(`/auth/register`, {
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

  const data = await response.json();

  setAccessTokenInStorage(data.accessToken);
  localStorage.setItem("userId", data.userId);

  return data;
}

export async function loginUser(request: LoginRequest): Promise<AuthResponse> {
  const response = await apiFetch(`/auth/login`, {
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

  const data = await response.json();

  setAccessTokenInStorage(data.accessToken);

  console.log("user id from response:", data.user.id);

  localStorage.setItem("currentUserId", data.user.id);

  localStorage.setItem("testKey", "hello");
  console.log(localStorage.getItem("testKey"));

  console.log("user id from localStorage:", localStorage.getItem("userId"));

  return data;
}
