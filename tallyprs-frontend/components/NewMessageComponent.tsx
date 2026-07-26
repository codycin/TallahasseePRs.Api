"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { createConversation } from "@/services/Messages/messageService";
import { searchUsers } from "@/services/UserSearch/userSearchService";
import { UserSearchResult } from "@/types/userSearch";
import { ApiError } from "@/utils/apiError";
import UserSearchCard from "@/components/UserSearchCard";

type NewConversationWindowProps = {
  onClose: () => void;
};

export default function NewConversationWindow({
  onClose,
}: NewConversationWindowProps) {
  const [isCreating, setIsCreating] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const [query, setQuery] = useState("");
  const [users, setUsers] = useState<UserSearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const router = useRouter();

  function handleMessageClicked(userId: string) {
    handleCreateConversation(userId);
  }

  useEffect(() => {
    const trimmedQuery = query.trim();

    if (trimmedQuery.length < 2) {
      setUsers([]);
      setError("");
      setLoading(false);
      return;
    }

    const timeoutId = window.setTimeout(async () => {
      try {
        setLoading(true);
        setError("");

        const results = await searchUsers(trimmedQuery);
        setUsers(results);
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          router.push("/login");
          return;
        }

        setError(err instanceof Error ? err.message : "Failed to search users");
      } finally {
        setLoading(false);
      }
    }, 300);

    return () => window.clearTimeout(timeoutId);
  }, [query, router]);

  async function handleCreateConversation(otherUserId: string) {
    try {
      setIsCreating(true);
      setErrorMessage("");

      const conversation = await createConversation(otherUserId);

      onClose();
      router.push(`/messages/${conversation.id}`);
    } catch (error) {
      console.error(error);
      setErrorMessage("Could not create conversation.");
    } finally {
      setIsCreating(false);
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 px-4 backdrop-blur-sm">
      {/* Click outside to close */}
      <button
        type="button"
        aria-label="Close new message window"
        onClick={onClose}
        className="absolute inset-0 cursor-default"
      />

      {/* Modal card */}
      <section className="relative z-10 w-full max-w-md overflow-hidden rounded-3xl border border-zinc-800 bg-black text-white shadow-2xl shadow-black/50">
        <header className="flex items-center justify-between border-b border-zinc-800 px-4 py-4">
          <h1 className="text-lg font-semibold">New Message</h1>

          <button
            type="button"
            onClick={onClose}
            className="rounded-full px-3 py-1 text-sm text-zinc-400 transition hover:bg-zinc-900 hover:text-white"
          >
            Cancel
          </button>
        </header>

        <div className="max-h-[75vh] space-y-4 overflow-y-auto px-4 py-5">
          <div>
            <label className="mb-2 block text-sm font-medium text-zinc-300">
              Username
            </label>

            <div className="rounded-2xl border border-zinc-800 bg-zinc-950 p-4 shadow-lg shadow-black/20">
              <input
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Search users..."
                autoFocus
                className="w-full rounded-xl border border-zinc-800 bg-zinc-900 px-4 py-3 text-base text-zinc-100 outline-none transition placeholder:text-zinc-500 focus:border-sky-700 focus:bg-zinc-950 focus:ring-4 focus:ring-sky-950 sm:text-sm"
              />
            </div>
          </div>

          {loading && <p className="text-sm text-zinc-500">Searching...</p>}

          {isCreating && (
            <p className="text-sm text-zinc-500">Creating conversation...</p>
          )}

          {error && (
            <p className="rounded-xl border border-rose-900/70 bg-rose-950/40 px-4 py-3 text-sm text-rose-300">
              {error}
            </p>
          )}

          {errorMessage && (
            <p className="rounded-xl border border-rose-900/70 bg-rose-950/40 px-4 py-3 text-sm text-rose-300">
              {errorMessage}
            </p>
          )}

          <section className="space-y-3">
            {users.map((user) => (
              <UserSearchCard
                key={user.userId}
                user={user}
                onClickTitle={isCreating ? "Creating..." : "Message"}
                onClick={handleMessageClicked}
              />
            ))}
          </section>

          {!loading &&
            query.trim().length >= 2 &&
            users.length === 0 &&
            !error && (
              <p className="rounded-xl border border-zinc-800 bg-zinc-950 px-4 py-4 text-center text-sm text-zinc-500">
                No users found.
              </p>
            )}
        </div>
      </section>
    </div>
  );
}
