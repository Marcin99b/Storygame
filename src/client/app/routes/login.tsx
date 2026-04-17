import { redirect, useNavigate } from "react-router";
import { useState } from "react";
import { usersApi } from "../api/users";
import { ApiError } from "../api/client";

export async function clientLoader() {
  try {
    await usersApi.me();
    return redirect("/");
  } catch (e) {
    if (e instanceof ApiError && e.status === 401) {
      return null;
    }
    throw e;
  }
}

export function meta() {
  return [{ title: "Sign In — Storygame" }];
}

export default function Login() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleLogin() {
    setLoading(true);
    setError(null);
    try {
      await usersApi.login();
      navigate("/");
    } catch {
      setError("Login failed. Please try again.");
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-950">
      <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-8 w-full max-w-sm">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-1">Storygame</h1>
        <p className="text-sm text-gray-500 dark:text-gray-400 mb-8">Track your reading progress</p>

        {error && (
          <p className="text-sm text-red-600 dark:text-red-400 mb-4">{error}</p>
        )}

        <button
          onClick={handleLogin}
          disabled={loading}
          className="w-full bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white rounded-lg px-4 py-2.5 text-sm font-medium transition-colors cursor-pointer"
        >
          {loading ? "Signing in..." : "Sign In"}
        </button>
      </div>
    </div>
  );
}
