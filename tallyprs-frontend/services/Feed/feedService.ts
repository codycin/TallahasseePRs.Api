import { PostResponse } from "@/types/post";
import { FeedPage } from "@/types/feed";
import { apiFetch } from "../apiClient";

export async function getPostFeed(
  type: "Global" | "Following" | "User" = "Global",
  limit: number = 20,
  cursor?: string,
): Promise<FeedPage<PostResponse>> {
  const params = new URLSearchParams();
  params.set("type", type);
  params.set("limit", limit.toString());

  if (cursor) {
    params.set("cursor", cursor);
  }

  const response = await apiFetch(`/feed?${params.toString()}`, {
    method: "GET",
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to fetch Post Feed");
  }

  return response.json();
}
