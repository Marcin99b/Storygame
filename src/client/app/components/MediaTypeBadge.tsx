import type { MediaType } from "../api/types";

type Props = {
  type: MediaType;
};

const BADGE_COLORS: Record<MediaType, string> = {
  Ebook: "bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-300",
  Paperback: "bg-green-100 text-green-700 dark:bg-green-900/40 dark:text-green-300",
  Audiobook: "bg-purple-100 text-purple-700 dark:bg-purple-900/40 dark:text-purple-300",
};

export const MediaTypeBadge = ({ type }: Props) => (
  <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${BADGE_COLORS[type]}`}>
    {type}
  </span>
);
