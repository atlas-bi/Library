import type { Metadata } from "next"
import { redirect } from "next/navigation"
import { loadAnalyticsDashboardAction } from "@/app/analytics/actions"
import { AnalyticsDashboard } from "@/components/analytics/analytics-dashboard"
import { AnalyticsPageHeader } from "@/components/analytics/analytics-page-header"
import { buildInitialAnalyticsFilters } from "@/lib/analytics/page-filters"
import { LibraryShell } from "@/components/layout/library-shell"
import { type AuthUser, getCurrentUser, getToken } from "@/lib/auth"

export const metadata: Metadata = {
  title: "Analytics",
  description: "Site usage, live sessions, browser errors, and server errors.",
}

function getShellDisplayName(user: AuthUser | null): string {
  if (!user) return "Guest"
  if (user.fullname && user.fullname !== "Guest") return user.fullname
  return user.username || "Guest"
}

type AnalyticsPageProps = {
  searchParams: Promise<{
    userId?: string
    groupId?: string
    range?: string
    tracePage?: string
    errorPage?: string
  }>
}

export default async function AnalyticsPage({ searchParams }: AnalyticsPageProps) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const resolvedSearch = await searchParams
  const filters = buildInitialAnalyticsFilters(resolvedSearch)
  const [user, dashboardResult] = await Promise.all([
    getCurrentUser(),
    loadAnalyticsDashboardAction(filters),
  ])

  return (
    <LibraryShell
      displayName={getShellDisplayName(user)}
      isSignedIn={Boolean(user)}
      isAdministrator={user?.roles.includes("Administrator") ?? false}
      adminEnabled={user?.adminEnabled ?? false}
    >
      <AnalyticsPageHeader />
      <AnalyticsDashboard
        initialData={dashboardResult.data}
        initialFilters={filters}
        loadError={dashboardResult.error}
      />
    </LibraryShell>
  )
}
