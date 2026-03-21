"use client";

import Feed from "@/components/Feed";
import PostCard from "@/components/PostCard";
import { getPostFeed } from "@/services/Feed/feedService";
import type { PostResponse } from "@/types/post";

export default function HomeFeedClient() {
  return (
    <Feed<PostResponse>
      fetchPage={(cursor) => getPostFeed("Global", 20, cursor)}
      getKey={(post) => post.id}
      renderItem={(post) => <PostCard post={post} />}
    />
  );
}
