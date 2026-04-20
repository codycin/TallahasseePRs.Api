"use client";

import { removeVotePost, votePost } from "@/services/Post/posts";
import { PostResponse } from "@/types/post";
import { useEffect, useState } from "react";
import Link from "next/link";

type PostCardProps = { post: PostResponse };

type CommentLoadState = "idle" | "loading" | "loaded" | "error";

export default function PostCard({ post }: PostCardProps) {
  const created = new Date(post.createdAt).toLocaleString();

  const [voteCount, setVoteCount] = useState(post.voteCount);
  const [voteValue, setVoteValue] = useState<number | null>(
    post.myVoteValue ?? null,
  );

  const [commentsOpen, setCommentsOpen] = useState(false);
  const [commentLoadState, setCommentLoadState] =
    useState<CommentLoadState>("idle");
  const [comments, setComments] = useState<any[]>([]);

  useEffect(() => {
    setVoteValue(post.myVoteValue ?? null);
  }, [post.myVoteValue]);

  async function toggleComments() {
    const nextOpen = !commentsOpen;
    setCommentsOpen(nextOpen);

    if (nextOpen && commentLoadState === "idle") {
      setCommentLoadState("loading");

      try {
        // NEXT TODO:
        // const data = await getCommentsByPostId(post.id);
        // setComments(data);

        // temporary placeholder
        await new Promise((resolve) => setTimeout(resolve, 400));
        setComments([]);
        setCommentLoadState("loaded");
      } catch (error) {
        console.error("Failed to load comments", error);
        setCommentLoadState("error");
      }
    }
  }

  return (
    <article className="w-full max-w-2xl mx-auto bg-white md:rounded-2xl md:shadow p-4 space-y-3">
      {/* header */}
      <div className="flex justify-between items-center text-sm text-gray-500">
        <div>
          <span className="font-medium text-gray-800">{post.userName}</span>{" "}
          <Link href={`/profile/${post.userId}`}>View profile</Link>
        </div>
        <span>{created}</span>
      </div>

      {/* Title + Lift */}
      <div>
        <h2 className="text-lg font-semibold text-gray-900">{post.title}</h2>
        <p className="text-sm text-gray-600">
          {post.weight} {post.unit}
        </p>
      </div>

      {/* Media */}
      {post.media.length > 0 && (
        <div className="-mx-4 md:mx-0">
          {post.media.map((m) => (
            <img
              key={m.id}
              src={m.url}
              alt="post media"
              className="w-full object-cover md:rounded-xl"
            />
          ))}
        </div>
      )}

      {/* Description */}
      {post.description && (
        <p className="text-gray-700 text-sm">{post.description}</p>
      )}

      {/* Status Badge */}
      {post.status === 0 && (
        <div className="text-xs px-2 py-1 rounded bg-yellow-100 text-yellow-700 w-fit">
          Pending...
        </div>
      )}
      {post.status == 2 && (
        <div className="text-xs px-2 py-1 rounded bg-red-200 text-red-700 w-fit">
          Rejected.
        </div>
      )}

      {/* Engagement */}
      <div className="flex items-center gap-4 pt-2 text-sm">
        <button
          className={`flex items-center gap-1 ${
            voteValue === 1 ? "text-green-600 font-semibold" : "text-gray-500"
          } hover:cursor-pointer`}
          onClick={() => {
            if (voteValue === -1 || voteValue === 0 || voteValue === null) {
              votePost(post.id, 1);
              setVoteValue(1);
              setVoteCount(voteCount + 1);
              console.log("UP from DOWN/NULL clicked", {
                voteValue,
                myVoteValue: post.myVoteValue,
              });
            }
            if (voteValue === 1) {
              removeVotePost(post.id);
              setVoteValue(null);
              setVoteCount(voteCount - 1);
              console.log("UP FROM UP clicked", {
                voteValue,
                myVoteValue: post.myVoteValue,
              });
            }
          }}
        >
          ↑ {voteCount}
        </button>

        <button
          className={`${
            voteValue === -1 ? "text-red-600 font-semibold" : "text-gray-500"
          } hover:cursor-pointer`}
          onClick={() => {
            if (voteValue === 1 || voteValue === 0 || voteValue === null) {
              votePost(post.id, -1);
              setVoteValue(-1);
              if (voteValue === 1) {
                setVoteCount(voteCount - 1);
              }
            }
            if (voteValue === -1) {
              removeVotePost(post.id);
              setVoteValue(null);
            }
            console.log("clicked", {
              voteValue,
              myVoteValue: post.myVoteValue,
            });
          }}
        >
          ↓
        </button>

        <button
          type="button"
          onClick={toggleComments}
          className="text-gray-500 hover:cursor-pointer"
        >
          💬 {post.commentCount}
        </button>
      </div>

      {/* Comments Section */}
      {commentsOpen && (
        <div className="pt-2 border-t border-gray-200 space-y-3">
          <div className="flex items-center justify-between">
            <h3 className="text-sm font-semibold text-gray-800">Comments</h3>
            <button
              type="button"
              className="text-xs text-blue-600 hover:underline"
            >
              Add comment
            </button>
          </div>

          {commentLoadState === "loading" && (
            <div className="space-y-2">
              <div className="animate-pulse rounded-lg bg-gray-100 p-3">
                <div className="h-3 w-24 bg-gray-200 rounded mb-2" />
                <div className="h-3 w-full bg-gray-200 rounded mb-1" />
                <div className="h-3 w-2/3 bg-gray-200 rounded" />
              </div>
              <div className="animate-pulse rounded-lg bg-gray-100 p-3">
                <div className="h-3 w-20 bg-gray-200 rounded mb-2" />
                <div className="h-3 w-full bg-gray-200 rounded mb-1" />
                <div className="h-3 w-1/2 bg-gray-200 rounded" />
              </div>
            </div>
          )}

          {commentLoadState === "error" && (
            <div className="text-sm text-red-600">Failed to load comments.</div>
          )}

          {commentLoadState === "loaded" && comments.length === 0 && (
            <div className="text-sm text-gray-500">No comments yet.</div>
          )}

          {commentLoadState === "loaded" && comments.length > 0 && (
            <div className="space-y-3">
              {comments.map((comment) => (
                <div
                  key={comment.id}
                  className="rounded-lg bg-gray-50 p-3 border border-gray-200"
                >
                  <div className="text-xs text-gray-500 mb-1">
                    {comment.userName}
                  </div>
                  <p className="text-sm text-gray-700">{comment.content}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </article>
  );
}
