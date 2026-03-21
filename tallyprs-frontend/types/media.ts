export enum MediaPurpose {
  Post = 1,
  Comment = 2,
  ProfilePicture = 3,
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
  fileName: string;
  contentType: string;
  sizeBytes: number;
  purpose: MediaPurpose;
  url: string;
  postId?: string | null;
  commentId?: string | null;
  profileId?: string | null;
  createdAt: string;
};
