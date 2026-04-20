export enum MediaPurpose {
  Post = 1,
  Comment = 2,
  ProfilePicture = 3,
}
export enum MediaStatus {
  Pending = 1,
  Processing = 2,
  Ready = 3,
  Failed = 4,
  Deleted = 5,
}

export enum MediaKind {
  Image = 1,
  Video = 2,
}

export type CreateMediaUploadRequest = {
  fileName: string;
  contentType: string;
  sizeBytes: number;
  purpose: MediaPurpose;
  postId?: string | null;
  commentId?: string | null;
  profileId?: string | null;
};

export type CreateMediaUploadResponse = {
  mediaId: string;
  uploadUrl: string;
  storageKey: string;
  publicUrl?: string | null;
};

export type MediaResponse = {
  id: string;
  url: string;
  thumbnailUrl?: string | null;
  kind: "Image" | "Video";
  purpose: string;
  status: string;
  contentType: string;
  originalFileName: string;
  sizeBytes: number;
  width?: number | null;
  height?: number | null;
  durationSeconds?: number | null;
  createdAt: string;
};
