import { useRevalidator } from "react-router";
import { useState } from "react";
import { catalogApi } from "../api/catalog";
import type { CatalogBook } from "../api/types";
import { CatalogBookCard } from "../components/CatalogBookCard";
import { CatalogFilters, type TypeFilter } from "../components/CatalogFilters";

export async function clientLoader() {
  const data = await catalogApi.search();
  return { books: data.books };
}

export const meta = () => [{ title: "Catalog — Storygame" }];

const matchesTypeFilter = (book: CatalogBook, filter: TypeFilter): boolean => {
  if (filter === "all") return true;
  if (filter === "text") return book.textEditionFields.exist;
  return book.audiobookFields.exist;
};

type LoaderData = { books: CatalogBook[] };

const Catalog = ({ loaderData }: { loaderData: LoaderData }) => {
  const { books } = loaderData;
  const revalidator = useRevalidator();
  const [titleFilter, setTitleFilter] = useState("");
  const [typeFilter, setTypeFilter] = useState<TypeFilter>("all");

  const filtered = books.filter(
    (b) =>
      b.title.toLowerCase().includes(titleFilter.toLowerCase()) &&
      matchesTypeFilter(b, typeFilter),
  );

  return (
    <div>
      <h1 className="text-xl font-semibold text-gray-900 dark:text-white mb-6">Catalog</h1>

      <CatalogFilters
        titleFilter={titleFilter}
        typeFilter={typeFilter}
        onTitleChange={setTitleFilter}
        onTypeChange={setTypeFilter}
      />

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
};

export default Catalog;
