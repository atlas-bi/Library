import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { ReportEditWizard } from "@/components/reports/report-edit-wizard"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { getPublicApiBase } from "@/lib/api-base"
import { type AuthUser, getCurrentUser, getToken, hasPermission } from "@/lib/auth"
import { getReportDetailById, getReportLookups } from "@/lib/reports/api"
import type { ReportEditLookupOptions } from "@/lib/reports/types"

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

type EditSearchParams = { id?: string }

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

export default async function EditReportPage({
  searchParams,
}: {
  searchParams: Promise<EditSearchParams>
}) {
  const resolvedSearchParams = await searchParams
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const user = await getCurrentUser()
  const shellProps = getShellProps(user)
  if (!user || !hasPermission(user, "Edit Report Documentation")) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Permission required</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              You need the Edit Report Documentation permission to update reports.
            </p>
            <Button asChild variant="outline">
              <Link href="/">Back to home</Link>
            </Button>
          </CardContent>
        </Card>
      </LibraryShell>
    )
  }

  const idRaw = getSingleValue(resolvedSearchParams.id)
  const id = idRaw ? Number(idRaw) : NaN
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
        <h1 className="atlas-home-heading">Missing report</h1>
        <p className="mt-2 text-sm text-muted-foreground">Provide a valid report id in the URL.</p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/reports">Back to reports</Link>
        </Button>
      </LibraryShell>
    )
  }

  const result = await getReportDetailById(id)
  const report = result.data
  if (!report) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Unable to load report</CardTitle>
          </CardHeader>
          <CardContent>
            <Button asChild variant="outline">
              <Link href="/reports">Back to reports</Link>
            </Button>
          </CardContent>
        </Card>
      </LibraryShell>
    )
  }

  if (!report.canEditDocumentation) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Editing not available</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              You do not have access to edit this report.
            </p>
            <Button asChild variant="outline">
              <Link href={`/reports?id=${report.id}`}>Back to report</Link>
            </Button>
          </CardContent>
        </Card>
      </LibraryShell>
    )
  }

  const [
    organizationalValues,
    runFrequencies,
    fragilities,
    maintenanceSchedules,
    fragilityTags,
    maintenanceLogStatuses,
  ] = await Promise.all([
    getReportLookups("org-value"),
    getReportLookups("run-freq"),
    getReportLookups("fragility"),
    getReportLookups("maint-sched"),
    getReportLookups("ro-fragility"),
    getReportLookups("maint-log-stat"),
  ])

  const lookupOptions: ReportEditLookupOptions = {
    organizationalValues: organizationalValues.data ?? [],
    runFrequencies: runFrequencies.data ?? [],
    fragilities: fragilities.data ?? [],
    maintenanceSchedules: maintenanceSchedules.data ?? [],
    fragilityTags: fragilityTags.data ?? [],
    maintenanceLogStatuses: maintenanceLogStatuses.data ?? [],
  }

  const publicApiBase = getPublicApiBase()

  return (
    <LibraryShell {...shellProps} searchPlaceholder="search for reports..">
      <div className="mb-4 text-sm text-muted-foreground">
        <Link href={`/reports?id=${report.id}`} className="hover:underline">
          {report.displayTitle || report.displayName || report.name}
        </Link>
        <span className="px-1">/</span>
        <span>Edit documentation</span>
      </div>
      <ReportEditWizard
        report={report}
        cancelHref={`/reports?id=${report.id}`}
        lookupOptions={lookupOptions}
        publicApiBase={publicApiBase}
      />
    </LibraryShell>
  )
}
