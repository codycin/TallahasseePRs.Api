export type FeedPage<T> = {
  items: T[];
  nextCursor: string | null;
};
