import { Link, useRevalidator } from "react-router";
import { useState } from "react";
import { libraryApi } from "../api/library";
import { trackingApi } from "../api/tracking";
import type { LibraryBook, Tracking } from "../api/types";
import { LibraryBookCard } from "../components/LibraryBookCard";
import { AddCustomBookModal } from "../components/AddCustomBookModal";

export async function clientLoader() {
  const [libraryData, trackingData] = await Promise.all([
    libraryApi.getBooks(),
    trackingApi.getTrackings(),
  ]);
  const trackingsMap: Record<string, Tracking> = Object.fromEntries(
    (trackingData.trackings ?? []).map((t) => [t.libraryBookId, t]),
  );
  return { books: libraryData.books ?? [], trackingsMap };
}

export const meta = () => [{ title: "Library — Storygame" }];

const EmptyLibrary = ({ onAddCustom }: { onAddCustom: () => void }) => (
  <div className="text-center py-16 text-gray-500 dark:text-gray-400">
    <p className="mb-2">Your library is empty.</p>
    <div className="flex items-center justify-center gap-3 mt-3">
      <Link to="/catalog" className="text-sm text-blue-600 dark:text-blue-400 hover:underline">
        Browse the catalog
      </Link>
      <span className="text-gray-300 dark:text-gray-600">or</span>
      <button
        onClick={onAddCustom}
        className="text-sm text-blue-600 dark:text-blue-400 hover:underline cursor-pointer"
      >
        add a custom book
      </button>
    </div>
  </div>
);

type LoaderData = { books: LibraryBook[]; trackingsMap: Record<string, Tracking> };

const Library = ({ loaderData }: { loaderData: LoaderData }) => {
  const { books, trackingsMap } = loaderData;
  const revalidator = useRevalidator();
  const [showAddModal, setShowAddModal] = useState(false);

  const handleBookAdded = () => {
    setShowAddModal(false);
    revalidator.revalidate();
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-xl font-semibold text-gray-900 dark:text-white">My Library</h1>
        <div className="flex gap-2">
          <button
            onClick={() => setShowAddModal(true)}
            className="text-sm border border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-800 text-gray-700 dark:text-gray-300 rounded-lg px-4 py-2 transition-colors cursor-pointer"
          >
            Add Custom Book
          </button>
          <Link
            to="/catalog"
            className="text-sm bg-blue-600 hover:bg-blue-700 text-white rounded-lg px-4 py-2 transition-colors"
          >
            Browse Catalog
          </Link>
        </div>
      </div>

      {books.length === 0 ? (
        <EmptyLibrary onAddCustom={() => setShowAddModal(true)} />
      ) : (
        <div className="grid gap-3">
          {books.map((book) => (
            <LibraryBookCard
              key={book.id}
              book={book}
              tracking={trackingsMap[book.id]}
            />
          ))}
        </div>
      )}

      {showAddModal && (
        <AddCustomBookModal
          onClose={() => setShowAddModal(false)}
          onAdded={handleBookAdded}
        />
      )}
    </div>
  );
};

export default Library;
