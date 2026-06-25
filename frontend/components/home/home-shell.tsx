import { HomeTabsClient } from "@/components/home/home-tabs-client"
import { LibraryShell } from "@/components/layout/library-shell"
import type { HomeTabsVisibility } from "@/lib/home/types"

type HomeShellProps = {
  displayName: string
  isSignedIn: boolean
  isAdministrator: boolean
  adminEnabled: boolean
  visibleTabs: HomeTabsVisibility
}

export function HomeShell({
  displayName,
  isSignedIn,
  isAdministrator,
  adminEnabled,
  visibleTabs,
}: HomeShellProps) {
  return (
    <LibraryShell
      displayName={displayName}
      isSignedIn={isSignedIn}
      isAdministrator={isAdministrator}
      adminEnabled={adminEnabled}
    >
      <h1 className="atlas-home-heading">Hi, {displayName}!</h1>

      {isSignedIn ? (
        <HomeTabsClient visibleTabs={visibleTabs} />
      ) : (
        <section className="atlas-home-card px-6 py-7 text-sm text-[var(--atlas-home-text)]">
          <a
            href="/auth/login"
            className="font-medium text-[var(--atlas-home-link)] hover:underline"
          >
            Sign in
          </a>{" "}
          to view your stars, subscriptions, report runs, and groups.
        </section>
      )}
    </LibraryShell>
  )
}
