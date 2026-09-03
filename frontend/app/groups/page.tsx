import Link from "next/link"
import { redirect } from "next/navigation"
import type { Metadata } from "next"
import { GroupDetailsTable } from "@/components/groups/group-details-table"
import { GroupPageTabs } from "@/components/groups/group-page-tabs"
import { GroupReportsTable } from "@/components/groups/group-reports-table"
import { GroupRunListPanel } from "@/components/groups/group-run-list-panel"
import { GroupUsersTable } from "@/components/groups/group-users-table"
import { HomeGroupsPanelView } from "@/components/home/home-groups-panel"
import { LibraryShell } from "@/components/layout/library-shell"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { type AuthUser, getCurrentUser, getToken } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { getGroupById, getGroupReports, getGroupsList, getGroupUsers } from "@/lib/groups/api"
import { getHomeUserPageSummary } from "@/lib/home/api"

type GroupsSearchParams = {
  id?: string
}

type ShellProps = {
  displayName: string
  isSignedIn: boolean
  isAdministrator: boolean
  adminEnabled: boolean
}

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

function resolveDisplayName(user: AuthUser | null): string {
  if (!user) return "Guest"
  if (user.fullname && user.fullname !== "Guest") return user.fullname
  return user.username || "Guest"
}

function getShellProps(user: AuthUser | null): ShellProps {
  return {
    displayName: resolveDisplayName(user),
    isSignedIn: !!user,
    isAdministrator: !!user && user.roles.includes("Administrator"),
    adminEnabled: user?.adminEnabled ?? false,
  }
}

export async function generateMetadata({
  searchParams,
}: {
  searchParams: Promise<GroupsSearchParams>
}): Promise<Metadata> {
  const resolvedSearchParams = await searchParams
  const idRaw = getSingleValue(resolvedSearchParams.id)

  if (!idRaw) {
    return { title: "Groups" }
  }

  const id = Number(idRaw)
  if (!Number.isFinite(id) || id <= 0) {
    return { title: "Group not found" }
  }

  const result = await getGroupById(id)
  return {
    title: result.data?.name?.trim() || `Group ${id}`,
  }
}

export default async function GroupsPage({
  searchParams,
}: {
  searchParams: Promise<GroupsSearchParams>
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const resolvedSearchParams = await searchParams
  const idRaw = getSingleValue(resolvedSearchParams.id)
  
  const user = await getCurrentUser()
  const shellProps = getShellProps(user)

  if (!idRaw) {
    return <GroupsListView shellProps={shellProps} />
  }

  const id = Number(idRaw)
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for groups..">
        <div className="mx-auto max-w-4xl py-10">
          <h1 className="text-2xl font-bold">Group not found</h1>
          <p className="mt-2 text-sm text-muted-foreground">Missing or invalid group id.</p>
          <Button asChild className="mt-6" variant="outline">
            <Link href="/groups">Back to groups</Link>
          </Button>
        </div>
      </LibraryShell>
    )
  }

  const [groupResult, usersResult, reportsResult] = await Promise.all([
    getGroupById(id),
    getGroupUsers(id),
    getGroupReports(id),
  ])

  const group = groupResult.data
  if (!group) {
    const message = getUserFriendlyErrorMessage(groupResult.error ?? "unknown")
    const denied = groupResult.error === "forbidden" || groupResult.status === 403

    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for groups..">
        <div className="mx-auto max-w-4xl py-10">
          <Card>
            <CardHeader>
              <CardTitle className="text-xl">
                {denied ? "Access denied" : "Unable to load group"}
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <p className="text-sm text-muted-foreground">
                {denied ? "You do not have access to this page." : message}
              </p>
              <Button asChild variant="outline">
                <Link href="/groups">Back to groups</Link>
              </Button>
            </CardContent>
          </Card>
        </div>
      </LibraryShell>
    )
  }

  if (group.permissions?.canViewGroups === false) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for groups..">
        <div className="mx-auto max-w-4xl py-10">
          <p className="text-sm text-muted-foreground">You do not have access to this page.</p>
        </div>
      </LibraryShell>
    )
  }

  const userId = user?.userId ? Number(user.userId) : NaN
  const summaryResult =
    Number.isFinite(userId) && userId > 0 ? await getHomeUserPageSummary(userId) : null
  const reportTypeIds = summaryResult?.data?.defaultReportTypeIds ?? []
  const showAnalytics = group.permissions?.canViewSiteAnalytics === true

  const users = usersResult.data ?? []
  const reports = reportsResult.data ?? []

  return (
    <LibraryShell {...shellProps} searchPlaceholder="search for groups..">
      <div className="mb-6 space-y-4 border-b border-[var(--atlas-home-border-soft)] pb-6">
        <div className="text-sm text-[var(--atlas-home-muted)]">
          <Link href="/" className="text-[var(--atlas-home-link)] hover:underline">
            Home
          </Link>
          <span className="mx-2 text-[var(--atlas-home-muted)]">/</span>
          <Link href="/groups" className="text-[var(--atlas-home-link)] hover:underline">
            Groups
          </Link>
        </div>
        <h1 className="atlas-home-heading mb-0">
          {group.name?.trim() || `Group ${group.id}`}
        </h1>
      </div>

      <GroupPageTabs
        showAnalytics={showAnalytics}
        details={<GroupDetailsTable group={group} />}
        users={<GroupUsersTable users={users} />}
        reports={<GroupReportsTable reports={reports} />}
        activity={<ProfileAnalyticsPanel id={group.id} type="group" />}
        runList={<GroupRunListPanel groupId={group.id} reportTypeIds={reportTypeIds} />}
        analytics={
          <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
              Site analytics for this group are available in the Analytics application.
            </p>
            <Button asChild variant="outline">
              <Link href="/analytics">Open Analytics</Link>
            </Button>
          </div>
        }
      />
    </LibraryShell>
  )
}

async function GroupsListView({ shellProps }: { shellProps: ShellProps }) {
  const result = await getGroupsList()
  const list = result.data

  if (!list) {
    const message = getUserFriendlyErrorMessage(result.error ?? "unknown")
    const denied = result.error === "forbidden" || result.status === 403

    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for groups..">
        <div className="mx-auto max-w-4xl py-10">
          <Card>
            <CardHeader>
              <CardTitle className="text-xl">
                {denied ? "Access denied" : "Unable to load groups"}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground">
                {denied ? "You do not have access to this page." : message}
              </p>
            </CardContent>
          </Card>
        </div>
      </LibraryShell>
    )
  }

  if (list.permissions?.canViewGroups === false) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for groups..">
        <div className="mx-auto max-w-4xl py-10">
          <p className="text-sm text-muted-foreground">You do not have access to this page.</p>
        </div>
      </LibraryShell>
    )
  }

  const panel = {
    kind: "groups" as const,
    title: "Groups",
    emptyMessage: "No groups to show.",
    rows: (list.items ?? []).map((item) => ({
      id: String(item.id),
      name: item.name?.trim() || `Group ${item.id}`,
      type: item.type?.trim() || undefined,
      source: item.source?.trim() || undefined,
      href: item.url ?? `/groups?id=${item.id}`,
    })),
  }

  return (
    <LibraryShell {...shellProps} searchPlaceholder="search for groups..">
      <div className="mb-6 space-y-4 border-b border-[var(--atlas-home-border-soft)] pb-6">
        <div className="text-sm text-[var(--atlas-home-muted)]">
          <Link href="/" className="text-[var(--atlas-home-link)] hover:underline">
            Home
          </Link>
        </div>
        <h1 className="atlas-home-heading mb-0">Groups</h1>
      </div>
      <HomeGroupsPanelView panel={panel} />
    </LibraryShell>
  )
}
