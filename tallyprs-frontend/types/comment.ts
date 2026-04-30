export type CommentResponse = {
  id: string;
  postId: string;
  userId: string;
  userName: string;
  body: string;
  createdAt: string;
  parentCommentId?: string | null;
  replies: CommentResponse[];
};

export type commentRequest = {
  body: String;
};
