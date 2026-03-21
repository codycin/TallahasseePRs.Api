export type RegisterRequest = {
  email: string;
  username: string;
  password: string;
};

export type LoginRequest = {
  emailOrUsername: string;
  password: string;
};

export type AuthResponse = {
  token: string;
  refresh: string;
  expiresAt: string;
  userId: string;
  email: string;
  username: string;
};
