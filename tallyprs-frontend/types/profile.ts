import { MediaResponse } from "./media";

export type UserProfileResponse = {
  userId: string;
  displayName: string;
  profilePicture: MediaResponse | null;
  homeGym: string | null;
  lifterType: string | null;
  specialtyLifts: string | null;
  measurementsJson: string | null;
  followCount: number;
  followingCount: number;
};

export type PublicProfileResponse = {
  userId: string;
  displayName: string;
  profilePicture: MediaResponse | null;
  homeGym: string | null;
  lifterType: string | null;
  specialtyLifts: string | null;
  followCount: number;
  followingCount: number;
  isFollowedByCurrentUser: boolean;
};

export type UpdateProfileRequest = {
  displayName: string | null;
  profilePictureId: string | null;
  removeProfilePicture?: boolean;
  homeGym: string | null;
  lifterType: string | null;
  specialtyLifts: string | null;
  measurementsJson: string | null;
};
