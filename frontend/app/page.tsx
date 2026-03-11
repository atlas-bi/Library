import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import Link from "next/link";

interface Report {
  id: number;
  name: string;
  description: string | null;
  type: string | null;
  url: string | null;
  lastModified: string | null;
}

interface User {
  username: string;
  fullname: string;
  userId: string;
}

async function getUser(token: string): Promise<User | null> {
  try {
    const res = await fetch(`${process.env.API_URL}/api/auth/me`, {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    });
    if (!res.ok) return null;
    return res.json();
  } catch {
    return null;
  }
}

async function getReports(token: string): Promise<Report[]> {
  try {
    const res = await fetch(`${process.env.API_URL}/api/reports?pageSize=20`, {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    });
    if (!res.ok) return [];
    const data = await res.json();
    return data.reports ?? [];
  } catch {
    return [];
  }
}

export default async function HomePage() {
  const cookieStore = await cookies();
  const token = cookieStore.get("atlas_token")?.value;

  if (!token) redirect("/auth/login");

  const [user, reports] = await Promise.all([getUser(token), getReports(token)]);

  if (!user) redirect("/auth/login");

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Nav */}
      <header className="bg-white border-b border-gray-200">
        <div className="max-w-6xl mx-auto px-6 h-14 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <span className="font-semibold text-gray-900">Atlas Library</span>
            <span className="text-gray-300">·</span>
            <span className="text-sm text-blue-600">Next.js Demo</span>
          </div>
          <div className="flex items-center gap-4">
            <span className="text-sm text-gray-500">
              Hi, <span className="font-medium text-gray-800">{user.fullname}</span>
            </span>
            <Link
              href="/auth/logout"
              className="text-sm text-gray-500 hover:text-gray-800 transition-colors"
            >
              Sign out
            </Link>
          </div>
        </div>
      </header>

      {/* Content */}
      <main className="max-w-6xl mx-auto px-6 py-10">
        <div className="mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Reports</h1>
          <p className="text-sm text-gray-500 mt-1">
            Fetched live from the C# API —{" "}
            <code className="bg-gray-100 px-1.5 py-0.5 rounded text-xs">
              {process.env.API_URL}/api/reports
            </code>
          </p>
        </div>

        {reports.length === 0 ? (
          <div className="bg-white rounded-xl border border-gray-200 p-12 text-center text-gray-400">
            No reports found.
          </div>
        ) : (
          <div className="grid gap-4">
            {reports.map((report) => (
              <div
                key={report.id}
                className="bg-white rounded-xl border border-gray-200 p-5 hover:border-blue-200 hover:shadow-sm transition-all"
              >
                <div className="flex items-start justify-between gap-4">
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 mb-1">
                      <h2 className="font-semibold text-gray-900 truncate">{report.name}</h2>
                      {report.type && (
                        <span className="shrink-0 text-xs font-medium bg-blue-50 text-blue-700 px-2 py-0.5 rounded-full">
                          {report.type}
                        </span>
                      )}
                    </div>
                    {report.description && (
                      <p className="text-sm text-gray-500 line-clamp-2">{report.description}</p>
                    )}
                  </div>
                  {report.lastModified && (
                    <span className="shrink-0 text-xs text-gray-400">
                      {new Date(report.lastModified).toLocaleDateString()}
                    </span>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </main>
    </div>
  );
}
