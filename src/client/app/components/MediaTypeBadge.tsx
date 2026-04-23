import type { MediaType } from "../api/types";

type Props = {
  type: MediaType;
};

const BADGE_STYLES: Record<MediaType, string> = {
  Ebook:
    "bg-plum-500/10 text-plum-700 dark:bg-plum-500/20 dark:text-plum-500 ring-plum-500/20",
  Paperback:
    "bg-amber-accent/10 text-amber-accent dark:bg-amber-accent/20 ring-amber-accent/20",
  Audiobook:
    "bg-emerald-500/10 text-emerald-700 dark:bg-emerald-500/20 dark:text-emerald-400 ring-emerald-500/20",
};

const ICONS: Record<MediaType, string> = {
  Ebook: "📖",
  Paperback: "📚",
  Audiobook: "🎧",
};

export const MediaTypeBadge = ({ type }: Props) => (
  <span
    className={`inline-flex items-center gap-1 text-xs px-2 py-0.5 rounded-full font-medium ring-1 ring-inset ${BADGE_STYLES[type]}`}
  >
    <span aria-hidden>{ICONS[type]}</span>
    {type}
  </span>
);
