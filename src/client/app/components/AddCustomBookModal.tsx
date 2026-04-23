import { useEffect, useState } from "react";
import { libraryApi } from "../api/library";
import type { MediaType } from "../api/types";
import { inputClass, labelClass } from "./AuthShell";

type Props = {
  onClose: () => void;
  onAdded: () => void;
};

const FORMATS: { value: MediaType; icon: string; label: string }[] = [
  { value: "Ebook", icon: "📖", label: "Ebook" },
  { value: "Paperback", icon: "📚", label: "Paperback" },
  { value: "Audiobook", icon: "🎧", label: "Audiobook" },
];

export const AddCustomBookModal = ({ onClose, onAdded }: Props) => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [mediaType, setMediaType] = useState<MediaType>("Ebook");
  const [length, setLength] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [onClose]);

  const isAudiobook = mediaType === "Audiobook";
  const lengthLabel = isAudiobook ? "Length (minutes)" : "Length (pages)";
  const lengthPlaceholder = isAudiobook ? "e.g. 420" : "e.g. 320";

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title.trim() || !length) return;
    setLoading(true);
    setError(null);
    try {
      await libraryApi.addBook({
        title: title.trim(),
        description: description.trim(),
        mediaType,
        length: parseInt(length, 10),
      });
      onAdded();
    } catch {
      setError("Failed to add book. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      className="fixed inset-0 bg-ink-900/60 backdrop-blur-sm flex items-center justify-center z-50 p-4"
      onClick={(e) => e.target === e.currentTarget && onClose()}
      role="dialog"
      aria-modal="true"
      aria-label="Add custom book"
    >
      <div className="bg-white dark:bg-ink-800 rounded-2xl border border-paper-200 dark:border-ink-700 w-full max-w-md p-6 shadow-xl">
        <div className="flex items-center justify-between mb-5">
          <h2 className="font-display text-xl font-semibold text-ink-800 dark:text-paper-100">
            Add a custom book
          </h2>
          <button
            onClick={onClose}
            aria-label="Close"
            className="text-ink-700/60 dark:text-paper-200/60 hover:text-ink-800 dark:hover:text-paper-100 cursor-pointer"
          >
            ✕
          </button>
        </div>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <div>
            <label className={labelClass}>
              Title <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Book title"
              required
              autoFocus
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass}>Description</label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Short description (optional)"
              rows={2}
              className={`${inputClass} resize-none`}
            />
          </div>
          <div>
            <label className={labelClass}>Format</label>
            <div className="grid grid-cols-3 gap-2">
              {FORMATS.map((f) => (
                <button
                  key={f.value}
                  type="button"
                  onClick={() => setMediaType(f.value)}
                  className={`text-sm px-3 py-2 rounded-lg border transition-colors cursor-pointer ${
                    mediaType === f.value
                      ? "bg-plum-600 text-white border-plum-600"
                      : "border-paper-200 dark:border-ink-700 text-ink-700 dark:text-paper-200 hover:border-plum-500"
                  }`}
                >
                  <span className="mr-1">{f.icon}</span>
                  {f.label}
                </button>
              ))}
            </div>
          </div>
          <div>
            <label className={labelClass}>
              {lengthLabel} <span className="text-red-500">*</span>
            </label>
            <input
              type="number"
              value={length}
              onChange={(e) => setLength(e.target.value)}
              placeholder={lengthPlaceholder}
              min={1}
              required
              className={inputClass}
            />
          </div>
          {error && <p className="text-sm text-red-600">{error}</p>}
          <div className="flex justify-end gap-2 mt-1">
            <button
              type="button"
              onClick={onClose}
              className="text-sm px-4 py-2 rounded-lg text-ink-700 dark:text-paper-200 hover:bg-paper-100 dark:hover:bg-ink-900 transition-colors cursor-pointer"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading || !title.trim() || !length}
              className="text-sm px-4 py-2 rounded-lg bg-plum-600 hover:bg-plum-700 disabled:opacity-50 text-white transition-colors cursor-pointer"
            >
              {loading ? "Adding..." : "Add book"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
