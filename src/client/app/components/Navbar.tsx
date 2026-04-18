import { NavLink, useNavigate } from "react-router";
import { usersApi } from "../api/users";

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  `text-sm ${isActive ? "text-blue-600 dark:text-blue-400 font-medium" : "text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white"}`;

const LogoutButton = () => {
  const navigate = useNavigate();

  const handleLogout = async () => {
    await usersApi.logout();
    navigate("/login");
  };

  return (
    <button
      onClick={handleLogout}
      className="text-sm text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors cursor-pointer"
    >
      Sign out
    </button>
  );
};

export const Navbar = () => (
  <nav className="bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-800">
    <div className="max-w-4xl mx-auto px-4 h-14 flex items-center justify-between">
      <div className="flex items-center gap-6">
        <span className="font-semibold text-gray-900 dark:text-white">Storygame</span>
        <NavLink to="/" end className={navLinkClass}>
          Library
        </NavLink>
        <NavLink to="/catalog" className={navLinkClass}>
          Catalog
        </NavLink>
      </div>
      <LogoutButton />
    </div>
  </nav>
);
