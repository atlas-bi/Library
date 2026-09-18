import type { Metadata } from "next"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { UserSettingsPanel } from "@/components/users/user-settings-panel"
import { getCurrentUser, getToken } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { getUserSettings } from "@/lib/users/api"

export const metadata: Metadata = { title: "Settings" }

function getShellDisplayName(
  user: {
    fullname?: string | null
    username?: string | null
  } | null,
) {
  return user?.fullname?.trim() || user?.username?.trim() || "Guest"
}

export default async function UserSettingsPage() {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const currentUser = await getCurrentUser()
  const settingsResult = await getUserSettings()

  return (
    <LibraryShell
      displayName={getShellDisplayName(currentUser)}
      isSignedIn={Boolean(currentUser)}
      isAdministrator={currentUser?.roles.includes("Administrator") ?? false}
      adminEnabled={currentUser?.adminEnabled ?? false}
    >
      <h1 className="atlas-home-heading">Settings</h1>

      {settingsResult.error || !settingsResult.data ? (
        <p className="text-red-500">
          {getUserFriendlyErrorMessage(settingsResult.error ?? "unknown")}
        </p>
      ) : (
        <UserSettingsPanel
          initialShareNotificationEnabled={settingsResult.data.shareNotificationEnabled}
        />
      )}
    </LibraryShell>
  )
}
