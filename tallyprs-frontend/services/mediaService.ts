import { apiFetch } from "@/services/apiClient";
import { MediaPurpose } from "@/types/media";
import { CreateMediaUploadRequest } from "@/types/media";
import { CreateMediaUploadResponse } from "@/types/media";
import { MediaResponse } from "@/types/media";

export async function createMediaUpload(
  request: CreateMediaUploadRequest,
): Promise<CreateMediaUploadResponse> {
  const response = await apiFetch("/media/uploads", {
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
      console.error("Create media upload failed:", json);
      throw new Error(JSON.stringify(json, null, 2));
    }

    const text = await response.text();
    console.error("Create media upload failed:", text);
    throw new Error(text || "Failed to create media upload.");
  }

  return response.json();
}

export async function uploadFileToPresignedUrl(
  uploadUrl: string,
  file: File,
): Promise<void> {
  const response = await fetch(uploadUrl, {
    method: "PUT",
    headers: {
      "Content-Type": file.type,
    },
    body: file,
  });

  if (!response.ok) {
    throw new Error(`Storage upload failed with status ${response.status}.`);
  }
}

export async function completeMediaUpload(
  mediaId: string,
): Promise<MediaResponse> {
  const response = await apiFetch(`/media/${mediaId}/complete`, {
    method: "POST",
  });

  if (!response.ok) {
    const message = await safeReadError(response);
    throw new Error(message || "Failed to complete media upload.");
  }

  return response.json();
}

export async function uploadSingleMedia(
  file: File,
  purpose: MediaPurpose,
  options?: {
    postId?: string | null;
    commentId?: string | null;
    profileId?: string | null;
  },
): Promise<MediaResponse> {
  const request: CreateMediaUploadRequest = {
    fileName: file.name,
    contentType: file.type,
    sizeBytes: file.size,
    purpose,
    postId: options?.postId ?? null,
    commentId: options?.commentId ?? null,
    profileId: options?.profileId ?? null,
  };

  const init = await createMediaUpload(request);
  await uploadFileToPresignedUrl(init.uploadUrl, file);

  const media = await completeMediaUpload(init.mediaId);

  if (media.kind === "Video" && media.status === "Processing") {
    //Video and processing
    return await waitForVideoReady(media.id);
  }

  return media;
}

export async function uploadMultipleMedia(
  files: File[],
  purposeResolver: (file: File) => MediaPurpose,
  options?: {
    postId?: string | null;
    commentId?: string | null;
    profileId?: string | null;
  },
): Promise<MediaResponse[]> {
  const results: MediaResponse[] = [];

  for (const file of files) {
    const purpose = purposeResolver(file);
    const media = await uploadSingleMedia(file, purpose, options);
    results.push(media);
  }

  return results;
}

async function safeReadError(response: Response): Promise<string | null> {
  try {
    const contentType = response.headers.get("content-type") ?? "";

    if (contentType.includes("application/json")) {
      const json = await response.json();
      return (
        json?.title || json?.message || json?.detail || JSON.stringify(json)
      );
    }

    return await response.text();
  } catch {
    return null;
  }
}

export async function waitForVideoReady(
  mediaId: string,
  timeoutMs = 30000,
  intervalMs = 1500,
): Promise<MediaResponse> {
  const started = Date.now();

  while (Date.now() - started < timeoutMs) {
    const media = await getMediaById(mediaId);

    if (media.status === "Ready") {
      //Ready
      return media;
    }

    if (media.status === "Failed") {
      //Failed
      throw new Error("Video processing failed.");
    }

    await new Promise((resolve) => setTimeout(resolve, intervalMs));
  }

  throw new Error("Timed out waiting for video processing.");
}

export async function getMediaById(mediaId: string): Promise<MediaResponse> {
  const response = await apiFetch(`/media/${mediaId}`, {
    method: "GET",
  });

  if (!response.ok) {
    const message = await safeReadError(response);
    throw new Error(message || "Failed to fetch media.");
  }

  return response.json();
}
