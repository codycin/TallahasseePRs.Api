"use client";
import { FeedPage } from "@/types/feed";
import { useState, useEffect } from "react";

type FeedProps<T> = {
  fetchPage: (cursor?: string) => Promise<FeedPage<T>>;
  renderItem: (item: T) => React.ReactNode;
  getKey: (item: T) => string;
};

export default function Feed<T>({
  fetchPage,
  renderItem,
  getKey,
}: FeedProps<T>) {
  const [items, setItems] = useState<T[]>([]);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const hasMore = nextCursor !== null;

  useEffect(() => {
    async function loadInitial() {
      console.log("[Feed] loadInitial started");
      try {
        setLoading(true);
        setError(null);

        const page = await fetchPage();

        console.log("[Feed] page returned:", page);

        setItems(page.items);
        setNextCursor(page.nextCursor ?? null);
      } catch (err) {
        console.error("[Feed] loadInitial failed:", err);
        setError(err instanceof Error ? err.message : "Failed to load feed.");
      } finally {
        setLoading(false);
        console.log("[Feed] loadInitial finished");
      }
    }

    loadInitial();
  }, [fetchPage]);

  async function loadMore() {
    console.log("[Feed] loadMore started with cursor:", nextCursor);

    if (!hasMore || loading) return;

    try {
      setLoading(true);
      setError(null);

      const page = await fetchPage(nextCursor ?? undefined);

      console.log("[Feed] loadMore page returned:", page);

      setItems((prev) => [...prev, ...page.items]);
      setNextCursor(page.nextCursor ?? null);
    } catch (err) {
      console.error("[Feed] loadMore failed:", err);
      setError(
        err instanceof Error ? err.message : "Failed to load more posts.",
      );
    } finally {
      setLoading(false);
      console.log("[Feed] loadMore finished");
    }
  }

  return (
    <div>
      {items.map((item) => (
        <div key={getKey(item)}>{renderItem(item)}</div>
      ))}
      {loading && <p>Loading...</p>}
      {hasMore && !loading && <button onClick={loadMore}>Load More</button>}
      {error && <p>{error}</p>}
    </div>
  );
}
