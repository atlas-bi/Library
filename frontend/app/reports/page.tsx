import Image from "next/image"
import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import { ReportActionRail } from "@/components/reports/report-action-rail"
import { AppAlertDialog } from "@/components/ui/app-alert-dialog"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { type AuthUser, getCurrentUser, getToken } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { getReportDetailById } from "@/lib/reports/api"
import type { ReportDetail } from "@/lib/reports/types"

type ReportsSearchParams = {
  id?: string
}

function formatReportTitle(report: ReportDetail) {
  return report.displayTitle || report.displayName || report.name
}

function getFullName(person?: { fullName?: string | null } | null) {
  return (person?.fullName ?? "").trim()
}

function resolveDisplayName(user: AuthUser | null): string {
  if (!user) return "Guest"
  if (user.fullname && user.fullname !== "Guest") return user.fullname
  return user.username || "Guest"
}

function getShellProps(user: AuthUser | null) {
  return {
    displayName: resolveDisplayName(user),
    isSignedIn: !!user,
    isAdministrator: !!user && user.roles.includes("Administrator"),
    adminEnabled: user?.adminEnabled ?? false,
  }
}

export default async function ReportsPage({
  searchParams,
}: {
  searchParams: Promise<ReportsSearchParams>
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const resolvedSearchParams = await searchParams
  const user = await getCurrentUser()
  const shellProps = getShellProps(user)

  const idRaw = resolvedSearchParams.id
  const id = idRaw ? Number(idRaw) : NaN
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
        <h1 className="atlas-home-heading">Report not found</h1>
        <p className="text-sm text-[var(--atlas-home-muted)]">Missing or invalid report id.</p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/">Back to home</Link>
        </Button>
      </LibraryShell>
    )
  }

  const result = await getReportDetailById(id)
  const report = result.data
  if (!report) {
    const message = getUserFriendlyErrorMessage(result.error ?? "unknown")
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Unable to load report</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">{message}</p>
            <div className="flex items-center gap-2">
              <Button asChild variant="outline">
                <Link href="/">Back to home</Link>
              </Button>
              <AppAlertDialog
                triggerLabel="See details"
                title="Report load issue"
                description={message}
                confirmLabel="OK"
                cancelLabel="Close"
                intent="error"
              />
            </div>
          </CardContent>
        </Card>
      </LibraryShell>
    )
  }

  const title = formatReportTitle(report)

  return (
    <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
      <div className="mb-6 space-y-4 border-b border-[var(--atlas-home-border-soft)] pb-6">
        <div className="text-sm text-[var(--atlas-home-muted)]">
          <Link href="/" className="text-[var(--atlas-home-link)] hover:underline">
            Home
          </Link>
        </div>
        <div className="space-y-3">
          <h1 className="atlas-home-heading mb-0">{title}</h1>
          <div className="text-sm text-[var(--atlas-home-muted)]">
            {report.typeShortName ? <span>{report.typeShortName}</span> : null}
            {report.availability ? <span> • {report.availability}</span> : null}
          </div>
          {Array.isArray(report.headerTags) && report.headerTags.length > 0 ? (
            <div className="flex flex-wrap gap-2">
              {report.headerTags
                .filter((tag) => tag.showInHeader === true || tag.showInHeader === "Y")
                .map((tag) => (
                  <span
                    key={tag.id}
                    className="rounded-md border border-[var(--atlas-home-border-soft)] bg-white px-2 py-1 text-xs text-[var(--atlas-home-muted)]"
                    title={tag.description ?? tag.name ?? ""}
                  >
                    {tag.name ?? `Tag ${tag.id}`}
                  </span>
                ))}
            </div>
          ) : null}
          {report.description || report.detailedDescription ? (
            <p className="max-w-3xl text-sm text-[var(--atlas-home-text)]">
              {report.detailedDescription || report.description}
            </p>
          ) : null}
        </div>
      </div>

      <div className="grid gap-10 xl:grid-cols-[4.75rem_minmax(0,1fr)]">
        <ReportActionRail
          report={report}
          title={title}
          profilePanel={<ProfileAnalyticsPanel id={report.id} type="report" />}
        />

        <div className="min-w-0 space-y-6">
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

          <Card>
            <CardHeader>
              <CardTitle className="text-base">Details</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 text-sm">
              {report.lastModified ? (
                <div className="text-muted-foreground">
                  Last modified: {new Date(report.lastModified).toLocaleString()}
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
                    <Link href={`/users?id=${report.requester.id}`} className="underline">
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

          {report.features?.termsEnabled !== false && report.terms && report.terms.length > 0 ? (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Terms</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2">
                  {report.terms.map((term) => (
                    <li key={term.id} className="text-sm">
                      {term.name ?? term.summary ?? `Term ${term.id}`}
                    </li>
                  ))}
                </ul>
              </CardContent>
            </Card>
          ) : null}

          {report.canViewGroups && Array.isArray(report.groups) && report.groups.length > 0 ? (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Groups</CardTitle>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2">
                  {report.groups.map((group) => (
                    <li key={group.id} className="text-sm">
                      {group.name ?? group.email ?? `Group ${group.id}`}
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
                      {report.parents.map((parent) => (
                        <li key={parent.id ?? parent.url}>
                          <Link href={`/reports?id=${parent.id ?? ""}`} className="underline">
                            {parent.name ?? parent.displayTitle ?? parent.type ?? "Report"}
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
                      {report.children.map((child) => (
                        <li key={child.id ?? child.url}>
                          <Link href={`/reports?id=${child.id ?? ""}`} className="underline">
                            {child.name ?? child.displayTitle ?? child.type ?? "Report"}
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
                  {report.queries.map((query) => (
                    <li key={query.id} className="text-sm">
                      <div className="font-medium">{query.name ?? `Query ${query.id}`}</div>
                      {query.language ? (
                        <div className="text-muted-foreground">Language: {query.language}</div>
                      ) : null}
                      {query.source ? (
                        <div className="text-muted-foreground">{query.source}</div>
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
                  {report.componentQueries.map((query) => (
                    <li key={query.id} className="text-sm">
                      <div className="font-medium">{query.name ?? `Query ${query.id}`}</div>
                      {query.language ? (
                        <div className="text-muted-foreground">Language: {query.language}</div>
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
                  {report.images.map((image) => (
                    <div key={image.id} className="rounded-md border p-2">
                      {image.source ? (
                        <Image
                          src={image.source}
                          alt={`Report ${image.id}`}
                          width={900}
                          height={600}
                          className="h-auto w-full"
                        />
                      ) : null}
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          ) : null}
        </div>
      </div>
    </LibraryShell>
  )
}
