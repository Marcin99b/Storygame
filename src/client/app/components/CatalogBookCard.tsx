import { useState } from "react";
import { libraryApi } from "../api/library";
import type { CatalogBook, MediaType } from "../api/types";

type Props = {
  book: CatalogBook;
  onAdded: () => void;
};

const lengthForType = (book: CatalogBook, type: MediaType): number =>
  type === "Audiobook" ? book.audiobookFields.totalMinutes : book.textEditionFields.totalPages;

const unitForType = (type: MediaType): string =>
  type === "Audiobook" ? "minutes" : "pages";

const availableMediaTypes = (book: CatalogBook): MediaType[] => {
  const types: MediaType[] = [];
  if (book.textEditionFields.exist) types.push("Ebook", "Paperback");
  if (book.audiobookFields.exist) types.push("Audiobook");
  return types;
};

export const CatalogBookCard = ({ book, onAdded }: Props) => {
  const [expanded, setExpanded] = useState(false);
  const [selectedType, setSelectedType] = useState<MediaType | "">("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleAdd = async () => {
    if (!selectedType) return;
    setLoading(true);
    setError(null);
    try {
      await libraryApi.addBook({
        catalogBookId: book.id,
        title: book.title,
        description: book.description,
        mediaType: selectedType,
        length: lengthForType(book, selectedType),
      });
      setExpanded(false);
      setSelectedType("");
      onAdded();
    } catch {
      setError("Failed to add book. It may already be in your library.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-white dark:bg-gray-900 rounded-lg border border-gray-200 dark:border-gray-800 p-4">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0">
          <h2 className="font-medium text-gray-900 dark:text-white mb-1">{book.title}</h2>
          <p className="text-sm text-gray-500 dark:text-gray-400 line-clamp-2">{book.description}</p>
          <div className="flex gap-3 mt-2 text-xs text-gray-400 dark:text-gray-500">
            {book.textEditionFields.exist && (
              <span>{book.textEditionFields.totalPages} pages</span>
            )}
            {book.audiobookFields.exist && (
              <span>{book.audiobookFields.totalMinutes} min audiobook</span>
            )}
          </div>
        </div>
        <button
          onClick={() => setExpanded((v) => !v)}
          className="shrink-0 text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 transition-colors cursor-pointer"
        >
          {expanded ? "Cancel" : "Add to Library"}
        </button>
      </div>

      {expanded && (
        <div className="mt-4 pt-4 border-t border-gray-100 dark:border-gray-800 flex items-center gap-3">
          <select
            value={selectedType}
            onChange={(e) => setSelectedType(e.target.value as MediaType)}
            className="text-sm rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 text-gray-900 dark:text-white px-3 py-1.5 focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">Select format...</option>
            {availableMediaTypes(book).map((t) => (
              <option key={t} value={t}>
                {t} — {lengthForType(book, t)} {unitForType(t)}
              </option>
            ))}
          </select>
          <button
            onClick={handleAdd}
            disabled={!selectedType || loading}
            className="text-sm bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white rounded-lg px-4 py-1.5 transition-colors cursor-pointer"
          >
            {loading ? "Adding..." : "Confirm"}
          </button>
          {error && <p className="text-xs text-red-500">{error}</p>}
        </div>
      )}
    </div>
  );
};
