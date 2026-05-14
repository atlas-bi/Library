import {
  ChartNoAxesColumn,
  ChevronDown,
  Folder,
  ListFilter,
  PlayCircle,
  Search,
  Square,
  Star,
  User,
  Waypoints,
} from "lucide-react"
import Link from "next/link"
import { getServerApiBase } from "@/lib/api-base"
import { getCurrentUser, getToken } from "@/lib/auth"

type Report = {
  id: number
  name: string
  displayTitle?: string | null
  displayName?: string | null
  description: string | null
  type?: string | null
  typeShortName?: string | null
}

async function getRecentReports(token: string): Promise<Report[]> {
  try {
    const apiBase = getServerApiBase()
    if (!apiBase) return []
    const res = await fetch(`${apiBase}/api/reports?pageSize=8`, {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    })
    if (!res.ok) return []
    const data = (await res.json()) as { reports?: Report[] }
    return data.reports ?? []
  } catch {
    return []
  }
}

export default async function HomePage() {
  const user = await getCurrentUser()

  const token = (await getToken()) ?? ""
  const recentReports = token ? await getRecentReports(token) : []
  const displayName = user?.fullname || user?.username || "Guest"

  return (
    <div className="flex min-h-screen flex-col bg-[#f5f6f7] text-[#2f3b46]">
      <header className="border-b border-[#e3e6e8] bg-[#f7f8f9]">
        <div className="mx-auto flex h-9 w-full max-w-[1240px] items-center gap-2 px-4 text-[11px]">
          <Link href="/" className="flex items-center gap-2 text-[#1f5f7a]">
            <span className="text-[13px] font-semibold">◌</span>
            <span className="font-semibold tracking-[0.01em]">/ library</span>
          </Link>

          <form action="/search" method="get" className="relative mx-2 hidden flex-1 md:block">
            <Search className="absolute left-2 top-1/2 h-3 w-3 -translate-y-1/2 text-[#a1aab3]" />
            <input
              name="q"
              aria-label="Search the library"
              placeholder="Search the library"
              className="h-6 w-full rounded-sm border border-[#dfe3e7] bg-white pl-6 pr-2 text-[11px] outline-none"
            />
          </form>

          <div className="ml-auto flex items-center gap-2 text-[#707b85]">
            <button type="button" className="inline-flex items-center gap-0.5 hover:text-[#2f3b46]">
              <ListFilter className="h-3 w-3" />
              <ChevronDown className="h-2.5 w-2.5" />
            </button>
            <button type="button" className="inline-flex items-center gap-0.5 hover:text-[#2f3b46]">
              <User className="h-3 w-3" />
              <ChevronDown className="h-2.5 w-2.5" />
            </button>
            {!user ? (
              <Link href="/auth/login" className="text-[11px] text-[#2e6c8d] hover:underline">
                Sign in
              </Link>
            ) : (
              <Link href="/auth/logout" className="text-[11px] text-[#2e6c8d] hover:underline">
                Sign out
              </Link>
            )}
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

        <section className="mt-4 grid grid-cols-1 gap-4 lg:grid-cols-[170px_1fr]">
          <aside className="space-y-2">
            <div className="text-[12px] font-semibold text-[#333f49]">Quick Filter</div>
            <input
              aria-label="Type to filter reports"
              placeholder="type to filter..."
              className="h-7 w-full rounded-sm border border-[#dfe3e7] bg-white px-2 text-[11px] outline-none"
            />
            {["All", "Unsorted", "Folder"].map((item, index) => (
              <div
                key={item}
                className="rounded-sm border border-[#e4e7ea] bg-white px-3 py-2 text-[12px] text-[#3b4854] shadow-[0_1px_2px_rgba(0,0,0,0.04)]"
              >
                <span className="inline-flex items-center gap-2">
                  <span className="inline-flex h-4 w-4 items-center justify-center rounded-full bg-[#17b29e] text-[10px] text-white">
                    {index === 0 ? "9" : "0"}
                  </span>
                  {item === "All" ? (
                    <ChartNoAxesColumn className="h-3.5 w-3.5 text-[#5e6a75]" />
                  ) : item === "Unsorted" ? (
                    <Square className="h-3.5 w-3.5 text-[#5e6a75]" />
                  ) : (
                    <Folder className="h-3.5 w-3.5 text-[#5e6a75]" />
                  )}
                  {item}
                </span>
              </div>
            ))}
          </aside>

          <div className="space-y-3">
            {!user ? (
              <div className="rounded-sm border border-[#e4e7ea] bg-white p-4 text-[12px] text-[#606b75]">
                Sign in to view report list and open report details.
              </div>
            ) : recentReports.length === 0 ? (
              <div className="rounded-sm border border-[#e4e7ea] bg-white p-4 text-[12px] text-[#606b75]">
                No reports found in your starred list.
              </div>
            ) : (
              recentReports.map((report) => (
                <article
                  key={report.id}
                  className="rounded-sm border border-[#e4e7ea] bg-white shadow-[0_1px_2px_rgba(0,0,0,0.04)]"
                >
                  <div className="flex items-center justify-between gap-3 border-b border-[#eef0f2] px-3 py-2">
                    <div className="inline-flex items-center gap-2">
                      <PlayCircle className="h-4.5 w-4.5 text-[#28b99f]" />
                      <Link
                        href={`/reports?id=${report.id}`}
                        className="text-[12px] font-semibold text-[#2f3b46] hover:text-[#2e6c8d] hover:underline"
                      >
                        {report.displayTitle || report.displayName || report.name}
                      </Link>
                    </div>
                    <span className="text-[10px] text-[#7e8994]">
                      {report.typeShortName || report.type || "Report"}
                    </span>
                  </div>
                  <div className="grid grid-cols-[70px_1fr] gap-4 px-3 py-3">
                    <div className="relative flex h-14 items-end gap-1 self-center opacity-45">
                      <span className="h-3 w-2 rounded-[1px] bg-[#b7c0c9]" />
                      <span className="h-5 w-2 rounded-[1px] bg-[#b7c0c9]" />
                      <span className="h-7 w-2 rounded-[1px] bg-[#b7c0c9]" />
                      <span className="h-4 w-2 rounded-[1px] bg-[#b7c0c9]" />
                      <span className="h-9 w-2 rounded-[1px] bg-[#b7c0c9]" />
                      <svg
                        viewBox="0 0 60 26"
                        aria-hidden="true"
                        className="pointer-events-none absolute left-0 top-1 h-8 w-[58px]"
                      >
                        <polyline
                          points="0,20 10,18 20,8 29,11 39,2 49,6 59,1"
                          fill="none"
                          stroke="#b7c0c9"
                          strokeWidth="2"
                          strokeLinecap="round"
                          strokeLinejoin="round"
                        />
                      </svg>
                    </div>
                    <p className="text-[12px] text-[#596571]">
                      {report.description
                        ? `${report.description.slice(0, 180)}${
                            report.description.length > 180 ? "..." : ""
                          }`
                        : "No description available."}
                    </p>
                  </div>
                  <div className="grid grid-cols-[1fr_1fr] border-t border-[#eef0f2] text-[11px] text-[#7b8691]">
                    <div className="inline-flex items-center gap-1 px-3 py-2">
                      <Star className="h-3 w-3 text-[#e7c130]" />
                      <span className="text-[#3d5f90]">Starred</span>
                      <span>1</span>
                    </div>
                    <div className="inline-flex items-center justify-end gap-2 border-l border-[#eef0f2] px-3 py-2">
                      <Waypoints className="h-3 w-3" />
                      <Link
                        href={`/reports?id=${report.id}`}
                        className="text-[#2e6c8d] hover:underline"
                      >
                        Details
                      </Link>
                    </div>
                  </div>
                </article>
              ))
            )}
          </div>
        </section>
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
  )
}
