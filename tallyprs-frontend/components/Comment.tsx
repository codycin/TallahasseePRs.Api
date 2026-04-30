import { CommentResponse } from "@/types/comment";
import { useState } from "react";
import { deleteComment } from "@/services/Comment/commentService";
export default function CommentItem({
  comment,
  currentUserId,
  replyingToCommentId,
  setReplyingToCommentId,
  replyBody,
  setReplyBody,
  submitCommentReply,
  commentSubmitting,
  onDeleteComment,
}: {
  comment: CommentResponse;
  currentUserId: string | null;
  replyingToCommentId: string | null;
  setReplyingToCommentId: React.Dispatch<React.SetStateAction<string | null>>;
  replyBody: string;
  setReplyBody: React.Dispatch<React.SetStateAction<string>>;
  submitCommentReply: (commentId: string) => void;
  commentSubmitting: boolean;
  onDeleteComment: (commentId: string) => void;
}) {
  const canDelete = currentUserId === comment.userId;
  console.log(currentUserId);
  return (
    <div className="rounded-lg bg-gray-50 p-3 border border-gray-200">
      <div className="flex justify-between">
        <div className="text-xs text-gray-500 mb-1">{comment.userName}</div>

        {canDelete && (
          <button
            type="button"
            className="text-xs text-red-500"
            onClick={() => onDeleteComment(comment.id)}
          >
            delete
          </button>
        )}
      </div>
      <p className="text-sm text-gray-700">{comment.body}</p>

      <button
        className="text-xs text-gray-400"
        onClick={() => setReplyingToCommentId(comment.id)}
      >
        reply
      </button>

      {replyingToCommentId === comment.id && (
        <div className="mt-2 rounded-lg border border-gray-200 bg-white p-3 space-y-2">
          <textarea
            value={replyBody}
            onChange={(e) => setReplyBody(e.target.value)}
            placeholder="Write a reply..."
            className="w-full min-h-20 rounded-md border border-gray-300 bg-white p-2 text-sm text-gray-900 outline-none"
          />

          <div className="flex justify-end gap-2">
            <button
              type="button"
              onClick={() => {
                setReplyingToCommentId(null);
                setReplyBody("");
              }}
              className="px-3 py-1 text-sm text-gray-600"
            >
              Cancel
            </button>

            <button
              type="button"
              onClick={() => submitCommentReply(comment.id)}
              disabled={commentSubmitting || replyBody.trim().length === 0}
              className="bg-blue-600 text-white px-3 py-1 text-sm rounded disabled:opacity-50"
            >
              {commentSubmitting ? "Posting..." : "Post"}
            </button>
          </div>
        </div>
      )}

      {comment.replies?.length > 0 && (
        <div className="mt-3 ml-4 space-y-2 border-l border-gray-200 pl-3">
          {comment.replies.map((reply) => (
            <CommentItem
              key={reply.id}
              comment={reply}
              currentUserId={currentUserId}
              replyingToCommentId={replyingToCommentId}
              setReplyingToCommentId={setReplyingToCommentId}
              replyBody={replyBody}
              setReplyBody={setReplyBody}
              submitCommentReply={submitCommentReply}
              commentSubmitting={commentSubmitting}
              onDeleteComment={onDeleteComment}
            />
          ))}
        </div>
      )}
    </div>
  );
}
