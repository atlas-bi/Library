import { HomeShell } from "@/components/home/home-shell"
import { getCurrentUser } from "@/lib/auth"
import { getHomeUserPageSummary } from "@/lib/home/api"
import type { HomeTabsVisibility } from "@/lib/home/types"

const allTabsVisible: HomeTabsVisibility = {
  stars: true,
  subscriptions: true,
  "report-runs": true,
  groups: true,
}

export default async function HomePage() {
  const user = await getCurrentUser()

  if (!user) {
    return (
      <HomeShell
        displayName="Guest"
        isSignedIn={false}
        isAdministrator={false}
        adminEnabled={false}
        requestContext={{ userId: 0, defaultReportTypeIds: [] }}
        visibleTabs={allTabsVisible}
      />
    )
  }

  const userId = Number(user.userId)
  const page = Number.isFinite(userId) && userId > 0 ? await getHomeUserPageSummary(userId) : null
  const displayName = user.fullname || page?.data?.displayName || user.username || "Guest"

  return (
    <HomeShell
      displayName={displayName}
      isSignedIn
      isAdministrator={user.roles.includes("Administrator")}
      adminEnabled={user.adminEnabled}
      requestContext={{
        userId,
        defaultReportTypeIds: page?.data?.defaultReportTypeIds ?? [],
      }}
      visibleTabs={allTabsVisible}
    />
  )
}
