import Link from "next/link";
import { redirect } from "next/navigation";

import { getToken } from "@/lib/auth";
import { getReportDetailById } from "@/lib/reports/api";
import type { ReportDetail } from "@/lib/reports/types";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

function formatReportTitle(r: ReportDetail) {
  return r.displayTitle || r.displayName || r.name;
}

function getFullName(p?: { fullName?: string | null } | null) {
  return (p?.fullName ?? "").trim();
}

export default async function ReportsPage({
  searchParams,
}: {
  searchParams: { id?: string };
}) {
  const token = await getToken();
  if (!token) redirect("/auth/login");

  const idRaw = searchParams.id;
  const id = idRaw ? Number(idRaw) : NaN;
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
        <h1 className="text-2xl font-bold">Report not found</h1>
        <p className="mt-2 text-sm text-muted-foreground">
          Missing or invalid report id.
        </p>
      </div>
    );
  }

  const report = await getReportDetailById(id);
  if (!report) {
    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
        <h1 className="text-2xl font-bold">Report not found</h1>
        <p className="mt-2 text-sm text-muted-foreground">
          The API returned 401/404 or the report does not exist.
        </p>
      </div>
    );
  }

  const title = formatReportTitle(report);
  const featureFlags = report.features ?? {};

  return (
    <div className="min-h-screen bg-background">
      <header className="border-b bg-card/95 backdrop-blur">
        <div className="mx-auto flex w-full max-w-7xl flex-col gap-2 px-4 py-6">
          <div className="flex items-center justify-between gap-4">
            <div>
              <div className="text-2xl font-bold">{title}</div>
              <div className="mt-1 text-sm text-muted-foreground">
                {report.typeShortName ? `${report.typeShortName}` : null}
                {report.availability ? ` • ${report.availability}` : null}
              </div>
              {Array.isArray(report.headerTags) && report.headerTags.length > 0 ? (
                <div className="mt-3 flex flex-wrap gap-2">
                  {report.headerTags
                    .filter((t) => t.showInHeader === true || t.showInHeader === "Y")
                    .map((t) => (
                      <span
                        key={t.id}
                        className="rounded-md border bg-background px-2 py-1 text-xs text-muted-foreground"
                        title={t.description ?? t.name ?? ""}
                      >
                        {t.name ?? `Tag ${t.id}`}
                      </span>
                    ))}
                </div>
              ) : null}
            </div>

            <div className="flex items-center gap-2">
              {typeof report.starCount === "number" ? (
                <Badge variant={report.isStarred ? "default" : "outline"}>
                  <span className="mr-1">★</span>
                  {report.starCount}
                </Badge>
              ) : null}
            </div>
          </div>

          {report.description || report.detailedDescription ? (
            <p className="max-w-3xl text-sm text-muted-foreground">
              {report.detailedDescription || report.description}
            </p>
          ) : null}
        </div>
      </header>

      <main className="mx-auto grid w-full max-w-7xl gap-6 px-4 py-8 lg:grid-cols-[280px_1fr]">
        <aside className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Actions</CardTitle>
            </CardHeader>
            <CardContent className="space-y-2">
              {report.canRun && report.runUrl ? (
                <Button asChild className="w-full">
                  <a href={report.runUrl} target="_blank" rel="noreferrer">
                    Run report
                  </a>
                </Button>
              ) : (
                <Button variant="secondary" className="w-full" disabled>
                  Run unavailable
                </Button>
              )}

              {report.canEditDocumentation && report.editReportUrl ? (
                <Button asChild variant="outline" className="w-full">
                  <a href={report.editReportUrl} target="_blank" rel="noreferrer">
                    Edit documentation
                  </a>
                </Button>
              ) : null}

              {report.manageReportUrl ? (
                <Button asChild variant="outline" className="w-full">
                  <a href={report.manageReportUrl} target="_blank" rel="noreferrer">
                    Manage report
                  </a>
                </Button>
              ) : null}

              {report.recordViewerUrl ? (
                <Button asChild variant="outline" className="w-full">
                  <a href={report.recordViewerUrl} target="_blank" rel="noreferrer">
                    Record viewer
                  </a>
                </Button>
              ) : null}
            </CardContent>
          </Card>

          {report.maintenanceStatus?.isRequired ? (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Maintenance required</CardTitle>
              </CardHeader>
              <CardContent className="space-y-2 text-sm text-muted-foreground">
                <div>{report.maintenanceStatus.message ?? "Maintenance is required."}</div>
                {report.maintenanceStatus.nextMaintenanceDate ? (
                  <div>
                    Next maintenance:{" "}
                    {new Date(report.maintenanceStatus.nextMaintenanceDate).toLocaleDateString()}
                  </div>
                ) : null}
              </CardContent>
            </Card>
          ) : null}
        </aside>

        <section className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Details</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 text-sm">
              {report.lastModified ? (
                <div className="text-muted-foreground">
                  Last modified:{" "}
                  {new Date(report.lastModified).toLocaleString()}
                </div>
              ) : null}

              {report.author ? (
                <div className="text-muted-foreground">
                  Author:{" "}
                  {report.features?.userProfilesEnabled && report.canViewUserProfiles ? (
                    <Link href={`/users?id=${report.author.id}`} className="underline">
                      {getFullName(report.author) || report.author.username}
                    </Link>
                  ) : (
                    getFullName(report.author) || report.author.username
                  )}
                </div>
              ) : null}

              {report.lastModifiedBy ? (
                <div className="text-muted-foreground">
                  Last modified by:{" "}
                  {getFullName(report.lastModifiedBy) || report.lastModifiedBy.username}
                </div>
              ) : null}

              {report.requester ? (
                <div className="text-muted-foreground">
                  Requester:{" "}
                  {report.features?.userProfilesEnabled && report.canViewUserProfiles ? (
                    <Link
                      href={`/users?id=${report.requester.id}`}
                      className="underline"
                    >
                      {getFullName(report.requester) || report.requester.username}
                    </Link>
                  ) : (
                    getFullName(report.requester) || report.requester.username
                  )}
                </div>
              ) : null}

              {typeof report.runs === "number" ? (
                <div className="text-muted-foreground">Runs: {report.runs}</div>
              ) : null}
            </CardContent>
          </Card>

          {featureFlags.termsEnabled && report.terms && report.terms.length > 0 ? (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Terms</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2">
                  {report.terms.map((t) => (
                    <li key={t.id} className="text-sm">
                      {t.name ?? t.summary ?? `Term ${t.id}`}
                    </li>
                  ))}
                </ul>
              </CardContent>
            </Card>
          ) : null}

          {report.canViewGroups &&
          Array.isArray(report.groups) &&
          report.groups.length > 0 ? (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Groups</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2">
                  {report.groups.map((g) => (
                    <li key={g.id} className="text-sm">
                      {g.name ?? g.email ?? `Group ${g.id}`}
                    </li>
                  ))}
                </ul>
              </CardContent>
            </Card>
          ) : null}

          {Array.isArray(report.parents) || Array.isArray(report.children) ? (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Relationships</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {Array.isArray(report.parents) && report.parents.length > 0 ? (
                  <div>
                    <div className="text-sm font-medium">Parents</div>
                    <ul className="mt-2 space-y-2 text-sm text-muted-foreground">
                      {report.parents.map((p) => (
                        <li key={p.id ?? p.url}>
                          <Link
                            href={`/reports?id=${p.id ?? ""}`}
                            className="underline"
                          >
                            {p.name ?? p.displayTitle ?? p.type ?? "Report"}
                          </Link>
                        </li>
                      ))}
                    </ul>
                  </div>
                ) : null}
                {Array.isArray(report.children) && report.children.length > 0 ? (
                  <div>
                    <div className="text-sm font-medium">Children</div>
                    <ul className="mt-2 space-y-2 text-sm text-muted-foreground">
                      {report.children.map((c) => (
                        <li key={c.id ?? c.url}>
                          <Link
                            href={`/reports?id=${c.id ?? ""}`}
                            className="underline"
                          >
                            {c.name ?? c.displayTitle ?? c.type ?? "Report"}
                          </Link>
                        </li>
                      ))}
                    </ul>
                  </div>
                ) : null}
              </CardContent>
            </Card>
          ) : null}

          {Array.isArray(report.queries) && report.queries.length > 0 ? (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Queries</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2">
                  {report.queries.map((q) => (
                    <li key={q.id} className="text-sm">
                      <div className="font-medium">{q.name ?? `Query ${q.id}`}</div>
                      {q.language ? (
                        <div className="text-muted-foreground">
                          Language: {q.language}
                        </div>
                      ) : null}
                      {q.source ? (
                        <div className="text-muted-foreground">{q.source}</div>
                      ) : null}
                    </li>
                  ))}
                </ul>
              </CardContent>
            </Card>
          ) : null}

          {report.componentQueries && report.componentQueries.length > 0 ? (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Component Queries</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2">
                  {report.componentQueries.map((q) => (
                    <li key={q.id} className="text-sm">
                      <div className="font-medium">{q.name ?? `Query ${q.id}`}</div>
                      {q.language ? (
                        <div className="text-muted-foreground">
                          Language: {q.language}
                        </div>
                      ) : null}
                    </li>
                  ))}
                </ul>
              </CardContent>
            </Card>
          ) : null}

          {Array.isArray(report.images) && report.images.length > 0 ? (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Images</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
                  {report.images.map((img) => (
                    <div key={img.id} className="rounded-md border p-2">
                      {img.source ? (
                        // eslint-disable-next-line @next/next/no-img-element
                        <img
                          src={img.source}
                          alt={`Report image ${img.id}`}
                          className="h-auto w-full"
                        />
                      ) : null}
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          ) : null}
        </section>
      </main>
    </div>
  );
}

