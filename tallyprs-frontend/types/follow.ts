export type followResponse = {
  Id: string;
  FollowerId: string;
  FollowedId: string;
  FollowedAt: Date;
};

export type followRequest = {
  FollowedId: string;
};
