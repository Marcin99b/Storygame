import { useRevalidator } from "react-router";
import { useState } from "react";
import { trackingApi } from "../api/tracking";
import type { LibraryBook, Tracking } from "../api/types";
import { MediaTypeBadge } from "./MediaTypeBadge";

type Props = {
  book: LibraryBook;
  tracking?: Tracking;
};

const ProgressBar = ({
  current,
  total,
}: {
  current: number;
  total: number;
}) => {
  const pct = total > 0 ? Math.min(100, Math.round((current / total) * 100)) : 0;
  return (
    <div
      className="w-full bg-paper-100 dark:bg-ink-900 rounded-full h-2 mt-3 overflow-hidden"
      role="progressbar"
      aria-valuenow={pct}
      aria-valuemin={0}
      aria-valuemax={100}
    >
      <div
        className="h-full rounded-full bg-gradient-to-r from-plum-600 to-amber-accent transition-all"
        style={{ width: `${pct}%` }}
      />
    </div>
  );
};

const UpdateProgressForm = ({
  tracking,
  unit,
  onUpdated,
  onCancel,
}: {
  tracking: Tracking;
  unit: string;
  onUpdated: () => void;
  onCancel: () => void;
}) => {
  const [value, setValue] = useState(String(tracking.currentIndex));
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const parsed = parseInt(value, 10);
    if (isNaN(parsed) || parsed < 0 || parsed > tracking.totalLength) {
      setError(`Enter a value between 0 and ${tracking.totalLength}.`);
      return;
    }
    setLoading(true);
    setError(null);
    try {
      await trackingApi.updateIndex(tracking.id, parsed);
      onUpdated();
    } catch {
      setError("Failed to update.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-wrap items-center gap-2 mt-3">
      <input
        type="number"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        min={0}
        max={tracking.totalLength}
        autoFocus
        className="w-28 text-sm rounded-lg border border-paper-200 dark:border-ink-700 bg-paper-50 dark:bg-ink-900 text-ink-800 dark:text-paper-100 px-3 py-1.5 focus:outline-none focus:ring-2 focus:ring-plum-500"
      />
      <span className="text-xs text-ink-700/60 dark:text-paper-200/60">
        of {tracking.totalLength} {unit}
      </span>
      <button
        type="submit"
        disabled={loading}
        className="text-xs bg-plum-600 hover:bg-plum-700 text-white rounded-md px-3 py-1.5 disabled:opacity-50 cursor-pointer"
      >
        {loading ? "Saving..." : "Save"}
      </button>
      <button
        type="button"
        onClick={onCancel}
        className="text-xs text-ink-700/70 dark:text-paper-200/70 hover:text-ink-800 dark:hover:text-paper-100 cursor-pointer"
      >
        Cancel
      </button>
      {error && <span className="text-xs text-red-600 basis-full">{error}</span>}
    </form>
  );
};

export const LibraryBookCard = ({ book, tracking }: Props) => {
  const revalidator = useRevalidator();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [editingProgress, setEditingProgress] = useState(false);

  const unit = book.lengthUnit.toLowerCase();

  const handleStartTracking = async () => {
    setLoading(true);
    setError(null);
    try {
      await trackingApi.startTracking(book.id);
      revalidator.revalidate();
    } catch {
      setError("Failed to start tracking.");
    } finally {
      setLoading(false);
    }
  };

  const addedDate = new Date(book.addedToLibraryAt).toLocaleDateString(
    undefined,
    { month: "short", day: "numeric", year: "numeric" }
  );

  const pct = tracking
    ? Math.round((tracking.currentIndex / Math.max(1, tracking.totalLength)) * 100)
    : 0;

  return (
    <article className="bg-white dark:bg-ink-800 rounded-xl border border-paper-200 dark:border-ink-700 p-5">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2 mb-1">
            <h2 className="font-display text-lg font-semibold text-ink-800 dark:text-paper-100 truncate">
              {book.title}
            </h2>
            <MediaTypeBadge type={book.mediaType} />
            {tracking?.isFinished && (
              <span className="inline-flex items-center gap-1 text-xs px-2 py-0.5 rounded-full bg-emerald-500/10 text-emerald-700 dark:text-emerald-400 ring-1 ring-inset ring-emerald-500/20 font-medium">
                ✓ Finished
              </span>
            )}
          </div>
          <p className="text-sm text-ink-700/70 dark:text-paper-200/70 line-clamp-2">
            {book.description}
          </p>
          <div className="flex flex-wrap items-center gap-x-3 gap-y-1 mt-2 text-xs text-ink-700/60 dark:text-paper-200/60">
            <span>
              {book.length} {unit}
            </span>
            <span aria-hidden>·</span>
            <span>Added {addedDate}</span>
          </div>
          {error && <p className="text-xs text-red-600 mt-1">{error}</p>}
        </div>
        {!tracking && (
          <button
            onClick={handleStartTracking}
            disabled={loading}
            className="shrink-0 text-sm rounded-full px-3.5 py-1.5 border border-plum-600 text-plum-600 dark:text-plum-500 hover:bg-plum-600 hover:text-white disabled:opacity-50 transition-colors cursor-pointer"
          >
            {loading ? "Starting..." : "Start tracking"}
          </button>
        )}
      </div>

      {tracking && !tracking.isFinished && (
        <div className="mt-2">
          {editingProgress ? (
            <UpdateProgressForm
              tracking={tracking}
              unit={unit}
              onUpdated={() => {
                setEditingProgress(false);
                revalidator.revalidate();
              }}
              onCancel={() => setEditingProgress(false)}
            />
          ) : (
            <button
              type="button"
              onClick={() => setEditingProgress(true)}
              className="group w-full text-left cursor-pointer"
            >
              <div className="flex items-center justify-between text-xs text-ink-700/70 dark:text-paper-200/70">
                <span>
                  <span className="font-medium text-ink-800 dark:text-paper-100">
                    {tracking.currentIndex}
                  </span>{" "}
                  / {tracking.totalLength} {unit} · {pct}%
                </span>
                <span className="text-plum-600 dark:text-plum-500 opacity-0 group-hover:opacity-100 transition-opacity">
                  Update progress →
                </span>
              </div>
              <ProgressBar
                current={tracking.currentIndex}
                total={tracking.totalLength}
              />
            </button>
          )}
        </div>
      )}

      {tracking?.isFinished && (
        <ProgressBar current={tracking.totalLength} total={tracking.totalLength} />
      )}
    </article>
  );
};
