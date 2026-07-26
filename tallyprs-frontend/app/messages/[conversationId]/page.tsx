"use client";

import { useEffect, useRef, useState } from "react";
import { HubConnection, HubConnectionState } from "@microsoft/signalr";
import {
  createMessageConnection,
  startMessageConnection,
} from "@/services/Messages/messageHub";
import { useParams } from "next/navigation";
import { MessageResponse, ConversationUserResponse } from "@/types/message";
import { getDetailsForConversation } from "@/services/Messages/messageService";
import Link from "next/link";
import { BiArrowBack, BiSend, BiUser } from "react-icons/bi";

export default function MessagePage() {
  const params = useParams();
  const conversationId = params?.conversationId as string | undefined;

  const connectionRef = useRef<HubConnection | null>(null);
  const scrollRef = useRef<HTMLDivElement | null>(null);
  const textareaRef = useRef<HTMLTextAreaElement | null>(null);

  const [messages, setMessages] = useState<MessageResponse[]>([]);
  const [newMessages, setNewMessages] = useState<MessageResponse[]>([]);

  const [body, setBody] = useState("");
  const [isConnected, setIsConnected] = useState(false);
  const [otherUser, setOtherUser] = useState<ConversationUserResponse>();
  const [currentUserId, setCurrentUserId] = useState<string | null>(null);
  const [keyboardInset, setKeyboardInset] = useState(0);
  const [errorMessage, setErrorMessage] = useState("");

  const profileHref = otherUser?.userId ? `/profile/${otherUser.userId}` : "#";

  useEffect(() => {
    setCurrentUserId(window.localStorage.getItem("currentUserId"));

    document.body.classList.add("hide-mobile-nav");

    return () => {
      document.body.classList.remove("hide-mobile-nav");
    };
  }, []);

  useEffect(() => {
    const viewport = window.visualViewport;

    function updateKeyboardInset() {
      if (!viewport) {
        setKeyboardInset(0);
        return;
      }

      const inset = Math.max(
        0,
        window.innerHeight - viewport.height - viewport.offsetTop,
      );

      setKeyboardInset(inset);
    }

    updateKeyboardInset();

    viewport?.addEventListener("resize", updateKeyboardInset);
    viewport?.addEventListener("scroll", updateKeyboardInset);
    window.addEventListener("orientationchange", updateKeyboardInset);

    return () => {
      viewport?.removeEventListener("resize", updateKeyboardInset);
      viewport?.removeEventListener("scroll", updateKeyboardInset);
      window.removeEventListener("orientationchange", updateKeyboardInset);
    };
  }, []);

  useEffect(() => {
    const container = scrollRef.current;
    if (!container) return;

    requestAnimationFrame(() => {
      container.scrollTop = container.scrollHeight;
    });
  }, [messages.length, keyboardInset]);

  useEffect(() => {
    if (!conversationId) return;

    let cancelled = false;

    const connection = createMessageConnection();
    connectionRef.current = connection;

    const receiveMessageHandler = (message: MessageResponse) => {
      setMessages((prev) => {
        const alreadyExists = prev.some((x) => x.id === message.id);
        if (alreadyExists) return prev;

        return [...prev, message];
      });
    };

    async function loadConversationDetails() {
      if (!conversationId) return;
      try {
        const details = await getDetailsForConversation(conversationId);

        if (cancelled) return;

        setMessages(details.messages);
        setOtherUser(details.otherUser);
      } catch (error) {
        if (!cancelled) {
          console.error("Failed to load conversation:", error);
        }
      }
    }

    async function connect() {
      try {
        connection.on("ReceiveMessage", receiveMessageHandler);

        await startMessageConnection(connection);

        if (cancelled) {
          if (connection.state === HubConnectionState.Connected) {
            await connection.stop();
          }

          return;
        }

        setIsConnected(true);
      } catch (err) {
        if (!cancelled) {
          console.error("SignalR connection failed:", err);
          setIsConnected(false);
        }
      }
    }

    loadConversationDetails();
    void connect();

    return () => {
      cancelled = true;
      setIsConnected(false);

      connection.off("ReceiveMessage", receiveMessageHandler);

      if (connectionRef.current === connection) {
        connectionRef.current = null;
      }

      if (connection.state === HubConnectionState.Connected) {
        void connection.stop();
      }
    };
  }, [conversationId]);

  async function sendMessage() {
    const trimmedBody = body.trim();

    if (!trimmedBody) return;

    if (!conversationId) {
      console.error("Missing conversationId.");
      return;
    }

    const connection = connectionRef.current;

    if (!connection || connection.state !== HubConnectionState.Connected) {
      setErrorMessage("Message could not send because you are disconnected.");
      return;
    }

    try {
      setErrorMessage("");

      await connection.invoke("SendMessage", conversationId, trimmedBody);

      setBody("");

      requestAnimationFrame(() => {
        if (!textareaRef.current) return;
        textareaRef.current.style.height = "44px";
      });
    } catch (error) {
      console.error("Failed to send message:", error);
      setErrorMessage("Failed to send message.");
    }
  }

  function handleTextareaChange(value: string) {
    setBody(value);

    requestAnimationFrame(() => {
      const textarea = textareaRef.current;
      if (!textarea) return;

      textarea.style.height = "44px";
      textarea.style.height = `${Math.min(textarea.scrollHeight, 132)}px`;
    });
  }

  function formatMessageTime(value: string) {
    return new Date(value).toLocaleTimeString([], {
      hour: "numeric",
      minute: "2-digit",
    });
  }

  return (
    <main className="h-dvh overflow-hidden bg-black text-white">
      <section className="mx-auto flex h-dvh w-full max-w-2xl flex-col overflow-hidden border-white/10 bg-black md:border-x">
        <header className="z-30 border-b border-white/10 bg-black/85 px-3 pb-3 pt-[calc(0.75rem+env(safe-area-inset-top))] backdrop-blur-xl">
          <div className="flex items-center gap-3">
            <Link
              href="/messages"
              className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-zinc-900 text-zinc-200 ring-1 ring-white/10 transition hover:bg-zinc-800"
              aria-label="Back to messages"
            >
              <BiArrowBack className="text-2xl" />
            </Link>

            <Link
              href={profileHref}
              className="relative h-11 w-11 shrink-0 overflow-hidden rounded-full bg-zinc-900 ring-1 ring-white/10 transition hover:ring-white/25"
            >
              {otherUser?.profilePictureUrl ? (
                <img
                  src={otherUser.profilePictureUrl}
                  alt={`${otherUser.displayName}'s profile picture`}
                  className="h-full w-full object-cover"
                />
              ) : (
                <div className="flex h-full w-full items-center justify-center text-zinc-400">
                  <BiUser className="text-2xl" />
                </div>
              )}
            </Link>

            <div className="min-w-0 flex-1">
              <Link
                href={profileHref}
                className="block truncate text-[15px] font-semibold text-zinc-50 hover:underline"
              >
                {otherUser?.displayName ?? "Messages"}
              </Link>
            </div>
          </div>
        </header>

        {errorMessage && (
          <div className="border-b border-yellow-500/20 bg-yellow-500/10 px-4 py-2 text-sm text-yellow-200">
            {errorMessage}
          </div>
        )}

        <div
          ref={scrollRef}
          className="flex-1 overflow-y-auto px-3 pt-4 [scrollbar-width:none]"
          style={{
            paddingBottom: `calc(6.5rem + env(safe-area-inset-bottom) + ${keyboardInset}px)`,
          }}
        >
          {messages.length === 0 ? (
            <div className="flex h-full items-center justify-center px-8 text-center">
              <div>
                <div className="mx-auto mb-3 flex h-14 w-14 items-center justify-center rounded-full bg-zinc-900 ring-1 ring-white/10">
                  <BiUser className="text-3xl text-zinc-500" />
                </div>
                <p className="text-sm font-medium text-zinc-200">
                  Start the conversation
                </p>
                <p className="mt-1 text-sm text-zinc-500">
                  Send a message to begin chatting.
                </p>
              </div>
            </div>
          ) : (
            <div className="space-y-2">
              {messages.map((message, index) => {
                const isMine = currentUserId === message.senderId;
                const previousMessage = messages[index - 1];
                const showOtherAvatar =
                  !isMine && previousMessage?.senderId !== message.senderId;

                return (
                  <div
                    key={message.id}
                    className={`flex ${
                      isMine ? "justify-end" : "justify-start"
                    }`}
                  >
                    <div
                      className={`flex max-w-[82%] items-end gap-2 ${
                        isMine ? "flex-row-reverse" : ""
                      }`}
                    >
                      {!isMine &&
                        (showOtherAvatar ? (
                          <Link
                            href={profileHref}
                            className="mb-5 h-8 w-8 shrink-0 overflow-hidden rounded-full bg-zinc-900 ring-1 ring-white/10"
                          >
                            {otherUser?.profilePictureUrl ? (
                              <img
                                src={otherUser.profilePictureUrl}
                                alt={`${otherUser.displayName}'s profile picture`}
                                className="h-full w-full object-cover"
                              />
                            ) : (
                              <div className="flex h-full w-full items-center justify-center text-zinc-500">
                                <BiUser className="text-lg" />
                              </div>
                            )}
                          </Link>
                        ) : (
                          <div className="w-8 shrink-0" />
                        ))}

                      <div>
                        <div
                          className={`rounded-[1.35rem] px-4 py-2.5 shadow-sm ${
                            isMine
                              ? "rounded-br-md bg-blue-600 text-white"
                              : "rounded-bl-md bg-zinc-900 text-zinc-100 ring-1 ring-white/10"
                          }`}
                        >
                          <p className="whitespace-pre-wrap wrap-break-word text-[15px] leading-relaxed">
                            {message.body}
                          </p>
                        </div>

                        <p
                          className={`mt-1 px-1 text-[11px] text-zinc-600 ${
                            isMine ? "text-right" : "text-left"
                          }`}
                        >
                          {formatMessageTime(message.sentAtUtc)}
                        </p>
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>

        <form
          onSubmit={(e) => {
            e.preventDefault();
            void sendMessage();
          }}
          className="fixed inset-x-0 bottom-0 z-50 mx-auto border-t border-white/10 bg-black/90 px-3 pb-[calc(0.75rem+env(safe-area-inset-bottom))] 
          pt-3 backdrop-blur-xl transition-transform duration-150  w-full max-w-lg "
          style={{
            transform: `translateY(-${keyboardInset}px)`,
          }}
        >
          <div className="flex items-end gap-2 rounded-[1.75rem] bg-zinc-900 p-1.5 ring-1 ring-white/10">
            <textarea
              ref={textareaRef}
              value={body}
              onChange={(e) => handleTextareaChange(e.target.value)}
              onKeyDown={(e) => {
                if (
                  e.key === "Enter" &&
                  !e.shiftKey &&
                  !e.nativeEvent.isComposing
                ) {
                  e.preventDefault();
                  void sendMessage();
                }
              }}
              rows={1}
              placeholder="Message..."
              className="max-h-32 min-h-11 flex-1 resize-none bg-transparent px-3 py-2.5 text-base leading-6 text-white outline-none placeholder:text-zinc-500"
            />

            <button
              type="submit"
              disabled={!body.trim() || !isConnected}
              className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-blue-600 text-white transition hover:bg-blue-500 disabled:cursor-not-allowed disabled:bg-zinc-700 disabled:text-zinc-400"
              aria-label="Send message"
            >
              <BiSend className="text-2xl" />
            </button>
          </div>
        </form>
      </section>
    </main>
  );
}
