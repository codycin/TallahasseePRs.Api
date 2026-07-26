export type ConversationResponse = {
  id: string;
  createdAtUtc: string;
};

export type MessageResponse = {
  id: string;
  conversationId: string;
  senderId: string;
  body: string;
  sentAtUtc: string;
};

export type ConversationListItemResponse = {
  id: string;
  otherUser: ConversationUserResponse;
  lastMessageBody: string;
  lastMessageAtUtc: string;
  unreadCount: number;
};

export type ConversationDetailsResponse = {
  id: string;
  otherUser: ConversationUserResponse;
  messages: MessageResponse[];
};

export type ConversationUserResponse = {
  userId: string;
  displayName: string;
  profilePictureUrl: string;
};
