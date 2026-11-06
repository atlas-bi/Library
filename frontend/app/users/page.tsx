import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { ProfileFullView } from "@/components/profile/profile-full-view"
import { Button } from "@/components/ui/button"
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

function getShellDisplayName(
  user: {
    fullname?: string | null
    username?: string | null
  } | null,
) {
  return user?.fullname?.trim() || user?.username?.trim() || "Guest"
}

export default async function UsersPage({
  searchParams,
}: {
  searchParams: Promise<UsersSearchParams>
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const currentUser = await getCurrentUser()
  const resolvedSearchParams = await searchParams
  const idRaw = getSingleValue(resolvedSearchParams.id)
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
      <LibraryShell
        displayName={getShellDisplayName(currentUser)}
        isSignedIn={Boolean(currentUser)}
        isAdministrator={currentUser?.roles.includes("Administrator") ?? false}
        adminEnabled={currentUser?.adminEnabled ?? false}
      >
        <h1 className="atlas-home-heading">
          {denied ? "Access denied" : "Unable to load user profile"}
        </h1>
        <p className="mb-4 text-sm text-[var(--atlas-home-muted)]">
          {denied ? "You do not have access to this page." : message}
        </p>
        <Button asChild variant="outline">
          <Link href="/">Back to home</Link>
        </Button>
      </LibraryShell>
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
  const shellDisplayName =
    currentUser?.fullname?.trim() ||
    page.user.displayName?.trim() ||
    page.user.fullName?.trim() ||
    page.user.username?.trim() ||
    "Guest"

  return (
    <LibraryShell
      displayName={shellDisplayName}
      isSignedIn
      isAdministrator={currentUser?.roles.includes("Administrator") ?? false}
      adminEnabled={currentUser?.adminEnabled ?? false}
    >
      <header>
        <h1 className="atlas-home-heading">{title}</h1>
      </header>

      <UserPageTabs
        isCurrentUser={page.viewer.isCurrentUser}
        tabs={page.tabs}
        stars={
          stars ? (
            <UserStarsWorkspace stars={stars} />
          ) : (
            <div className="atlas-home-card px-6 py-7 text-sm text-[var(--atlas-home-text)]">
              Unable to load stars workspace.
            </div>
          )
        }
        subscriptions={<UserSubscriptionsTable rows={subscriptions} />}
        groups={<UserGroupsTable rows={groups} canViewGroups={page.permissions.canViewGroups} />}
        activity={
          <ProfileFullView
            id={resolvedId}
            type="user"
            variant="page"
            userProfilesEnabled={page.features.userProfilesEnabled}
          />
        }
        runList={<UserRunListPanel userId={resolvedId} reportTypeIds={page.defaultReportTypeIds} />}
        atlasHistory={<UserHistorySectionView history={history} />}
        analytics={
          page.permissions.canViewAnalytics ? (
            <div className="space-y-4">
              <p className="text-sm text-[var(--atlas-home-muted)]">
                Site analytics for this user are available in the Analytics application.
              </p>
              <Button asChild variant="outline">
                <Link href="/analytics">Open Analytics</Link>
              </Button>
            </div>
          ) : (
            <div className="atlas-home-card px-6 py-7 text-sm text-[var(--atlas-home-text)]">
              Analytics are not available.
            </div>
          )
        }
      />
    </LibraryShell>
  )
}
