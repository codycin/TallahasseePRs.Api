import { followRequest, followResponse } from "@/types/follow";
import { apiFetch } from "../apiClient";

export async function followUser(
  request: followRequest,
): Promise<followResponse> {
  const response = await apiFetch("/follow", {
    method: "POST",
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error(
      `API Error Status: ${response.status} ${response.statusText}`,
    );
    console.error(`API Error Details: ${errorText}`);
    throw new Error(`API failed with status ${response.status}`);
  }

  const data = await response.json();

  return data;
}

export async function unfollowUser(followedId: string): Promise<void> {
  const response = await apiFetch(`/follow/${followedId}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error(
      `API Error Status: ${response.status} ${response.statusText}`,
    );
    console.error(`API Error Details: ${errorText}`);
    throw new Error(`API failed with status ${response.status}`);
  }
}
