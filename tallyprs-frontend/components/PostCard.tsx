import { PostResponse } from "@/types/post";
import Link from "next/link";

type PostCardProps = { post: PostResponse };

export default function PostCard({ post }: PostCardProps) {
  const created = new Date(post.createdAt).toLocaleString();
  const isUpvoted = post.myVoteValue === 1;
  const isDownvoted = post.myVoteValue === -1;

  return (
    <article className="w-full max-w-2xl mx-auto bg-white md:rounded-2xl md:shadow p-4 space-y-3">
      - {/* header */}
      <div className="flex justify-between items-center text-sm text-gray-500">
        <div>
          <span className="font-medium text-gray-800">{post.userName}</span>
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
        <div className="text-xs px-2 py-1 rounded bg-yellow-100 text-yellow-700 w-fit ">
          Pending...
        </div>
      )}
      {post.status == 2 && (
        <div className="text-xs px-2 py-1 rounded bg-red-200 text-red-700 w-fit">
          Rejected.
        </div>
      )}
      {/* Engagement */}
      <div className="flex items-center gap-4 pt-2 text-sm ">
        <button
          className={`flex items-center gap-1 ${
            isUpvoted ? "text-green-600 font-semibold" : "text-gray-500"
          } hover: cursor-pointer`}
        >
          ↑ {post.voteCount}
        </button>

        <button
          className={`${
            isDownvoted ? "text-red-600 font-semibold" : "text-gray-500"
          } hover:cursor-pointer`}
        >
          ↓
        </button>

        <div className="text-gray-500 hover:cursor-pointer">
          💬 {post.commentCount}
        </div>
      </div>
    </article>
  );
}
