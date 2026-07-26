"use client";

import { useEffect, useState } from "react";
import { searchUsers } from "@/services/UserSearch/userSearchService";
import { UserSearchResult } from "@/types/userSearch";
import { ApiError } from "@/utils/apiError";
import { useRouter } from "next/navigation";
import UserSearchCard from "@/components/UserSearchCard";
export default function SearchPage() {
  const [query, setQuery] = useState("");
  const [users, setUsers] = useState<UserSearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const router = useRouter();
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
  }, [query]);
  function handleViewClick(userId: string) {
    router.push(`profile/${userId}`);
  }

  return (
    <main className="min-h-screen bg-black px-4 py-8 text-zinc-50">
      <div className="mx-auto max-w-2xl">
        <h1 className="mb-2 text-3xl font-bold tracking-tight text-zinc-50">
          Search Users
        </h1>

        <p className="mb-6 text-sm text-zinc-500">
          Find lifters by username or display name.
        </p>

        <div className="rounded-2xl border border-zinc-800 bg-zinc-950 p-4 shadow-lg shadow-black/20">
          <input
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Search users..."
            className="w-full rounded-xl border border-zinc-800 bg-zinc-900 px-4 py-3 text-base sm:text-sm text-zinc-100 outline-none transition placeholder:text-zinc-500 focus:border-sky-700 focus:bg-zinc-950 focus:ring-4 focus:ring-sky-950"
          />
        </div>

        {loading && <p className="mt-4 text-sm text-zinc-500">Searching...</p>}

        {error && (
          <p className="mt-4 rounded-xl border border-rose-900/70 bg-rose-950/40 px-4 py-3 text-sm text-rose-300">
            {error}
          </p>
        )}

        <section className="mt-6 space-y-4">
          {users.map((user) => (
            <UserSearchCard
              key={user.userId}
              user={user}
              onClickTitle="View"
              onClick={handleViewClick}
            />
          ))}
        </section>

        {!loading &&
          query.trim().length >= 2 &&
          users.length === 0 &&
          !error && (
            <p className="mt-6 rounded-xl border border-zinc-800 bg-zinc-950 px-4 py-4 text-center text-sm text-zinc-500">
              No users found.
            </p>
          )}
      </div>
    </main>
  );
}
