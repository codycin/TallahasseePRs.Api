"use client";

import ConversationCard from "@/components/ConversationCard";
import { getConversationsForUser } from "@/services/Messages/messageService";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { ApiError } from "@/utils/apiError";
import { ConversationListItemResponse } from "@/types/message";
import { BiPlusCircle } from "react-icons/bi";
import NewConversationWindow from "@/components/NewMessageComponent";

export default function MessageCenter() {
  const router = useRouter();
  const [conversations, setConversations] = useState<
    ConversationListItemResponse[]
  >([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [showNewMessage, setShowNewMessage] = useState(false);

  useEffect(() => {
    LoadConversationsAsync();
  }, []);

  async function LoadConversationsAsync() {
    try {
      setLoading(true);
      setError("");

      const data = await getConversationsForUser();
      setConversations(data);
    } catch (error) {
      if (error instanceof ApiError && error.status === 401) {
        router.push("/login");
        return;
      }
      setError(
        error instanceof Error ? error.message : "Failed to load conversations",
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <>
      {showNewMessage && (
        <NewConversationWindow onClose={() => setShowNewMessage(false)} />
      )}
      <main className="min-h-screen bg-black px-4 py-8 text-zinc-50">
        <section className="mx-auto max-w-3xl">
          <div className="mb-6 items-center justify-between gap-4">
            <div className="flex items-center mb-6 justify-between">
              <h1 className="text-3xl font-bold  tracking-tight text-zinc-50">
                Messages
              </h1>
              <BiPlusCircle
                onClick={() => setShowNewMessage(true)}
                className="mr-6"
                size={25}
              />
            </div>
            {loading && (
              <div className="rounded-2xl border border-zinc-800 bg-zinc-950 p-6 text-sm text-zinc-500 shadow-lg shadow-black/20">
                Loading messages...
              </div>
            )}

            {error && (
              <div className="mb-4 rounded-2xl border border-rose-900/70 bg-rose-950/40 p-4 text-sm text-rose-300">
                {error}
              </div>
            )}

            {!loading && conversations.length === 0 && (
              <div className="rounded-2xl border border-zinc-800 bg-zinc-950 p-8 text-center shadow-lg shadow-black/20">
                <h2 className="text-lg font-semibold text-zinc-100">
                  No messages yet
                </h2>

                <p className="mt-2 text-sm text-zinc-500">
                  Your conversations will show here.
                </p>
              </div>
            )}

            {!loading && conversations.length > 0 && (
              <div className="space-y-3">
                {conversations.map((conversation) => (
                  <ConversationCard
                    key={conversation.id}
                    conversation={conversation}
                  />
                ))}
              </div>
            )}
          </div>
        </section>
      </main>
    </>
  );
}
