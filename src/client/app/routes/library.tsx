import { Link, useRevalidator } from "react-router";
import { useState } from "react";
import { libraryApi } from "../api/library";
import { trackingApi } from "../api/tracking";
import type { LibraryBook } from "../api/types";

export async function clientLoader() {
  const data = await libraryApi.getBooks();
  return { books: data.books ?? [] };
}

export function meta() {
  return [{ title: "Library — Storygame" }];
}

export default function Library({ loaderData }: { loaderData: { books: LibraryBook[] } }) {
  const { books } = loaderData;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-xl font-semibold text-gray-900 dark:text-white">My Library</h1>
        <Link
          to="/catalog"
          className="text-sm bg-blue-600 hover:bg-blue-700 text-white rounded-lg px-4 py-2 transition-colors"
        >
          Browse Catalog
        </Link>
      </div>

      {books.length === 0 ? (
        <EmptyLibrary />
      ) : (
        <div className="grid gap-3">
          {books.map((book) => (
            <BookCard key={book.id} book={book} />
          ))}
        </div>
      )}
    </div>
  );
}

function EmptyLibrary() {
  return (
    <div className="text-center py-16 text-gray-500 dark:text-gray-400">
      <p className="mb-2">Your library is empty.</p>
      <Link to="/catalog" className="text-sm text-blue-600 dark:text-blue-400 hover:underline">
        Browse the catalog to add your first book.
      </Link>
    </div>
  );
}

function BookCard({ book }: { book: LibraryBook }) {
  const revalidator = useRevalidator();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleStartTracking() {
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
  }

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
}

function MediaTypeBadge({ type }: { type: string }) {
  const colors: Record<string, string> = {
    Ebook: "bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-300",
    Paperback: "bg-green-100 text-green-700 dark:bg-green-900/40 dark:text-green-300",
    Audiobook: "bg-purple-100 text-purple-700 dark:bg-purple-900/40 dark:text-purple-300",
  };
  return (
    <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${colors[type] ?? ""}`}>
      {type}
    </span>
  );
}
