export class ApiError extends Error {
  constructor(public readonly status: number, message: string) {
    super(message);
  }
}

let csrfToken: string | null = null;

async function getCsrfToken(): Promise<string> {
  if (csrfToken !== null) return csrfToken;
  const res = await fetch("/api/users/CSRF", { credentials: "include" });
  if (!res.ok) throw new ApiError(res.status, "Failed to fetch CSRF token");
  csrfToken = await res.text();
  return csrfToken;
}

export function resetCsrfToken(): void {
  csrfToken = null;
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(init?.headers as Record<string, string>),
  };

  if (init?.method === "POST") {
    headers["X-CSRF-TOKEN"] = await getCsrfToken();
  }

  const res = await fetch(`/api${path}`, {
    credentials: "include",
    ...init,
    headers,
  });

  if (!res.ok) {
    throw new ApiError(res.status, `${res.status} ${res.statusText}`);
  }

  const text = await res.text();
  if (!text) return undefined as T;
  return JSON.parse(text) as T;
}

export const api = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, body?: unknown) =>
    request<T>(path, {
      method: "POST",
      body: body !== undefined ? JSON.stringify(body) : undefined,
    }),
};
