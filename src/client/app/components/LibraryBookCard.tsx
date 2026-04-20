import { useRevalidator } from "react-router";
import { useState } from "react";
import { trackingApi } from "../api/tracking";
import type { LibraryBook, Tracking } from "../api/types";
import { MediaTypeBadge } from "./MediaTypeBadge";

type Props = {
  book: LibraryBook;
  tracking?: Tracking;
};

const ProgressBar = ({ current, total }: { current: number; total: number }) => {
  const pct = total > 0 ? Math.round((current / total) * 100) : 0;
  return (
    <div className="w-full bg-gray-100 dark:bg-gray-800 rounded-full h-1.5 mt-2">
      <div
        className="bg-blue-500 h-1.5 rounded-full transition-all"
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
    if (isNaN(parsed) || parsed < 0 || parsed > tracking.totalLength) return;
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
    <form onSubmit={handleSubmit} className="flex items-center gap-2 mt-2">
      <input
        type="number"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        min={0}
        max={tracking.totalLength}
        autoFocus
        className="w-24 text-sm rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 text-gray-900 dark:text-white px-2 py-1 focus:outline-none focus:ring-2 focus:ring-blue-500"
      />
      <span className="text-xs text-gray-400 dark:text-gray-500">/ {tracking.totalLength} {unit}</span>
      <button
        type="submit"
        disabled={loading}
        className="text-xs text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 disabled:opacity-50 cursor-pointer"
      >
        {loading ? "Saving..." : "Save"}
      </button>
      <button
        type="button"
        onClick={onCancel}
        className="text-xs text-gray-400 dark:text-gray-500 hover:text-gray-600 dark:hover:text-gray-300 cursor-pointer"
      >
        Cancel
      </button>
      {error && <span className="text-xs text-red-500">{error}</span>}
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
      await trackingApi.startTracking(book.id, book.length);
      revalidator.revalidate();
    } catch {
      setError("Failed to start tracking.");
    } finally {
      setLoading(false);
    }
  };

  const addedDate = new Date(book.addedToLibraryAt).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });

  return (
    <div className="bg-white dark:bg-gray-900 rounded-lg border border-gray-200 dark:border-gray-800 p-4">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0 flex-1">
          <div className="flex items-center gap-2 mb-1">
            <h2 className="font-medium text-gray-900 dark:text-white truncate">{book.title}</h2>
            <MediaTypeBadge type={book.mediaType} />
          </div>
          <p className="text-sm text-gray-500 dark:text-gray-400 line-clamp-2">{book.description}</p>
          <div className="flex items-center gap-3 mt-1.5">
            <span className="text-xs text-gray-400 dark:text-gray-500">
              {book.length} {unit}
            </span>
            {!tracking && (
              <span className="text-xs text-gray-400 dark:text-gray-500">Added {addedDate}</span>
            )}
            {tracking && tracking.isFinished && (
              <span className="text-xs font-medium text-green-600 dark:text-green-400">✓ Finished</span>
            )}
            {tracking && !tracking.isFinished && !editingProgress && (
              <button
                onClick={() => setEditingProgress(true)}
                className="text-xs text-gray-400 dark:text-gray-500 hover:text-blue-600 dark:hover:text-blue-400 transition-colors cursor-pointer"
              >
                {tracking.currentIndex} / {tracking.totalLength} {unit} — update
              </button>
            )}
          </div>
          {error && <p className="text-xs text-red-500 mt-1">{error}</p>}
          {tracking && !tracking.isFinished && editingProgress && (
            <UpdateProgressForm
              tracking={tracking}
              unit={unit}
              onUpdated={() => {
                setEditingProgress(false);
                revalidator.revalidate();
              }}
              onCancel={() => setEditingProgress(false)}
            />
          )}
          {tracking && !tracking.isFinished && !editingProgress && (
            <ProgressBar current={tracking.currentIndex} total={tracking.totalLength} />
          )}
        </div>
        {!tracking && (
          <button
            onClick={handleStartTracking}
            disabled={loading}
            className="shrink-0 text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 disabled:opacity-50 transition-colors cursor-pointer"
          >
            {loading ? "Starting..." : "Start tracking"}
          </button>
        )}
      </div>
    </div>
  );
};
