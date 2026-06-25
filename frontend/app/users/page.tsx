import Link from "next/link"
import { redirect } from "next/navigation"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { UserGroupsTable } from "@/components/users/user-groups-table"
import { UserHistorySectionView } from "@/components/users/user-history-section"
import { UserPageTabs } from "@/components/users/user-page-tabs"
import { UserRunListPanel } from "@/components/users/user-run-list-panel"
import { UserStarsWorkspace } from "@/components/users/user-stars-workspace"
import { UserSubscriptionsTable } from "@/components/users/user-subscriptions-table"
import { getCurrentUser, getToken } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import {
  getUserGroups,
  getUserHistory,
  getUserPage,
  getUserStars,
  getUserSubscriptions,
} from "@/lib/users/api"

type UsersSearchParams = {
  id?: string
}

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

export default async function UsersPage({ searchParams }: { searchParams: UsersSearchParams }) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const currentUser = await getCurrentUser()
  const idRaw = getSingleValue(searchParams.id)
  const resolvedId = idRaw ? Number(idRaw) : Number(currentUser?.userId)
  if (!Number.isFinite(resolvedId) || resolvedId <= 0) {
    redirect("/")
  }

  if (!idRaw && currentUser?.userId) {
    redirect(`/users?id=${currentUser.userId}`)
  }

  const [pageResult, starsResult, groupsResult, subscriptionsResult, historyResult] =
    await Promise.all([
      getUserPage(resolvedId),
      getUserStars(resolvedId),
      getUserGroups(resolvedId),
      getUserSubscriptions(resolvedId),
      getUserHistory(resolvedId),
    ])

  const page = pageResult.data
  if (!page) {
    const message = getUserFriendlyErrorMessage(pageResult.error ?? "unknown")
    const denied = pageResult.error === "forbidden" || pageResult.status === 403

    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">
              {denied ? "Access denied" : "Unable to load user profile"}
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              {denied ? "You do not have access to this page." : message}
            </p>
            <Button asChild variant="outline">
              <Link href="/">Back to home</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  const stars = starsResult.data
  const groups = groupsResult.data ?? []
  const subscriptions = subscriptionsResult.data ?? []
  const history = historyResult.data ?? {
    atlasHistory: [],
    reportEdits: [],
    initiativeEdits: [],
    collectionEdits: [],
    termEdits: [],
  }

  const title = page.viewer.isCurrentUser
    ? `Hi, ${page.user.firstName?.trim() || page.user.displayName?.trim() || page.user.fullName?.trim() || "there"}`
    : `You are viewing ${page.user.fullName?.trim() || page.user.displayName?.trim() || page.user.username?.trim() || "this user"}'s Profile`

  return (
    <div className="min-h-screen bg-background">
      <div className="mx-auto w-full max-w-7xl px-4 py-8">
        <header className="mb-6 space-y-3 border-b border-border/60 pb-6">
          <h1 className="font-serif text-4xl font-semibold tracking-tight">{title}</h1>
          {page.permissions.canToggleAdminMode ? (
            <div>
              <Button asChild variant="outline" size="sm">
                <Link
                  href={`/auth/admin-mode?returnTo=${encodeURIComponent(`/users?id=${resolvedId}`)}`}
                >
                  Toggle admin mode
                </Link>
              </Button>
            </div>
          ) : null}
        </header>

        <UserPageTabs
          isCurrentUser={page.viewer.isCurrentUser}
          tabs={page.tabs}
          stars={
            stars ? (
              <UserStarsWorkspace stars={stars} />
            ) : (
              <p className="text-sm text-muted-foreground">Unable to load stars workspace.</p>
            )
          }
          subscriptions={<UserSubscriptionsTable rows={subscriptions} />}
          groups={<UserGroupsTable rows={groups} canViewGroups={page.permissions.canViewGroups} />}
          activity={
            <ProfileAnalyticsPanel id={resolvedId} type="user" />
          }
          runList={
            <UserRunListPanel userId={resolvedId} reportTypeIds={page.defaultReportTypeIds} />
          }
          atlasHistory={<UserHistorySectionView history={history} />}
          analytics={
            page.permissions.canViewAnalytics ? (
              <div className="space-y-4">
                <p className="text-sm text-muted-foreground">
                  Site analytics for this user are available in the Analytics application.
                </p>
                <Button asChild variant="outline">
                  <Link href="/analytics">Open Analytics</Link>
                </Button>
              </div>
            ) : (
              <p className="text-sm text-muted-foreground">Analytics are not available.</p>
            )
          }
        />
      </div>
    </div>
  )
}
