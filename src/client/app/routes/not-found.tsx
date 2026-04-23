import { Link } from "react-router";

export default function NotFound() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-paper-50 dark:bg-ink-900 p-6">
      <div className="text-center max-w-sm">
        <p className="font-display text-7xl font-semibold bg-gradient-to-br from-plum-600 to-amber-accent bg-clip-text text-transparent">
          404
        </p>
        <h1 className="font-display text-2xl font-semibold text-ink-800 dark:text-paper-100 mt-4 mb-2">
          This chapter isn't written yet
        </h1>
        <p className="text-sm text-ink-700/70 dark:text-paper-200/70 mb-6">
          The page you're looking for doesn't exist.
        </p>
        <Link
          to="/"
          className="inline-block text-sm bg-plum-600 hover:bg-plum-700 text-white rounded-lg px-4 py-2 transition-colors"
        >
          Back to your library
        </Link>
      </div>
    </div>
  );
}
