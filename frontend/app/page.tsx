import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import Link from "next/link";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

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
    <div className="min-h-screen bg-background">
      {/* Nav */}
      <header className="bg-card border-b border-border">
        <div className="max-w-6xl mx-auto px-6 h-14 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <span className="font-semibold text-foreground">Atlas Library</span>
            <span className="text-muted-foreground">·</span>
            <span className="text-sm text-primary">Next.js Demo</span>
          </div>
          <div className="flex items-center gap-4">
            <span className="text-sm text-muted-foreground">
              Hi, <span className="font-medium text-foreground">{user.fullname}</span>
            </span>
            <Button asChild variant="ghost" size="sm">
              <Link href="/auth/logout">Sign out</Link>
            </Button>
          </div>
        </div>
      </header>

      {/* Content */}
      <main className="max-w-6xl mx-auto px-6 py-10">
        <div className="mb-8">
          <h1 className="text-2xl font-bold text-foreground">Reports</h1>
          <p className="text-sm text-muted-foreground mt-1">
            Fetched live from the C# API —{" "}
            <code className="bg-muted px-1.5 py-0.5 rounded text-xs">
              {process.env.API_URL}/api/reports
            </code>
          </p>
        </div>

        {reports.length === 0 ? (
          <Card className="p-12 text-center text-muted-foreground">
            <CardContent className="p-0">No reports found.</CardContent>
          </Card>
        ) : (
          <div className="grid gap-4">
            {reports.map((report) => (
              <Card key={report.id} className="transition-shadow hover:shadow-sm">
                <CardHeader className="pb-0">
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-1">
                        <CardTitle className="truncate">{report.name}</CardTitle>
                        {report.type && <Badge>{report.type}</Badge>}
                      </div>
                      {report.description && (
                        <CardDescription className="line-clamp-2">
                          {report.description}
                        </CardDescription>
                      )}
                    </div>

                    {report.lastModified && (
                      <CardAction className="text-xs text-muted-foreground">
                        {new Date(report.lastModified).toLocaleDateString()}
                      </CardAction>
                    )}
                  </div>
                </CardHeader>
                {report.url && (
                  <CardContent className="pt-0">
                    <Button asChild variant="outline" size="sm">
                      <Link href={report.url} target="_blank" rel="noreferrer">
                        Open report
                      </Link>
                    </Button>
                  </CardContent>
                )}
              </Card>
            ))}
          </div>
        )}
      </main>
    </div>
  );
}
