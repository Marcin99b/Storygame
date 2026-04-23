import { NavLink, useNavigate } from "react-router";
import { usersApi } from "../api/users";

type NavbarProps = {
  userName?: string;
};

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  `px-3 py-1.5 rounded-full text-sm transition-colors ${
    isActive
      ? "bg-plum-600 text-white"
      : "text-ink-700 dark:text-paper-200 hover:bg-paper-100 dark:hover:bg-ink-800"
  }`;

const LogoutButton = () => {
  const navigate = useNavigate();
  const handleLogout = async () => {
    try {
      await usersApi.logout();
    } finally {
      navigate("/login");
    }
  };
  return (
    <button
      onClick={handleLogout}
      className="text-sm text-ink-700 dark:text-paper-200 hover:text-plum-600 dark:hover:text-plum-500 transition-colors cursor-pointer"
    >
      Sign out
    </button>
  );
};

export const Navbar = ({ userName }: NavbarProps) => (
  <nav className="bg-paper-50/80 dark:bg-ink-900/80 backdrop-blur border-b border-paper-200 dark:border-ink-800 sticky top-0 z-10">
    <div className="max-w-5xl mx-auto px-4 h-16 flex items-center justify-between gap-4">
      <div className="flex items-center gap-5 min-w-0">
        <NavLink to="/" className="flex items-center gap-2 shrink-0">
          <span className="inline-block w-8 h-8 rounded-lg bg-gradient-to-br from-plum-600 to-amber-accent" />
          <span className="font-display text-xl font-semibold text-ink-800 dark:text-paper-100">
            Storygame
          </span>
        </NavLink>
        <div className="flex items-center gap-1 ml-2">
          <NavLink to="/" end className={navLinkClass}>
            Library
          </NavLink>
          <NavLink to="/catalog" className={navLinkClass}>
            Catalog
          </NavLink>
        </div>
      </div>
      <div className="flex items-center gap-4">
        {userName && (
          <span className="hidden sm:inline text-sm text-ink-700 dark:text-paper-200">
            Hello, <span className="font-medium">{userName}</span>
          </span>
        )}
        <LogoutButton />
      </div>
    </div>
  </nav>
);
