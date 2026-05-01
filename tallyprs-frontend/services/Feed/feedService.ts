import { PostResponse } from "@/types/post";
import { FeedPage } from "@/types/feed";
import { apiFetch } from "../apiClient";

export type MainFeedType = "Global" | "Following";

export async function getPostFeed(
  type: MainFeedType = "Global",
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
    throw new Error(errorText || "Failed to fetch post feed");
  }

  return response.json();
}

export async function getUserPostFeed(
  userId: string,
  limit: number = 20,
  cursor?: string,
): Promise<FeedPage<PostResponse>> {
  const params = new URLSearchParams();

  params.set("limit", limit.toString());

  if (cursor) {
    params.set("cursor", cursor);
  }

  const response = await apiFetch(
    `/profiles/${userId}/posts?${params.toString()}`,
    {
      method: "GET",
    },
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Failed to fetch user posts");
  }

  return response.json();
}
