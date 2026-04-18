import { Outlet, redirect } from "react-router";
import { ApiError } from "../api/client";
import { usersApi } from "../api/users";
import { Navbar } from "../components/Navbar";

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

const Layout = () => (
  <div className="min-h-screen bg-gray-50 dark:bg-gray-950">
    <Navbar />
    <main className="max-w-4xl mx-auto px-4 py-8">
      <Outlet />
    </main>
  </div>
);

export default Layout;
