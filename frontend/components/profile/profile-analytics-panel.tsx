import { loadProfileAnalyticsAction } from "@/app/profile/actions"
import { ProfileFullView } from "@/components/profile/profile-full-view"

export async function ProfileAnalyticsPanel({
  id,
  type,
  userProfilesEnabled = true,
}: {
  id: number
  type: string
  userProfilesEnabled?: boolean
}) {
  const result = await loadProfileAnalyticsAction(id, type)
  if (!result.data) {
    return <p className="text-sm text-muted-foreground">Profile analytics unavailable.</p>
  }

  return (
    <ProfileFullView
      id={id}
      type={type}
      initialData={result.data}
      userProfilesEnabled={userProfilesEnabled}
    />
  )
}
