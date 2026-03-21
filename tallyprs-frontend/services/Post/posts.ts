import { apiFetch } from "../apiClient";
import { CreatePostRequest } from "@/types/post";

export async function getPosts() {
  const response = await apiFetch(`/posts`, {
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

export async function createPost(request: CreatePostRequest) {
  const response = await apiFetch("/posts", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    const contentType = response.headers.get("content-type") ?? "";

    if (contentType.includes("application/json")) {
      const json = await response.json();
      console.error("Create post failed:", json);
      throw new Error(JSON.stringify(json, null, 2));
    }

    const text = await response.text();
    console.error("Create post failed:", text);
    throw new Error(text || "Failed to create post.");
  }

  return response.json();
}
