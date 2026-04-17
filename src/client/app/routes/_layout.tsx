import { NavLink, Outlet, redirect, useNavigate } from "react-router";
import type { Route } from "./+types/_layout";
import { usersApi } from "../api/users";
import { ApiError } from "../api/client";

export async function clientLoader() {
  try {
    await usersApi.me();
  } catch (e) {
    if (e instanceof ApiError && e.status === 401) {
      return redirect("/login");
    }
    throw e;
  }
  return null;
}

export default function Layout() {
  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-950">
      <nav className="bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-800">
        <div className="max-w-4xl mx-auto px-4 h-14 flex items-center justify-between">
          <div className="flex items-center gap-6">
            <span className="font-semibold text-gray-900 dark:text-white">Storygame</span>
            <NavLink
              to="/"
              end
              className={({ isActive }) =>
                `text-sm ${isActive ? "text-blue-600 dark:text-blue-400 font-medium" : "text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white"}`
              }
            >
              Library
            </NavLink>
            <NavLink
              to="/catalog"
              className={({ isActive }) =>
                `text-sm ${isActive ? "text-blue-600 dark:text-blue-400 font-medium" : "text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white"}`
              }
            >
              Catalog
            </NavLink>
          </div>
          <LogoutButton />
        </div>
      </nav>
      <main className="max-w-4xl mx-auto px-4 py-8">
        <Outlet />
      </main>
    </div>
  );
}

function LogoutButton() {
  const navigate = useNavigate();

  async function handleLogout() {
    await usersApi.logout();
    navigate("/login");
  }

  return (
    <button
      onClick={handleLogout}
      className="text-sm text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors cursor-pointer"
    >
      Sign out
    </button>
  );
}
