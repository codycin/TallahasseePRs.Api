"use client";

import { ConversationListItemResponse } from "@/types/message";
import { useRouter } from "next/navigation";
import { BiUser } from "react-icons/bi";

type ConversationCardProps = {
  conversation: ConversationListItemResponse;
};

export default function ConversationCard({
  conversation,
}: ConversationCardProps) {
  const router = useRouter();

  const displayName = conversation.otherUser.displayName || "Unknown User";

  const initials = displayName
    .split(" ")
    .map((part) => part[0])
    .join("")
    .slice(0, 2)
    .toUpperCase();

  const hasUnread = conversation.unreadCount > 0;

  const lastMessageText =
    conversation.lastMessageBody?.trim() || "No messages yet";

  const lastMessageTime = conversation.lastMessageAtUtc
    ? new Date(conversation.lastMessageAtUtc).toLocaleDateString([], {
        month: "short",
        day: "numeric",
      })
    : "";

  return (
    <button
      type="button"
      onClick={() => router.push(`/messages/${conversation.id}`)}
      className={[
        "w-full rounded-2xl border p-4 text-left shadow-lg shadow-black/20 transition",
        hasUnread
          ? "border-sky-800/70 bg-sky-950/30 hover:border-sky-700 hover:bg-sky-950/40"
          : "border-zinc-800 bg-zinc-950 hover:border-zinc-700 hover:bg-zinc-900",
      ].join(" ")}
    >
      <div className="flex items-center gap-3">
        <div className="h-12 w-12 shrink-0 overflow-hidden rounded-full border border-zinc-700 bg-zinc-900">
          {conversation.otherUser?.profilePictureUrl ? (
            <img
              src={conversation.otherUser.profilePictureUrl}
              alt={`${conversation.otherUser.displayName}'s profile picture`}
              className="h-full w-full object-cover"
            />
          ) : (
            <div className="flex h-full w-full items-center justify-center text-zinc-400">
              <BiUser className="text-2xl" />
            </div>
          )}
        </div>

        <div className="min-w-0 flex-1">
          <div className="flex items-center justify-between gap-3">
            <p
              className={[
                "truncate text-sm",
                hasUnread
                  ? "font-semibold text-white"
                  : "font-medium text-zinc-100",
              ].join(" ")}
            >
              {displayName}
            </p>

            {lastMessageTime && (
              <span className="shrink-0 text-xs text-zinc-500">
                {lastMessageTime}
              </span>
            )}
          </div>

          <div className="mt-1 flex items-center justify-between gap-3">
            <p
              className={[
                "truncate text-sm",
                hasUnread ? "font-medium text-zinc-200" : "text-zinc-500",
              ].join(" ")}
            >
              {lastMessageText}
            </p>

            {hasUnread && (
              <span className="flex h-5 min-w-5 shrink-0 items-center justify-center rounded-full bg-sky-500 px-1.5 text-xs font-bold text-white">
                {conversation.unreadCount > 9 ? "9+" : conversation.unreadCount}
              </span>
            )}
          </div>
        </div>
      </div>
    </button>
  );
}
