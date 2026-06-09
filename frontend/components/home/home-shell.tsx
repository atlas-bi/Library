import { HomeTabsClient } from "@/components/home/home-tabs-client"
import { LibraryShell } from "@/components/layout/library-shell"
import type { HomeTabRequestContext, HomeTabsVisibility } from "@/lib/home/types"

type HomeShellProps = {
  displayName: string
  isSignedIn: boolean
  isAdministrator: boolean
  adminEnabled: boolean
  requestContext: HomeTabRequestContext
  visibleTabs: HomeTabsVisibility
}

export function HomeShell({
  displayName,
  isSignedIn,
  isAdministrator,
  adminEnabled,
  requestContext,
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

      <HomeTabsClient
        requestContext={requestContext}
        visibleTabs={visibleTabs}
        isSignedIn={isSignedIn}
      />
    </LibraryShell>
  )
}
