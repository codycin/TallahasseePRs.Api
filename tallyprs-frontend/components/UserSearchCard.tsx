import { UserSearchResult } from "@/types/userSearch";
import Link from "next/link";

type UserSearchCardProps = {
  user: UserSearchResult;
  onClick: (userId: string) => void;
  onClickTitle: string;
};

export default function UserSearchCard({
  user,
  onClick,
  onClickTitle,
}: UserSearchCardProps) {
  const displayName = user.displayName || user.userName;

  return (
    <div className="flex items-center gap-4 rounded-2xl border border-zinc-800 bg-zinc-950 p-4 shadow-lg shadow-black/20 transition hover:border-zinc-700 hover:bg-zinc-900/80">
      <Link href={`/profile/${user.userId}`} className="shrink-0">
        {user.profilePictureUrl ? (
          <img
            src={user.profilePictureUrl}
            alt={`${displayName}'s profile picture`}
            className="h-14 w-14 rounded-full object-cover ring-1 ring-zinc-700"
          />
        ) : (
          <div className="flex h-14 w-14 items-center justify-center rounded-full bg-zinc-900 text-lg font-semibold text-zinc-300 ring-1 ring-zinc-700">
            {displayName.charAt(0).toUpperCase()}
          </div>
        )}
      </Link>

      <div className="min-w-0 flex-1">
        <Link
          href={`/profiles/${user.userId}`}
          className="block truncate font-semibold text-zinc-100 hover:text-white hover:underline"
        >
          {displayName}
        </Link>

        <p className="truncate text-sm text-zinc-500">@{user.userName}</p>
      </div>

      <button
        onClick={() => onClick(user.userId)}
        className="rounded-full border border-zinc-700 px-4 py-2 text-sm font-medium text-zinc-300 transition hover:border-zinc-500 hover:bg-zinc-800 hover:text-white"
      >
        {onClickTitle}
      </button>
    </div>
  );
}
