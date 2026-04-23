import type { ReactNode } from "react";

type Props = {
  title: string;
  subtitle?: string;
  children: ReactNode;
  footer?: ReactNode;
};

export const AuthShell = ({ title, subtitle, children, footer }: Props) => (
  <div className="min-h-screen flex flex-col items-center justify-center bg-paper-50 dark:bg-ink-900 p-6">
    <div className="w-full max-w-md">
      <div className="flex items-center gap-2 justify-center mb-8">
        <span className="inline-block w-9 h-9 rounded-lg bg-gradient-to-br from-plum-600 to-amber-accent" />
        <span className="font-display text-2xl font-semibold text-ink-800 dark:text-paper-100">
          Storygame
        </span>
      </div>
      <div className="bg-white dark:bg-ink-800 rounded-2xl border border-paper-200 dark:border-ink-700 p-8 shadow-sm">
        <h1 className="font-display text-2xl font-semibold text-ink-800 dark:text-paper-100 mb-1">
          {title}
        </h1>
        {subtitle && (
          <p className="text-sm text-ink-700/70 dark:text-paper-200/70 mb-6">
            {subtitle}
          </p>
        )}
        {children}
      </div>
      {footer && <div className="mt-5 text-center text-sm">{footer}</div>}
    </div>
  </div>
);

export const inputClass =
  "w-full text-sm rounded-lg border border-paper-200 dark:border-ink-700 bg-paper-50 dark:bg-ink-900 text-ink-800 dark:text-paper-100 px-3.5 py-2.5 placeholder-ink-700/40 dark:placeholder-paper-200/40 focus:outline-none focus:ring-2 focus:ring-plum-500 focus:border-transparent transition-shadow";

export const primaryBtnClass =
  "w-full bg-plum-600 hover:bg-plum-700 disabled:opacity-50 disabled:cursor-not-allowed text-white rounded-lg px-4 py-2.5 text-sm font-medium transition-colors cursor-pointer";

export const labelClass =
  "block text-xs font-medium uppercase tracking-wide text-ink-700/80 dark:text-paper-200/70 mb-1.5";
