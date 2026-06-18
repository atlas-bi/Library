import Link from "next/link"
import { redirect } from "next/navigation"
import { GroupDetailsTable } from "@/components/groups/group-details-table"
import { GroupPageTabs } from "@/components/groups/group-page-tabs"
import { GroupReportsTable } from "@/components/groups/group-reports-table"
import { GroupRunListPanel } from "@/components/groups/group-run-list-panel"
import { GroupUsersTable } from "@/components/groups/group-users-table"
import { HomeGroupsPanelView } from "@/components/home/home-groups-panel"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { getCurrentUser, getToken } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { getGroupById, getGroupReports, getGroupsList, getGroupUsers } from "@/lib/groups/api"
import { getHomeUserPageSummary } from "@/lib/home/api"

type GroupsSearchParams = {
  id?: string
}

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

export default async function GroupsPage({ searchParams }: { searchParams: GroupsSearchParams }) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const idRaw = getSingleValue(searchParams.id)

  if (!idRaw) {
    return <GroupsListView />
  }

  const id = Number(idRaw)
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
        <h1 className="text-2xl font-bold">Group not found</h1>
        <p className="mt-2 text-sm text-muted-foreground">Missing or invalid group id.</p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/groups">Back to groups</Link>
        </Button>
      </div>
    )
  }

  const [groupResult, usersResult, reportsResult, user] = await Promise.all([
    getGroupById(id),
    getGroupUsers(id),
    getGroupReports(id),
    getCurrentUser(),
  ])

  const group = groupResult.data
  if (!group) {
    const message = getUserFriendlyErrorMessage(groupResult.error ?? "unknown")
    const denied = groupResult.error === "forbidden" || groupResult.status === 403

    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
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
    )
  }

  if (group.permissions?.canViewGroups === false) {
    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
        <p className="text-sm text-muted-foreground">You do not have access to this page.</p>
      </div>
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
    <div className="min-h-screen bg-background">
      <div className="mx-auto w-full max-w-7xl px-4 py-8">
        <header className="mb-6 space-y-2 border-b border-border/60 pb-6">
          <div className="text-sm text-muted-foreground">
            <Link href="/groups" className="hover:underline">
              Groups
            </Link>
          </div>
          <h1 className="font-serif text-4xl font-semibold tracking-tight">
            {group.name?.trim() || `Group ${group.id}`}
          </h1>
        </header>

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
      </div>
    </div>
  )
}

async function GroupsListView() {
  const result = await getGroupsList()
  const list = result.data

  if (!list) {
    const message = getUserFriendlyErrorMessage(result.error ?? "unknown")
    const denied = result.error === "forbidden" || result.status === 403

    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
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
    )
  }

  if (list.permissions?.canViewGroups === false) {
    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
        <p className="text-sm text-muted-foreground">You do not have access to this page.</p>
      </div>
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
    <div className="mx-auto min-h-screen w-full max-w-7xl space-y-8 px-4 py-8">
      <header className="space-y-2 border-b border-border/60 pb-6">
        <h1 className="font-serif text-4xl font-semibold tracking-tight">Groups</h1>
      </header>
      <HomeGroupsPanelView panel={panel} />
    </div>
  )
}
