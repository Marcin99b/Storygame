import { useRevalidator } from "react-router";
import { useState, useEffect, useCallback } from "react";
import { trackingApi } from "../api/tracking";
import type { LibraryBook, TimePeriod, Tracking, TrackingStatistic } from "../api/types";
import { MediaTypeBadge } from "./MediaTypeBadge";

type Props = {
  book: LibraryBook;
  tracking?: Tracking;
};

const ProgressBar = ({
  current,
  total,
}: {
  current: number;
  total: number;
}) => {
  const pct = total > 0 ? Math.min(100, Math.round((current / total) * 100)) : 0;
  return (
    <div
      className="w-full bg-paper-100 dark:bg-ink-900 rounded-full h-2 mt-3 overflow-hidden"
      role="progressbar"
      aria-valuenow={pct}
      aria-valuemin={0}
      aria-valuemax={100}
    >
      <div
        className="h-full rounded-full bg-gradient-to-r from-plum-600 to-amber-accent transition-all"
        style={{ width: `${pct}%` }}
      />
    </div>
  );
};

const UpdateProgressForm = ({
  tracking,
  unit,
  onUpdated,
  onCancel,
}: {
  tracking: Tracking;
  unit: string;
  onUpdated: () => void;
  onCancel: () => void;
}) => {
  const [value, setValue] = useState(String(tracking.currentIndex));
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const parsed = parseInt(value, 10);
    if (isNaN(parsed) || parsed < 0 || parsed > tracking.totalLength) {
      setError(`Enter a value between 0 and ${tracking.totalLength}.`);
      return;
    }
    setLoading(true);
    setError(null);
    try {
      await trackingApi.updateIndex(tracking.id, parsed);
      onUpdated();
    } catch {
      setError("Failed to update.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-wrap items-center gap-2 mt-3">
      <input
        type="number"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        min={0}
        max={tracking.totalLength}
        autoFocus
        className="w-28 text-sm rounded-lg border border-paper-200 dark:border-ink-700 bg-paper-50 dark:bg-ink-900 text-ink-800 dark:text-paper-100 px-3 py-1.5 focus:outline-none focus:ring-2 focus:ring-plum-500"
      />
      <span className="text-xs text-ink-700/60 dark:text-paper-200/60">
        of {tracking.totalLength} {unit}
      </span>
      <button
        type="submit"
        disabled={loading}
        className="text-xs bg-plum-600 hover:bg-plum-700 text-white rounded-md px-3 py-1.5 disabled:opacity-50 cursor-pointer"
      >
        {loading ? "Saving..." : "Save"}
      </button>
      <button
        type="button"
        onClick={onCancel}
        className="text-xs text-ink-700/70 dark:text-paper-200/70 hover:text-ink-800 dark:hover:text-paper-100 cursor-pointer"
      >
        Cancel
      </button>
      {error && <span className="text-xs text-red-600 basis-full">{error}</span>}
    </form>
  );
};

const PERIODS: TimePeriod[] = ["Day", "Week", "Month", "Year"];

const PERIOD_RANGE_DAYS: Record<TimePeriod, number> = {
  Day: 14,
  Week: 56,
  Month: 180,
  Year: 1825,
};

function formatBarLabel(from: string, period: TimePeriod): string {
  const d = new Date(from);
  if (period === "Day")
    return d.toLocaleDateString(undefined, { month: "short", day: "numeric" });
  if (period === "Week") {
    const jan1 = new Date(d.getFullYear(), 0, 1);
    const week = Math.ceil(((d.getTime() - jan1.getTime()) / 86400000 + jan1.getDay() + 1) / 7);
    return `W${week}`;
  }
  if (period === "Month")
    return d.toLocaleDateString(undefined, { month: "short" });
  return String(d.getFullYear());
}

const TrackingStatsPanel = ({
  trackingId,
  unit,
}: {
  trackingId: string;
  unit: string;
}) => {
  const [period, setPeriod] = useState<TimePeriod>("Day");
  const [stats, setStats] = useState<TrackingStatistic[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchStats = useCallback(
    async (p: TimePeriod) => {
      setLoading(true);
      setError(null);
      const to = new Date();
      const from = new Date(to);
      from.setDate(from.getDate() - PERIOD_RANGE_DAYS[p]);
      try {
        const result = await trackingApi.getStatistics(
          trackingId,
          p,
          from.toISOString(),
          to.toISOString()
        );
        setStats(result.trackingStatistics);
      } catch {
        setError("Failed to load statistics.");
      } finally {
        setLoading(false);
      }
    },
    [trackingId]
  );

  useEffect(() => {
    fetchStats(period);
  }, [period, fetchStats]);

  const maxValue = stats && stats.length > 0 ? Math.max(1, ...stats.map((s) => s.value)) : 1;

  return (
    <div className="mt-3 pt-3 border-t border-paper-200 dark:border-ink-700">
      <div className="flex items-center justify-between mb-3">
        <span className="text-xs font-medium text-ink-700/60 dark:text-paper-200/60">
          Reading activity
        </span>
        <div className="flex gap-0.5">
          {PERIODS.map((p) => (
            <button
              key={p}
              onClick={() => setPeriod(p)}
              className={`text-xs px-2 py-0.5 rounded cursor-pointer transition-colors ${
                period === p
                  ? "bg-plum-600 text-white"
                  : "text-ink-700/50 dark:text-paper-200/50 hover:text-ink-800 dark:hover:text-paper-100"
              }`}
            >
              {p}
            </button>
          ))}
        </div>
      </div>

      {loading && (
        <div className="h-20 flex items-center justify-center text-xs text-ink-700/40 dark:text-paper-200/40">
          Loading…
        </div>
      )}

      {error && <p className="text-xs text-red-600">{error}</p>}

      {!loading && stats && (
        stats.length === 0 ? (
          <p className="text-xs text-ink-700/40 dark:text-paper-200/40 text-center py-4">
            No activity in this period
          </p>
        ) : (
          <div className="overflow-x-auto -mx-1 px-1">
            <div
              className="flex items-end gap-px"
              style={{ minWidth: `${stats.length * 24}px` }}
            >
              {stats.map((s, i) => {
                const heightPct = Math.max(4, Math.round((s.value / maxValue) * 100));
                return (
                  <div key={i} className="flex-1 flex flex-col items-center gap-1 min-w-0">
                    <div className="relative w-full" style={{ height: "56px" }}>
                      <div
                        className="absolute bottom-0 w-full rounded-sm bg-gradient-to-t from-plum-600 to-amber-accent opacity-80"
                        style={{ height: `${heightPct}%` }}
                        title={`${s.value} ${unit}`}
                      />
                    </div>
                    <span className="text-[9px] leading-tight text-ink-700/40 dark:text-paper-200/40 truncate w-full text-center">
                      {formatBarLabel(s.from, period)}
                    </span>
                  </div>
                );
              })}
            </div>
          </div>
        )
      )}
    </div>
  );
};

export const LibraryBookCard = ({ book, tracking }: Props) => {
  const revalidator = useRevalidator();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [editingProgress, setEditingProgress] = useState(false);
  const [showStats, setShowStats] = useState(false);

  const unit = book.lengthUnit.toLowerCase();

  const handleStartTracking = async () => {
    setLoading(true);
    setError(null);
    try {
      await trackingApi.startTracking(book.id);
      revalidator.revalidate();
    } catch {
      setError("Failed to start tracking.");
    } finally {
      setLoading(false);
    }
  };

  const addedDate = new Date(book.addedToLibraryAt).toLocaleDateString(
    undefined,
    { month: "short", day: "numeric", year: "numeric" }
  );

  const pct = tracking
    ? Math.round((tracking.currentIndex / Math.max(1, tracking.totalLength)) * 100)
    : 0;

  return (
    <article className="bg-white dark:bg-ink-800 rounded-xl border border-paper-200 dark:border-ink-700 p-5">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2 mb-1">
            <h2 className="font-display text-lg font-semibold text-ink-800 dark:text-paper-100 truncate">
              {book.title}
            </h2>
            <MediaTypeBadge type={book.mediaType} />
            {tracking?.isFinished && (
              <span className="inline-flex items-center gap-1 text-xs px-2 py-0.5 rounded-full bg-emerald-500/10 text-emerald-700 dark:text-emerald-400 ring-1 ring-inset ring-emerald-500/20 font-medium">
                ✓ Finished
              </span>
            )}
          </div>
          <p className="text-sm text-ink-700/70 dark:text-paper-200/70 line-clamp-2">
            {book.description}
          </p>
          <div className="flex flex-wrap items-center gap-x-3 gap-y-1 mt-2 text-xs text-ink-700/60 dark:text-paper-200/60">
            <span>
              {book.length} {unit}
            </span>
            <span aria-hidden>·</span>
            <span>Added {addedDate}</span>
          </div>
          {error && <p className="text-xs text-red-600 mt-1">{error}</p>}
        </div>
        {!tracking && (
          <button
            onClick={handleStartTracking}
            disabled={loading}
            className="shrink-0 text-sm rounded-full px-3.5 py-1.5 border border-plum-600 text-plum-600 dark:text-plum-500 hover:bg-plum-600 hover:text-white disabled:opacity-50 transition-colors cursor-pointer"
          >
            {loading ? "Starting..." : "Start tracking"}
          </button>
        )}
      </div>

      {tracking && !tracking.isFinished && (
        <div className="mt-2">
          {editingProgress ? (
            <UpdateProgressForm
              tracking={tracking}
              unit={unit}
              onUpdated={() => {
                setEditingProgress(false);
                revalidator.revalidate();
              }}
              onCancel={() => setEditingProgress(false)}
            />
          ) : (
            <button
              type="button"
              onClick={() => setEditingProgress(true)}
              className="group w-full text-left cursor-pointer"
            >
              <div className="flex items-center justify-between text-xs text-ink-700/70 dark:text-paper-200/70">
                <span>
                  <span className="font-medium text-ink-800 dark:text-paper-100">
                    {tracking.currentIndex}
                  </span>{" "}
                  / {tracking.totalLength} {unit} · {pct}%
                </span>
                <span className="text-plum-600 dark:text-plum-500 opacity-0 group-hover:opacity-100 transition-opacity">
                  Update progress →
                </span>
              </div>
              <ProgressBar
                current={tracking.currentIndex}
                total={tracking.totalLength}
              />
            </button>
          )}
        </div>
      )}

      {tracking?.isFinished && (
        <ProgressBar current={tracking.totalLength} total={tracking.totalLength} />
      )}

      {tracking?.isStarted && (
        <div className="mt-2 flex justify-end">
          <button
            type="button"
            onClick={() => setShowStats((s) => !s)}
            className="text-xs text-ink-700/50 dark:text-paper-200/50 hover:text-plum-600 dark:hover:text-plum-500 transition-colors cursor-pointer"
          >
            {showStats ? "Hide stats ↑" : "Stats ↓"}
          </button>
        </div>
      )}

      {tracking?.isStarted && showStats && (
        <TrackingStatsPanel trackingId={tracking.id} unit={unit} />
      )}
    </article>
  );
};
