import { Link, redirect, useNavigate } from "react-router";
import { useState } from "react";
import { usersApi } from "../api/users";
import { ApiError } from "../api/client";
import {
  AuthShell,
  inputClass,
  primaryBtnClass,
  labelClass,
} from "../components/AuthShell";

export async function clientLoader() {
  try {
    await usersApi.me();
    return redirect("/");
  } catch (e) {
    if (e instanceof ApiError && e.status === 401) return null;
    throw e;
  }
}

export function meta() {
  return [{ title: "Create account — Storygame" }];
}

type Step = "form" | "verify";

export default function Register() {
  const navigate = useNavigate();
  const [step, setStep] = useState<Step>("form");
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [code, setCode] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim() || !email.trim()) return;
    setLoading(true);
    setError(null);
    try {
      await usersApi.register({ name: name.trim(), email: email.trim() });
      setStep("verify");
    } catch (err) {
      if (err instanceof ApiError && err.status === 429) {
        setError("Too many attempts. Please wait a minute.");
      } else {
        setError(
          "Couldn't create your account. That email may already be registered."
        );
      }
    } finally {
      setLoading(false);
    }
  };

  const handleVerify = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!code.trim()) return;
    setLoading(true);
    setError(null);
    try {
      await usersApi.verify({
        email: email.trim(),
        verificationCode: code.trim(),
      });
      navigate("/login");
    } catch {
      setError("Invalid or expired code.");
      setLoading(false);
    }
  };

  if (step === "verify") {
    return (
      <AuthShell
        title="Verify your email"
        subtitle={`We sent a verification code to ${email}. Paste it below to activate your account.`}
        footer={
          <Link
            to="/mail"
            className="text-plum-600 dark:text-plum-500 hover:underline"
          >
            Open the dev mailbox →
          </Link>
        }
      >
        <form onSubmit={handleVerify} className="flex flex-col gap-4">
          <div>
            <label className={labelClass}>Verification code</label>
            <input
              type="text"
              value={code}
              onChange={(e) => setCode(e.target.value)}
              placeholder="Paste the code from the email"
              required
              autoFocus
              className={inputClass}
            />
          </div>
          {error && (
            <p className="text-sm text-red-600 dark:text-red-400">{error}</p>
          )}
          <button type="submit" disabled={loading} className={primaryBtnClass}>
            {loading ? "Verifying..." : "Verify & continue"}
          </button>
          <p className="text-xs text-ink-700/60 dark:text-paper-200/60 text-center">
            After verifying, you'll sign in from the login page.
          </p>
        </form>
      </AuthShell>
    );
  }

  return (
    <AuthShell
      title="Create your account"
      subtitle="Start tracking what you read and listen to."
      footer={
        <span className="text-ink-700/70 dark:text-paper-200/70">
          Already have an account?{" "}
          <Link
            to="/login"
            className="text-plum-600 dark:text-plum-500 hover:underline font-medium"
          >
            Sign in
          </Link>
        </span>
      }
    >
      <form onSubmit={handleRegister} className="flex flex-col gap-4">
        <div>
          <label className={labelClass}>Display name</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="How we'll greet you"
            required
            autoFocus
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass}>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="you@example.com"
            required
            className={inputClass}
          />
        </div>
        {error && (
          <p className="text-sm text-red-600 dark:text-red-400">{error}</p>
        )}
        <button type="submit" disabled={loading} className={primaryBtnClass}>
          {loading ? "Creating account..." : "Create account"}
        </button>
      </form>
    </AuthShell>
  );
}
