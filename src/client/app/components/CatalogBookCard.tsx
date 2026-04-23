import { useState } from "react";
import { libraryApi } from "../api/library";
import type { CatalogBook, MediaType } from "../api/types";

type Props = {
  book: CatalogBook;
  onAdded: () => void;
};

const lengthForType = (book: CatalogBook, type: MediaType): number =>
  type === "Audiobook"
    ? book.audiobookFields.totalMinutes
    : book.textEditionFields.totalPages;

const unitForType = (type: MediaType): string =>
  type === "Audiobook" ? "min" : "pages";

const availableMediaTypes = (book: CatalogBook): MediaType[] => {
  const types: MediaType[] = [];
  if (book.textEditionFields.exist) types.push("Ebook", "Paperback");
  if (book.audiobookFields.exist) types.push("Audiobook");
  return types;
};

const FORMAT_ICON: Record<MediaType, string> = {
  Ebook: "📖",
  Paperback: "📚",
  Audiobook: "🎧",
};

export const CatalogBookCard = ({ book, onAdded }: Props) => {
  const [expanded, setExpanded] = useState(false);
  const [selectedType, setSelectedType] = useState<MediaType | "">("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [added, setAdded] = useState(false);

  const handleAdd = async () => {
    if (!selectedType) return;
    setLoading(true);
    setError(null);
    try {
      await libraryApi.addBook({
        catalogBookId: book.id,
        imageId: book.imageId ?? null,
        title: book.title,
        description: book.description,
        mediaType: selectedType,
        length: lengthForType(book, selectedType),
      });
      setAdded(true);
      setTimeout(() => {
        setExpanded(false);
        setSelectedType("");
        setAdded(false);
        onAdded();
      }, 900);
    } catch {
      setError("Couldn't add this book. It may already be in your library.");
    } finally {
      setLoading(false);
    }
  };

  const formats = availableMediaTypes(book);

  return (
    <article className="bg-white dark:bg-ink-800 rounded-xl border border-paper-200 dark:border-ink-700 p-5 hover:border-plum-500/40 dark:hover:border-plum-500/40 transition-colors">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0 flex-1">
          <h2 className="font-display text-lg font-semibold text-ink-800 dark:text-paper-100 mb-1">
            {book.title}
          </h2>
          <p className="text-sm text-ink-700/70 dark:text-paper-200/70 line-clamp-2">
            {book.description}
          </p>
          <div className="flex flex-wrap gap-2 mt-3">
            {book.textEditionFields.exist && (
              <span className="inline-flex items-center gap-1 text-xs px-2 py-0.5 rounded-full bg-paper-100 dark:bg-ink-900 text-ink-700 dark:text-paper-200">
                <span aria-hidden>📖</span>
                {book.textEditionFields.totalPages} pages
              </span>
            )}
            {book.audiobookFields.exist && (
              <span className="inline-flex items-center gap-1 text-xs px-2 py-0.5 rounded-full bg-paper-100 dark:bg-ink-900 text-ink-700 dark:text-paper-200">
                <span aria-hidden>🎧</span>
                {book.audiobookFields.totalMinutes} min
              </span>
            )}
          </div>
        </div>
        <button
          onClick={() => setExpanded((v) => !v)}
          className={`shrink-0 text-sm rounded-full px-3.5 py-1.5 border transition-colors cursor-pointer ${
            expanded
              ? "border-paper-200 dark:border-ink-700 text-ink-700 dark:text-paper-200 hover:bg-paper-100 dark:hover:bg-ink-900"
              : "border-plum-600 text-plum-600 dark:text-plum-500 hover:bg-plum-600 hover:text-white"
          }`}
        >
          {expanded ? "Cancel" : "Add to library"}
        </button>
      </div>

      {expanded && (
        <div className="mt-4 pt-4 border-t border-paper-200 dark:border-ink-700">
          <p className="text-xs uppercase tracking-wide text-ink-700/60 dark:text-paper-200/60 mb-2">
            Choose a format
          </p>
          <div className="flex flex-wrap gap-2 mb-3">
            {formats.map((t) => (
              <button
                key={t}
                onClick={() => setSelectedType(t)}
                className={`text-sm px-3 py-1.5 rounded-lg border transition-colors cursor-pointer ${
                  selectedType === t
                    ? "bg-plum-600 text-white border-plum-600"
                    : "border-paper-200 dark:border-ink-700 text-ink-700 dark:text-paper-200 hover:border-plum-500"
                }`}
              >
                <span className="mr-1.5">{FORMAT_ICON[t]}</span>
                {t} · {lengthForType(book, t)} {unitForType(t)}
              </button>
            ))}
          </div>
          <div className="flex items-center gap-3">
            <button
              onClick={handleAdd}
              disabled={!selectedType || loading || added}
              className="text-sm bg-plum-600 hover:bg-plum-700 disabled:opacity-50 text-white rounded-lg px-4 py-1.5 transition-colors cursor-pointer"
            >
              {added ? "✓ Added" : loading ? "Adding..." : "Add to library"}
            </button>
            {error && <p className="text-xs text-red-600">{error}</p>}
          </div>
        </div>
      )}
    </article>
  );
};
