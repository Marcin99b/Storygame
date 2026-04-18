import { Link } from "react-router";
import { libraryApi } from "../api/library";
import type { LibraryBook } from "../api/types";
import { LibraryBookCard } from "../components/LibraryBookCard";

export async function clientLoader() {
  const data = await libraryApi.getBooks();
  return { books: data.books ?? [] };
}

export const meta = () => [{ title: "Library — Storygame" }];

const EmptyLibrary = () => (
  <div className="text-center py-16 text-gray-500 dark:text-gray-400">
    <p className="mb-2">Your library is empty.</p>
    <Link to="/catalog" className="text-sm text-blue-600 dark:text-blue-400 hover:underline">
      Browse the catalog to add your first book.
    </Link>
  </div>
);

type LoaderData = { books: LibraryBook[] };

const Library = ({ loaderData }: { loaderData: LoaderData }) => {
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
            <LibraryBookCard key={book.id} book={book} />
          ))}
        </div>
      )}
    </div>
  );
};

export default Library;
