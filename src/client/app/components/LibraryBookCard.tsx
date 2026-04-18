import { useRevalidator } from "react-router";
import { useState } from "react";
import { trackingApi } from "../api/tracking";
import type { LibraryBook } from "../api/types";
import { MediaTypeBadge } from "./MediaTypeBadge";

type Props = {
  book: LibraryBook;
};

export const LibraryBookCard = ({ book }: Props) => {
  const revalidator = useRevalidator();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

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

  return (
    <div className="bg-white dark:bg-gray-900 rounded-lg border border-gray-200 dark:border-gray-800 p-4 flex items-start justify-between gap-4">
      <div className="min-w-0">
        <div className="flex items-center gap-2 mb-1">
          <h2 className="font-medium text-gray-900 dark:text-white truncate">{book.title}</h2>
          <MediaTypeBadge type={book.mediaType} />
        </div>
        <p className="text-sm text-gray-500 dark:text-gray-400 line-clamp-2">{book.description}</p>
        <p className="text-xs text-gray-400 dark:text-gray-500 mt-1">
          {book.length} {book.lengthUnit.toLowerCase()}
        </p>
        {error && <p className="text-xs text-red-500 mt-1">{error}</p>}
      </div>
      <button
        onClick={handleStartTracking}
        disabled={loading}
        className="shrink-0 text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 disabled:opacity-50 transition-colors cursor-pointer"
      >
        {loading ? "Starting..." : "Start tracking"}
      </button>
    </div>
  );
};
