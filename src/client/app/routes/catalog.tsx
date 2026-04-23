import { useRevalidator } from "react-router";
import { useEffect, useRef, useState } from "react";
import { catalogApi } from "../api/catalog";
import type { CatalogBook } from "../api/types";
import { CatalogBookCard } from "../components/CatalogBookCard";
import { CatalogFilters, type TypeFilter } from "../components/CatalogFilters";

export async function clientLoader() {
  const data = await catalogApi.search();
  return { books: data.books };
}

export const meta = () => [{ title: "Catalog — Storygame" }];

type LoaderData = { books: CatalogBook[] };

const typeFilterToParams = (filter: TypeFilter) => {
  if (filter === "text") return { hasTextEdition: true };
  if (filter === "audio") return { hasAudiobook: true };
  return {};
};

const Catalog = ({ loaderData }: { loaderData: LoaderData }) => {
  const [books, setBooks] = useState(loaderData.books);
  const revalidator = useRevalidator();
  const [titleFilter, setTitleFilter] = useState("");
  const [typeFilter, setTypeFilter] = useState<TypeFilter>("all");
  const [searching, setSearching] = useState(false);
  const debounceRef = useRef<number | null>(null);

  useEffect(() => {
    setBooks(loaderData.books);
  }, [loaderData.books]);

  useEffect(() => {
    if (debounceRef.current) window.clearTimeout(debounceRef.current);
    debounceRef.current = window.setTimeout(async () => {
      setSearching(true);
      try {
        const data = await catalogApi.search({
          titleContains: titleFilter.trim() || undefined,
          ...typeFilterToParams(typeFilter),
        });
        setBooks(data.books);
      } finally {
        setSearching(false);
      }
    }, 250);
    return () => {
      if (debounceRef.current) window.clearTimeout(debounceRef.current);
    };
  }, [titleFilter, typeFilter]);

  return (
    <div>
      <header className="mb-8">
        <h1 className="font-display text-3xl font-semibold text-ink-800 dark:text-paper-100">
          Catalog
        </h1>
        <p className="text-sm text-ink-700/70 dark:text-paper-200/70 mt-1">
          Browse curated titles and add them to your library.
        </p>
      </header>

      <CatalogFilters
        titleFilter={titleFilter}
        typeFilter={typeFilter}
        onTitleChange={setTitleFilter}
        onTypeChange={setTypeFilter}
      />

      {searching && (
        <p className="text-xs text-ink-700/50 dark:text-paper-200/50 mb-3">
          Searching...
        </p>
      )}

      {books.length === 0 ? (
        <div className="text-center py-16 text-ink-700/60 dark:text-paper-200/60">
          <p className="font-display text-xl mb-1">No matches found</p>
          <p className="text-sm">Try a different title or format.</p>
        </div>
      ) : (
        <div className="grid gap-3">
          {books.map((book) => (
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
};

export default Catalog;
