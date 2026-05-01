"use client";

import Feed from "@/components/Feed";
import PostCard from "@/components/PostCard";
import { getUserPostFeed } from "@/services/Feed/feedService";
import type { PostResponse } from "@/types/post";

type UserPostsFeedClientProps = {
  userId: string;
};

export default function UserPostsFeedClient({
  userId,
}: UserPostsFeedClientProps) {
  return (
    <Feed<PostResponse>
      fetchPage={(cursor) => getUserPostFeed(userId, 20, cursor)}
      getKey={(post) => post.id}
      renderItem={(post, { removeItem }) => (
        <PostCard post={post} onDeleted={removeItem} />
      )}
    />
  );
}
