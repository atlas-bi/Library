import Image from "next/image"
import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import { ReportActionRail } from "@/components/reports/report-action-rail"
import { ReportDescriptionSection } from "@/components/reports/report-description-section"
import { ReportDetailsTable } from "@/components/reports/report-details-table"
import { ReportMaintenanceSection } from "@/components/reports/report-maintenance-section"
import { ReportQuerySection } from "@/components/reports/report-query-section"
import { ReportRelationshipsSection } from "@/components/reports/report-relationships-section"
import {
  buildReportSectionLinks,
  ReportSectionNav,
} from "@/components/reports/report-section-nav"
import { ReportTermsSection } from "@/components/reports/report-terms-section"
import { ReportsListTable } from "@/components/reports/reports-list-table"
import { AppAlertDialog } from "@/components/ui/app-alert-dialog"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { type AuthUser, getCurrentUser, getToken } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { getReportDetailById, getReportsList } from "@/lib/reports/api"
import type { ReportDetail } from "@/lib/reports/types"

type ReportsSearchParams = {
  id?: string
  page?: string
  pageSize?: string
}

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

function asPositiveInt(raw: string | undefined, fallback: number): number {
  const parsed = Number(raw)
  return Number.isFinite(parsed) && parsed > 0 ? Math.floor(parsed) : fallback
}

function buildListHref(page: number, pageSize: number): string {
  const params = new URLSearchParams()
  params.set("page", String(page))
  params.set("pageSize", String(pageSize))
  return `/reports?${params.toString()}`
}

function formatReportTitle(report: ReportDetail) {
  return report.displayTitle || report.displayName || report.name
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

  const idRaw = getSingleValue(resolvedSearchParams.id)
  if (!idRaw) {
    const page = asPositiveInt(getSingleValue(resolvedSearchParams.page), 1)
    const pageSize = Math.min(100, asPositiveInt(getSingleValue(resolvedSearchParams.pageSize), 20))
    const listResult = await getReportsList(page, pageSize)
    const list = listResult.data

    if (!list) {
      const message = getUserFriendlyErrorMessage(listResult.error ?? "unknown")
      return (
        <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
          <Card>
            <CardHeader>
              <CardTitle className="text-xl">Unable to load reports</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <p className="text-sm text-muted-foreground">{message}</p>
              <Button asChild variant="outline">
                <Link href="/">Back to home</Link>
              </Button>
            </CardContent>
          </Card>
        </LibraryShell>
      )
    }

    const totalPages = Math.max(1, Math.ceil(list.total / list.pageSize))

    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
        <header className="mb-6 space-y-3 border-b border-[var(--atlas-home-border-soft)] pb-6">
          <div className="text-sm text-[var(--atlas-home-muted)]">
            <Link href="/" className="text-[var(--atlas-home-link)] hover:underline">
              Home
            </Link>
          </div>
          <h1 className="atlas-home-heading mb-0">Reports</h1>
          <p className="text-sm text-[var(--atlas-home-muted)]">
            Browse documented reports in the library.
          </p>
        </header>

        <ReportsListTable reports={list.reports} />

        {list.total > list.pageSize ? (
          <div className="mt-6 flex items-center justify-between gap-2 border-t border-[var(--atlas-home-border-soft)] pt-4 text-sm text-[var(--atlas-home-muted)]">
            <span>
              Page {list.page} of {totalPages} ({list.total} reports)
            </span>
            <div className="flex gap-2">
              {list.page > 1 ? (
                <Button asChild variant="outline" size="sm">
                  <Link href={buildListHref(list.page - 1, list.pageSize)}>Previous</Link>
                </Button>
              ) : null}
              {list.page < totalPages ? (
                <Button asChild variant="outline" size="sm">
                  <Link href={buildListHref(list.page + 1, list.pageSize)}>Next</Link>
                </Button>
              ) : null}
            </div>
          </div>
        ) : null}
      </LibraryShell>
    )
  }

  const id = Number(idRaw)
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
        <h1 className="atlas-home-heading">Report not found</h1>
        <p className="text-sm text-[var(--atlas-home-muted)]">Missing or invalid report id.</p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/reports">Back to reports</Link>
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
                <Link href="/reports">Back to reports</Link>
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
  const sectionLinks = buildReportSectionLinks(report)

  return (
    <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
      <div className="mb-6 space-y-4 border-b border-[var(--atlas-home-border-soft)] pb-6">
        <div className="text-sm text-[var(--atlas-home-muted)]">
          <Link href="/" className="text-[var(--atlas-home-link)] hover:underline">
            Home
          </Link>
          <span className="px-1">/</span>
          <Link href="/reports" className="text-[var(--atlas-home-link)] hover:underline">
            Reports
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
        </div>
        <ReportSectionNav links={sectionLinks} />
      </div>

      <div className="grid gap-10 xl:grid-cols-[4.75rem_minmax(0,1fr)]">
        <ReportActionRail
          report={report}
          title={title}
          profilePanel={<ProfileAnalyticsPanel id={report.id} type="report" />}
        />

        <div className="min-w-0 space-y-10">
          {report.maintenanceStatus?.isRequired ? (
            <div className="rounded-md border border-amber-300 bg-amber-50 px-4 py-3 text-sm text-amber-950">
              <div className="font-semibold">Maintenance past due</div>
              <div>{report.maintenanceStatus.message ?? "Maintenance is required."}</div>
              {report.maintenanceStatus.nextMaintenanceDate ? (
                <div className="mt-1 text-amber-900/80">
                  Next maintenance:{" "}
                  {new Date(report.maintenanceStatus.nextMaintenanceDate).toLocaleDateString()}
                </div>
              ) : null}
            </div>
          ) : null}

          {Array.isArray(report.images) && report.images.length > 0 ? (
            <section id="images" className="space-y-4 scroll-mt-24">
              <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Images</h2>
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
            </section>
          ) : null}

          <ReportDescriptionSection report={report} />
          <ReportTermsSection report={report} />
          <ReportDetailsTable
            report={report}
            canViewUserProfiles={report.features?.userProfilesEnabled && report.canViewUserProfiles}
          />
          <ReportQuerySection report={report} />
          <ReportRelationshipsSection report={report} />
          <ReportMaintenanceSection
            report={report}
            canViewUserProfiles={report.features?.userProfilesEnabled && report.canViewUserProfiles}
          />
        </div>
      </div>
    </LibraryShell>
  )
}
