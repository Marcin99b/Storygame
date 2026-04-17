import { useRevalidator } from "react-router";
import { useState } from "react";
import { catalogApi } from "../api/catalog";
import { libraryApi } from "../api/library";
import type { CatalogBook, MediaType } from "../api/types";

export async function clientLoader() {
  const data = await catalogApi.search();
  return { books: data.books };
}

export function meta() {
  return [{ title: "Catalog — Storygame" }];
}

export default function Catalog({ loaderData }: { loaderData: { books: CatalogBook[] } }) {
  const { books } = loaderData;
  const revalidator = useRevalidator();
  const [titleFilter, setTitleFilter] = useState("");
  const [typeFilter, setTypeFilter] = useState<"all" | "text" | "audio">("all");

  const filtered = books.filter((b) => {
    const matchesTitle = b.title.toLowerCase().includes(titleFilter.toLowerCase());
    const matchesType =
      typeFilter === "all" ||
      (typeFilter === "text" && b.textEditionFields.exist) ||
      (typeFilter === "audio" && b.audiobookFields.exist);
    return matchesTitle && matchesType;
  });

  return (
    <div>
      <h1 className="text-xl font-semibold text-gray-900 dark:text-white mb-6">Catalog</h1>

      <div className="flex gap-3 mb-6">
        <input
          type="text"
          placeholder="Search by title..."
          value={titleFilter}
          onChange={(e) => setTitleFilter(e.target.value)}
          className="flex-1 text-sm rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-900 dark:text-white px-3 py-2 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <div className="flex rounded-lg border border-gray-200 dark:border-gray-700 overflow-hidden text-sm">
          {(["all", "text", "audio"] as const).map((f) => (
            <button
              key={f}
              onClick={() => setTypeFilter(f)}
              className={`px-3 py-2 transition-colors cursor-pointer ${
                typeFilter === f
                  ? "bg-blue-600 text-white"
                  : "bg-white dark:bg-gray-900 text-gray-600 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-800"
              }`}
            >
              {f === "all" ? "All" : f === "text" ? "Text" : "Audio"}
            </button>
          ))}
        </div>
      </div>

      {filtered.length === 0 ? (
        <p className="text-center py-12 text-gray-500 dark:text-gray-400">No books found.</p>
      ) : (
        <div className="grid gap-3">
          {filtered.map((book) => (
            <CatalogBookCard
              key={book.id}
              book={book}
              onAdded={() => revalidator.revalidate()}
            />
          ))}
        </div>
      )}
    </div>
  );
}

function CatalogBookCard({
  book,
  onAdded,
}: {
  book: CatalogBook;
  onAdded: () => void;
}) {
  const [expanded, setExpanded] = useState(false);
  const [selectedType, setSelectedType] = useState<MediaType | "">("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const availableTypes: MediaType[] = [];
  if (book.textEditionFields.exist) availableTypes.push("Ebook", "Paperback");
  if (book.audiobookFields.exist) availableTypes.push("Audiobook");

  function lengthForType(type: MediaType): number {
    return type === "Audiobook" ? book.audiobookFields.totalMinutes : book.textEditionFields.totalPages;
  }

  function unitForType(type: MediaType): string {
    return type === "Audiobook" ? "minutes" : "pages";
  }

  async function handleAdd() {
    if (!selectedType) return;
    setLoading(true);
    setError(null);
    try {
      await libraryApi.addBook({
        catalogBookId: book.id,
        title: book.title,
        description: book.description,
        mediaType: selectedType,
        length: lengthForType(selectedType),
      });
      setExpanded(false);
      setSelectedType("");
      onAdded();
    } catch {
      setError("Failed to add book. It may already be in your library.");
    } finally {
      setLoading(false);
    }
  }

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
            {availableTypes.map((t) => (
              <option key={t} value={t}>
                {t} — {lengthForType(t)} {unitForType(t)}
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
}
