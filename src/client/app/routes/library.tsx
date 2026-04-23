import { Link, useRevalidator } from "react-router";
import { useMemo, useState } from "react";
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
    (trackingData.trackings ?? []).map((t) => [t.libraryBookId, t])
  );
  return { books: libraryData.books ?? [], trackingsMap };
}

export const meta = () => [{ title: "Library — Storygame" }];

type LoaderData = {
  books: LibraryBook[];
  trackingsMap: Record<string, Tracking>;
};

type ViewFilter = "all" | "reading" | "finished" | "not-started";

const VIEW_LABELS: Record<ViewFilter, string> = {
  all: "All",
  reading: "Reading",
  finished: "Finished",
  "not-started": "Not started",
};

const EmptyLibrary = ({ onAddCustom }: { onAddCustom: () => void }) => (
  <div className="text-center py-16 bg-white dark:bg-ink-800 rounded-2xl border border-dashed border-paper-200 dark:border-ink-700">
    <p className="font-display text-2xl text-ink-800 dark:text-paper-100 mb-1">
      Your library is waiting
    </p>
    <p className="text-sm text-ink-700/70 dark:text-paper-200/70 mb-5">
      Add the books and audiobooks you're reading to start tracking progress.
    </p>
    <div className="flex items-center justify-center gap-2">
      <Link
        to="/catalog"
        className="text-sm bg-plum-600 hover:bg-plum-700 text-white rounded-lg px-4 py-2 transition-colors"
      >
        Browse catalog
      </Link>
      <button
        onClick={onAddCustom}
        className="text-sm border border-paper-200 dark:border-ink-700 hover:bg-paper-100 dark:hover:bg-ink-900 text-ink-700 dark:text-paper-200 rounded-lg px-4 py-2 transition-colors cursor-pointer"
      >
        Add custom book
      </button>
    </div>
  </div>
);

const Library = ({ loaderData }: { loaderData: LoaderData }) => {
  const { books, trackingsMap } = loaderData;
  const revalidator = useRevalidator();
  const [showAddModal, setShowAddModal] = useState(false);
  const [view, setView] = useState<ViewFilter>("all");

  const stats = useMemo(() => {
    let reading = 0;
    let finished = 0;
    let notStarted = 0;
    for (const b of books) {
      const t = trackingsMap[b.id];
      if (!t) notStarted++;
      else if (t.isFinished) finished++;
      else reading++;
    }
    return { total: books.length, reading, finished, notStarted };
  }, [books, trackingsMap]);

  const visibleBooks = useMemo(() => {
    if (view === "all") return books;
    return books.filter((b) => {
      const t = trackingsMap[b.id];
      if (view === "not-started") return !t;
      if (view === "reading") return t && !t.isFinished;
      return t?.isFinished;
    });
  }, [books, trackingsMap, view]);

  const handleBookAdded = () => {
    setShowAddModal(false);
    revalidator.revalidate();
  };

  return (
    <div>
      <header className="mb-8 flex flex-col sm:flex-row sm:items-end sm:justify-between gap-4">
        <div>
          <h1 className="font-display text-3xl font-semibold text-ink-800 dark:text-paper-100">
            My library
          </h1>
          <p className="text-sm text-ink-700/70 dark:text-paper-200/70 mt-1">
            {stats.total === 0
              ? "Nothing here yet."
              : `${stats.total} title${stats.total === 1 ? "" : "s"} · ${stats.reading} in progress · ${stats.finished} finished`}
          </p>
        </div>
        <div className="flex gap-2">
          <button
            onClick={() => setShowAddModal(true)}
            className="text-sm border border-paper-200 dark:border-ink-700 hover:bg-paper-100 dark:hover:bg-ink-800 text-ink-700 dark:text-paper-200 rounded-lg px-4 py-2 transition-colors cursor-pointer"
          >
            + Custom book
          </button>
          <Link
            to="/catalog"
            className="text-sm bg-plum-600 hover:bg-plum-700 text-white rounded-lg px-4 py-2 transition-colors"
          >
            Browse catalog
          </Link>
        </div>
      </header>

      {books.length === 0 ? (
        <EmptyLibrary onAddCustom={() => setShowAddModal(true)} />
      ) : (
        <>
          <div className="flex flex-wrap gap-1 p-1 rounded-full bg-white dark:bg-ink-800 border border-paper-200 dark:border-ink-700 w-fit mb-5">
            {(Object.keys(VIEW_LABELS) as ViewFilter[]).map((v) => {
              const count =
                v === "all"
                  ? stats.total
                  : v === "reading"
                    ? stats.reading
                    : v === "finished"
                      ? stats.finished
                      : stats.notStarted;
              return (
                <button
                  key={v}
                  onClick={() => setView(v)}
                  className={`text-sm px-3.5 py-1.5 rounded-full transition-colors cursor-pointer ${
                    view === v
                      ? "bg-plum-600 text-white"
                      : "text-ink-700 dark:text-paper-200 hover:bg-paper-100 dark:hover:bg-ink-900"
                  }`}
                >
                  {VIEW_LABELS[v]}
                  <span
                    className={`ml-1.5 text-xs ${view === v ? "text-white/80" : "text-ink-700/50 dark:text-paper-200/50"}`}
                  >
                    {count}
                  </span>
                </button>
              );
            })}
          </div>

          {visibleBooks.length === 0 ? (
            <p className="text-center py-12 text-ink-700/60 dark:text-paper-200/60 text-sm">
              Nothing in this section.
            </p>
          ) : (
            <div className="grid gap-3">
              {visibleBooks.map((book) => (
                <LibraryBookCard
                  key={book.id}
                  book={book}
                  tracking={trackingsMap[book.id]}
                />
              ))}
            </div>
          )}
        </>
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
