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

export const CatalogFilters = ({ titleFilter, typeFilter, onTitleChange, onTypeChange }: Props) => (
  <div className="flex gap-3 mb-6">
    <input
      type="text"
      placeholder="Search by title..."
      value={titleFilter}
      onChange={(e) => onTitleChange(e.target.value)}
      className="flex-1 text-sm rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-900 dark:text-white px-3 py-2 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
    />
    <div className="flex rounded-lg border border-gray-200 dark:border-gray-700 overflow-hidden text-sm">
      {(Object.keys(TYPE_FILTER_LABELS) as TypeFilter[]).map((f) => (
        <button
          key={f}
          onClick={() => onTypeChange(f)}
          className={`px-3 py-2 transition-colors cursor-pointer ${
            typeFilter === f
              ? "bg-blue-600 text-white"
              : "bg-white dark:bg-gray-900 text-gray-600 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-800"
          }`}
        >
          {TYPE_FILTER_LABELS[f]}
        </button>
      ))}
    </div>
  </div>
);
