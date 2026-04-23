import { Link } from "react-router";
import { useState } from "react";
import { mailApi } from "../api/mail";
import { ApiError } from "../api/client";
import type { MailMessage } from "../api/types";
import { inputClass, labelClass } from "../components/AuthShell";

export function meta() {
  return [{ title: "Dev mailbox — Storygame" }];
}

const formatDate = (iso: string) => {
  const d = new Date(iso);
  return d.toLocaleString(undefined, {
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};

const MessageItem = ({ msg }: { msg: MailMessage }) => {
  const [copied, setCopied] = useState(false);
  const copy = async () => {
    try {
      await navigator.clipboard.writeText(msg.message);
      setCopied(true);
      setTimeout(() => setCopied(false), 1500);
    } catch {
      // ignore
    }
  };
  return (
    <li className="bg-white dark:bg-ink-800 rounded-xl border border-paper-200 dark:border-ink-700 p-5">
      <div className="flex items-start justify-between gap-3 mb-2">
        <div>
          <p className="font-display text-lg font-semibold text-ink-800 dark:text-paper-100">
            {msg.subject}
          </p>
          <p className="text-xs text-ink-700/60 dark:text-paper-200/60 mt-0.5">
            To {msg.receiver} · {formatDate(msg.sentAt)}
          </p>
        </div>
        <button
          onClick={copy}
          className="shrink-0 text-xs font-medium px-2.5 py-1 rounded-md border border-paper-200 dark:border-ink-700 text-ink-700 dark:text-paper-200 hover:bg-paper-100 dark:hover:bg-ink-700 transition-colors cursor-pointer"
        >
          {copied ? "Copied!" : "Copy code"}
        </button>
      </div>
      <pre className="font-mono text-sm bg-paper-100 dark:bg-ink-900 text-ink-800 dark:text-paper-100 rounded-lg p-3 whitespace-pre-wrap break-all">
        {msg.message}
      </pre>
    </li>
  );
};

export default function Mail() {
  const [email, setEmail] = useState("");
  const [messages, setMessages] = useState<MailMessage[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!email.trim()) return;
    setLoading(true);
    setError(null);
    try {
      const data = await mailApi.getMessages(email.trim());
      const sorted = [...data].sort(
        (a, b) => new Date(b.sentAt).getTime() - new Date(a.sentAt).getTime()
      );
      setMessages(sorted);
    } catch (err) {
      if (err instanceof ApiError && err.status === 404) {
        setError(
          "Dev mailbox is only available when the backend runs in Development mode."
        );
      } else {
        setError("Couldn't load messages. Try again.");
      }
      setMessages(null);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-paper-50 dark:bg-ink-900 p-6">
      <div className="max-w-2xl mx-auto">
        <div className="flex items-center justify-between mb-8">
          <div className="flex items-center gap-2">
            <span className="inline-block w-9 h-9 rounded-lg bg-gradient-to-br from-plum-600 to-amber-accent" />
            <div>
              <h1 className="font-display text-2xl font-semibold text-ink-800 dark:text-paper-100">
                Dev mailbox
              </h1>
              <p className="text-xs text-ink-700/60 dark:text-paper-200/60">
                Read simulated emails for any address (development only).
              </p>
            </div>
          </div>
          <Link
            to="/login"
            className="text-sm text-plum-600 dark:text-plum-500 hover:underline"
          >
            ← Back to sign in
          </Link>
        </div>

        <form
          onSubmit={load}
          className="bg-white dark:bg-ink-800 rounded-2xl border border-paper-200 dark:border-ink-700 p-5 mb-6 flex gap-3 items-end"
        >
          <div className="flex-1">
            <label className={labelClass}>Email address</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="you@example.com"
              required
              className={inputClass}
            />
          </div>
          <button
            type="submit"
            disabled={loading}
            className="shrink-0 bg-plum-600 hover:bg-plum-700 disabled:opacity-50 text-white rounded-lg px-5 py-2.5 text-sm font-medium transition-colors cursor-pointer"
          >
            {loading ? "Loading..." : "Fetch mail"}
          </button>
        </form>

        {error && (
          <div className="bg-red-50 dark:bg-red-950/40 border border-red-200 dark:border-red-900 text-red-700 dark:text-red-300 rounded-lg p-4 mb-4 text-sm">
            {error}
          </div>
        )}

        {messages !== null && (
          <>
            {messages.length === 0 ? (
              <div className="text-center py-12 text-ink-700/60 dark:text-paper-200/60">
                No messages for <span className="font-medium">{email}</span>{" "}
                yet.
              </div>
            ) : (
              <ul className="flex flex-col gap-3">
                {messages.map((m, i) => (
                  <MessageItem key={`${m.sentAt}-${i}`} msg={m} />
                ))}
              </ul>
            )}
          </>
        )}
      </div>
    </div>
  );
}
