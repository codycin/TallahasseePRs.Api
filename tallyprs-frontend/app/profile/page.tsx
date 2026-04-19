"use client";

import { useEffect, useState } from "react";
import { BiUser, BiLoaderAlt } from "react-icons/bi";

import { getProfile } from "@/services/Profile/profile";
import { UserProfileResponse } from "@/types/profile";

import { useRouter } from "next/navigation";

export default function ProfilePage() {
  const router = useRouter();

  const [profile, setProfile] = useState<UserProfileResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    async function loadProfile() {
      try {
        setIsLoading(true);
        setErrorMessage("");

        const data = await getProfile();
        setProfile(data);
      } catch (error) {
        console.error(error);
        setErrorMessage(
          error instanceof Error ? error.message : "Failed to load profile.",
        );
      } finally {
        setIsLoading(false);
      }
    }

    loadProfile();
  }, []);

  if (isLoading) {
    return (
      <main className="min-h-screen bg-black text-white">
        <div className="mx-auto flex min-h-screen w-full max-w-2xl items-center justify-center bg-black md:my-8 md:min-h-[400px] md:rounded-3xl md:shadow-xl">
          <div className="flex items-center gap-3 text-sm text-gray-300">
            <BiLoaderAlt className="animate-spin" size={20} />
            Loading profile...
          </div>
        </div>
      </main>
    );
  }

  if (errorMessage) {
    return (
      <main className="min-h-screen bg-black text-white">
        <div className="mx-auto min-h-screen w-full max-w-2xl bg-black p-4 md:my-8 md:min-h-0 md:rounded-3xl md:shadow-xl md:p-6">
          <div className="rounded-2xl border border-red-500/40 bg-red-500/10 px-4 py-3 text-sm text-red-300">
            {errorMessage}
          </div>
        </div>
      </main>
    );
  }

  return (
    <main className="min-h-screen bg-black text-white">
      <div className="mx-auto min-h-screen w-full max-w-2xl bg-black md:my-8 md:min-h-0 md:rounded-3xl md:shadow-xl">
        <header className="sticky top-0 z-10 flex items-center justify-between border-b border-gray-800 bg-black px-4 py-4 md:rounded-t-3xl">
          <h1 className="text-lg font-semibold text-white">My Profile</h1>
          <button
            type="button"
            className="rounded-md bg-blue-600 px-3 py-1 text-sm font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
            onClick={() => router.push("/profile/edit")}
          >
            Edit
          </button>
        </header>

        <section className="space-y-6 p-4 md:p-6">
          <div className="flex flex-col items-center gap-4">
            <div className="flex h-28 w-28 items-center justify-center overflow-hidden rounded-full border border-gray-700 bg-zinc-900">
              {profile?.profilePicture?.url ? (
                <img
                  src={
                    profile?.profilePicture?.url
                      ? `${profile.profilePicture.url}?t=${Date.now()}`
                      : undefined
                  }
                  alt="Profile"
                  className="h-full w-full object-cover"
                />
              ) : (
                <BiUser size={40} className="text-gray-400" />
              )}
            </div>

            <div className="text-center">
              <h2 className="text-xl font-semibold text-white">
                {profile?.displayName || "No display name set"}
              </h2>
              <p className="mt-1 text-sm text-gray-400">{profile?.userId}</p>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4 text-sm text-gray-400">
            <div>Followers: {profile?.followCount} </div>
            <div>Following: {profile?.followingCount} </div>
          </div>
          <div className="grid gap-4">
            <div className="rounded-2xl border border-gray-800 bg-zinc-900/60 p-4">
              <p className="text-xs uppercase tracking-wide text-gray-400">
                Home Gym
              </p>
              <p className="mt-2 text-sm text-white">
                {profile?.homeGym || "Not set"}
              </p>
            </div>

            <div className="rounded-2xl border border-gray-800 bg-zinc-900/60 p-4">
              <p className="text-xs uppercase tracking-wide text-gray-400">
                Lifter Type
              </p>
              <p className="mt-2 text-sm text-white">
                {profile?.lifterType || "Not set"}
              </p>
            </div>

            <div className="rounded-2xl border border-gray-800 bg-zinc-900/60 p-4">
              <p className="text-xs uppercase tracking-wide text-gray-400">
                Specialty Lifts
              </p>
              <p className="mt-2 text-sm text-white">
                {profile?.specialtyLifts || "Not set"}
              </p>
            </div>

            <div className="rounded-2xl border border-gray-800 bg-zinc-900/60 p-4">
              <p className="text-xs uppercase tracking-wide text-gray-400">
                Measurements JSON
              </p>
              <pre className="mt-2 whitespace-pre-wrap break-words text-sm text-white">
                {profile?.measurementsJson || "Not set"}
              </pre>
            </div>
          </div>
        </section>
      </div>
    </main>
  );
}
