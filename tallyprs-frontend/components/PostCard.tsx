import { PostResponse } from "@/types/post";

type PostCardProps = { post: PostResponse };

export default function PostCard({ post }: PostCardProps) {
  const created = new Date(post.createdAt).toLocaleString();
  const isUpvoted = post.myVoteValue === 1;
  const isDownvoted = post.myVoteValue === -1;

  return (
    <article className="bg-white md:rounded-2xl md:shadow p-4 space-y-3">
      {/* header */}
      <div className="flex justify-between items-center text-sm text-gray-500">
        <div>
          <span className="font-medium text-gray-800">{post.userName}</span>
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
      {post.status !== 1 && (
        <div className="text-xs px-2 py-1 rounded bg-yellow-100 text-yellow-700 w-fit">
          {post.status}
        </div>
      )}

      {/* Engagement */}
      <div className="flex items-center gap-4 pt-2 text-sm">
        <button
          className={`flex items-center gap-1 ${
            isUpvoted ? "text-green-600 font-semibold" : "text-gray-500"
          }`}
        >
          ↑ {post.voteCount}
        </button>

        <button
          className={`${
            isDownvoted ? "text-red-600 font-semibold" : "text-gray-500"
          }`}
        >
          ↓
        </button>

        <div className="text-gray-500">💬 {post.commentCount}</div>
      </div>
    </article>
  );
}
