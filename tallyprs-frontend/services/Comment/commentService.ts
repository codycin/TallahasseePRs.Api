import { apiFetch } from "../apiClient";
import { commentRequest, CommentResponse } from "@/types/comment";

export async function createComment(request: commentRequest, postId: string) {
  const response = await apiFetch(`/posts/${postId}/comments`, {
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
      console.error("Failed to post comment: ", json);
      throw new Error(JSON.stringify(json, null, 2));
    }

    const text = await response.text();
    console.error("Create comment failed", text);
    throw new Error(text || "Comment post failed");
  }

  return response.json();
}
export async function createCommentReply(
  request: commentRequest,
  postId: string,
  parentId: string,
) {
  const response = await apiFetch(
    `/posts/${postId}/comments/${parentId}/replies`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
    },
  );

  if (!response.ok) {
    const contentType = response.headers.get("content-type") ?? "";

    if (contentType.includes("application/json")) {
      const json = await response.json();
      console.error("Failed to post comment: ", json);
      throw new Error(JSON.stringify(json, null, 2));
    }

    const text = await response.text();
    console.error("Create comment failed", text);
    throw new Error(text || "Comment post failed");
  }

  return response.json();
}

export async function getCommentsForPost(
  postId: string,
): Promise<CommentResponse[]> {
  const response = await apiFetch(`/posts/${postId}/comments`, {
    method: "GET",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to fetch comments for ${postId}`);
  }

  return response.json();
}

export async function deleteComment(commentId: string) {
  const response = await apiFetch(`/comments/${commentId}`, {
    method: "delete",
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
