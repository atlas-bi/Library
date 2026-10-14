import { redirect } from "next/navigation"
import { HomeShell } from "@/components/home/home-shell"
import { getCurrentUser, getToken } from "@/lib/auth"
import { getHomeUserPageSummary } from "@/lib/home/api"
import type { HomeTabsVisibility } from "@/lib/home/types"

const allTabsVisible: HomeTabsVisibility = {
  stars: true,
  subscriptions: true,
  "report-runs": true,
  groups: true,
}

function samlLoginRequired(): boolean {
  const demo = process.env.DEMO?.trim().toLowerCase()
  return demo === "false" || demo === "0" || demo === "no"
}

export default async function HomePage() {
  if (!(await getToken()) && samlLoginRequired()) {
    redirect("/auth/login")
  }

  const user = await getCurrentUser()

  if (!user) {
    return (
      <HomeShell
        displayName="Guest"
        isSignedIn={false}
        isAdministrator={false}
        adminEnabled={false}
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
      visibleTabs={allTabsVisible}
    />
  )
}
