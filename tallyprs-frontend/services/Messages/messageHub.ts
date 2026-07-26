import * as signalR from "@microsoft/signalr";
import { API_BASE_URL } from "@/lib/api";
import { RefreshAccessToken } from "../authService";

let refreshPromise: Promise<string | null> | null = null;

function isTokenExpiringSoon(token: string, bufferSeconds = 60) {
  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    const expiresAtMs = payload.exp * 1000;

    return Date.now() + bufferSeconds * 1000 >= expiresAtMs;
  } catch {
    return true;
  }
}

async function getValidAccessToken(forceRefresh = false): Promise<string> {
  const currentToken = localStorage.getItem("accessToken");

  if (!forceRefresh && currentToken && !isTokenExpiringSoon(currentToken)) {
    return currentToken;
  }

  if (!refreshPromise) {
    refreshPromise = RefreshAccessToken()
      .then((token) => {
        return token?.accessToken ?? null;
      })
      .catch((err) => {
        console.error("Token refresh failed:", err);
        return null;
      })
      .finally(() => {
        refreshPromise = null;
      });
  }

  return (await refreshPromise) ?? "";
}

export function createMessageConnection() {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${API_BASE_URL}/hubs/messages`, {
      accessTokenFactory: async () => {
        return await getValidAccessToken();
      },
    })
    .withAutomaticReconnect([0, 2000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Warning)
    .build();
}

export async function startMessageConnection(
  connection: signalR.HubConnection,
): Promise<signalR.HubConnection> {
  if (connection.state === signalR.HubConnectionState.Connected) {
    return connection;
  }

  if (connection.state !== signalR.HubConnectionState.Disconnected) {
    return connection;
  }

  try {
    await connection.start();
    return connection;
  } catch (err) {
    const message = err instanceof Error ? err.message : String(err);

    // React Strict Mode/dev cleanup can stop the connection while negotiation is happening.
    // Do not refresh/retry this old connection.
    if (message.includes("stopped during negotiation")) {
      console.warn("SignalR start was cancelled during negotiation.");
      throw err;
    }

    console.warn("SignalR start failed. Forcing token refresh once...", err);

    const refreshedToken = await getValidAccessToken(true);

    if (!refreshedToken) {
      throw err;
    }

    if (connection.state !== signalR.HubConnectionState.Disconnected) {
      return connection;
    }

    await connection.start();
    return connection;
  }
}
