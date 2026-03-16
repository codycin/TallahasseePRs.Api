import { API_BASE_URL } from "@/lib/api";

export async function getPosts() {
  const response = await fetch(`${API_BASE_URL}/api/posts`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch posts");
  }

  return response.json();
}
