import { apiFetch } from "@/services/apiClient";
import {
  ConversationResponse,
  ConversationListItemResponse,
  ConversationDetailsResponse,
} from "@/types/message";

export async function createConversation(
  otherUserId: string,
): Promise<ConversationResponse> {
  const response = await apiFetch("/conversations", {
    method: "POST",
    body: JSON.stringify({
      otherUserId,
    }),
  });
  const createdConversation: ConversationResponse = await response.json();

  return createdConversation;
}

export async function getConversationsForUser(): Promise<
  ConversationListItemResponse[]
> {
  const response = await apiFetch("/conversations", {
    method: "GET",
  });
  const conversations: ConversationListItemResponse[] = await response.json();

  return conversations;
}

export async function getDetailsForConversation(
  conversationId: string,
): Promise<ConversationDetailsResponse> {
  const response = await apiFetch(`/conversations/${conversationId}`, {
    method: "GET",
  });
  const details: ConversationDetailsResponse = await response.json();

  return details;
}
