import Link from "next/link";
import { redirect } from "next/navigation";
import {
  ChevronDown,
  ListFilter,
  Search,
  User,
} from "lucide-react";

import { getCurrentUser, getToken } from "@/lib/auth";
import { getServerApiBase } from "@/lib/api-base";

type Report = {
  id: number;
  name: string;
  description: string | null;
  type: string | null;
};

async function getRecentReports(token: string): Promise<Report[]> {
  try {
    const apiBase = getServerApiBase();
    if (!apiBase) return [];
    const res = await fetch(`${apiBase}/api/reports?pageSize=8`, {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    });
    if (!res.ok) return [];
    const data = (await res.json()) as { reports?: Report[] };
    return data.reports ?? [];
  } catch {
    return [];
  }
}

export default async function HomePage() {
  const user = await getCurrentUser();
  if (!user) redirect("/auth/login");

  const token = (await getToken()) ?? "";
  const recentReports = token ? await getRecentReports(token) : [];
  const displayName = user.fullname || user.username || "Guest";

  return (
    <div className="flex min-h-screen flex-col bg-[#f5f6f7] text-[#2f3b46]">
      <header className="border-b border-[#e3e6e8] bg-[#f7f8f9]">
        <div className="mx-auto flex h-9 w-full max-w-[1240px] items-center gap-2 px-4 text-[11px]">
          <Link href="/" className="flex items-center gap-2 text-[#1f5f7a]">
            <span className="text-[13px] font-semibold">◌</span>
            <span className="font-semibold tracking-[0.01em]">/ library</span>
          </Link>

          <div className="relative mx-2 hidden flex-1 md:block">
            <Search className="absolute left-2 top-1/2 h-3 w-3 -translate-y-1/2 text-[#a1aab3]" />
            <input
              aria-label="Search the library"
              className="h-6 w-full rounded-sm border border-[#dfe3e7] bg-white pl-6 pr-2 text-[11px] outline-none"
            />
          </div>

          <div className="ml-auto flex items-center gap-2 text-[#707b85]">
            <button className="inline-flex items-center gap-0.5 hover:text-[#2f3b46]">
              <ListFilter className="h-3 w-3" />
              <ChevronDown className="h-2.5 w-2.5" />
            </button>
            <button className="inline-flex items-center gap-0.5 hover:text-[#2f3b46]">
              <User className="h-3 w-3" />
              <ChevronDown className="h-2.5 w-2.5" />
            </button>
          </div>
        </div>
      </header>

      <main className="mx-auto w-full max-w-[1240px] px-4 py-4">
        <h1 className="font-serif text-[46px] leading-[1.05] font-semibold text-[#2f3b46]">
          Hi, {displayName}!
        </h1>
        <nav className="mt-1 flex flex-wrap items-center gap-2 text-[11px] text-[#355e88]">
          <a href="#stars" className="hover:underline">
            Stars
          </a>
          <span className="text-[#98a2ac]">/</span>
          <a href="#subscriptions" className="hover:underline">
            Subscriptions
          </a>
          <span className="text-[#98a2ac]">/</span>
          <a href="#report-runs" className="hover:underline">
            Report Runs
          </a>
          <span className="text-[#98a2ac]">/</span>
          <a href="#groups" className="hover:underline">
            Groups
          </a>
        </nav>

        <p className="mt-3 text-[12px] text-[#646f79]">
          {recentReports.length === 0
            ? "You don't have any favorites! Search to get started."
            : `You have ${recentReports.length} recent report${
                recentReports.length === 1 ? "" : "s"
              } in Stars.`}
        </p>

        <div className="min-h-[420px]" />
      </main>

      <footer className="mt-auto border-t border-[#e2e5e8] bg-[#eaeced]">
        <div className="mx-auto flex w-full max-w-[1240px] items-start justify-between gap-8 px-4 py-7">
          <div>
            <div className="mb-3 flex items-center gap-2 text-[#1f5f7a]">
              <span className="text-[13px] font-semibold">◌</span>
              <span className="text-[24px] font-semibold leading-none">/ library</span>
            </div>
            <p className="text-[11px] text-[#55606a]">
              Atlas was created by the Riverside Healthcare Analytics team.
            </p>
            <p className="mt-2 text-[11px] text-[#55606a]">
              © 2026 My Organization Name | Release 3.15.2-alpha.1
            </p>
          </div>

          <div className="pt-2 text-[11px] text-[#4d5965]">
            <p className="font-semibold">Group One</p>
            <p className="mt-1 text-[#2e6c8d]">Something</p>
            <p className="text-[#2e6c8d]">Something Else</p>
          </div>
        </div>
      </footer>
    </div>
  );
}
