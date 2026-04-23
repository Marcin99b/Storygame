import { Outlet, redirect, useLoaderData } from "react-router";
import { ApiError } from "../api/client";
import { usersApi } from "../api/users";
import type { UserProfile } from "../api/types";
import { Navbar } from "../components/Navbar";

export async function clientLoader() {
  try {
    const me = await usersApi.me();
    return { me };
  } catch (e) {
    if (e instanceof ApiError && e.status === 401) {
      return redirect("/login");
    }
    throw e;
  }
}

type LoaderData = { me: UserProfile };

const Layout = () => {
  const { me } = useLoaderData() as LoaderData;
  return (
    <div className="min-h-screen bg-paper-50 dark:bg-ink-900">
      <Navbar userName={me.name} />
      <main className="max-w-5xl mx-auto px-4 py-10">
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;
