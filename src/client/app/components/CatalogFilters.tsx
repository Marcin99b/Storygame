export type TypeFilter = "all" | "text" | "audio";

const TYPE_FILTER_LABELS: Record<TypeFilter, string> = {
  all: "All",
  text: "Text",
  audio: "Audio",
};

type Props = {
  titleFilter: string;
  typeFilter: TypeFilter;
  onTitleChange: (value: string) => void;
  onTypeChange: (value: TypeFilter) => void;
};

export const CatalogFilters = ({
  titleFilter,
  typeFilter,
  onTitleChange,
  onTypeChange,
}: Props) => (
  <div className="flex flex-col sm:flex-row gap-3 mb-6">
    <div className="relative flex-1">
      <span className="absolute left-3 top-1/2 -translate-y-1/2 text-ink-700/40 dark:text-paper-200/40 text-sm">
        🔍
      </span>
      <input
        type="text"
        placeholder="Search the catalog by title..."
        value={titleFilter}
        onChange={(e) => onTitleChange(e.target.value)}
        className="w-full text-sm rounded-full border border-paper-200 dark:border-ink-700 bg-white dark:bg-ink-800 text-ink-800 dark:text-paper-100 pl-9 pr-4 py-2.5 placeholder-ink-700/40 dark:placeholder-paper-200/40 focus:outline-none focus:ring-2 focus:ring-plum-500 focus:border-transparent"
      />
    </div>
    <div
      role="tablist"
      aria-label="Filter by format"
      className="flex rounded-full border border-paper-200 dark:border-ink-700 overflow-hidden text-sm bg-white dark:bg-ink-800 p-1 gap-1"
    >
      {(Object.keys(TYPE_FILTER_LABELS) as TypeFilter[]).map((f) => (
        <button
          key={f}
          role="tab"
          aria-selected={typeFilter === f}
          onClick={() => onTypeChange(f)}
          className={`px-4 py-1.5 rounded-full transition-colors cursor-pointer ${
            typeFilter === f
              ? "bg-plum-600 text-white"
              : "text-ink-700 dark:text-paper-200 hover:bg-paper-100 dark:hover:bg-ink-700"
          }`}
        >
          {TYPE_FILTER_LABELS[f]}
        </button>
      ))}
    </div>
  </div>
);
