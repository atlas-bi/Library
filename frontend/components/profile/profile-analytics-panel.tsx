import { loadProfileAnalyticsAction } from "@/app/profile/actions"
import { ProfileAnalyticsPanelView } from "@/components/profile/profile-analytics-panel-view"

export async function ProfileAnalyticsPanel({ id, type }: { id: number; type: string }) {
  const result = await loadProfileAnalyticsAction(id, type)
  if (!result.data) {
    return <p className="text-sm text-muted-foreground">Profile analytics unavailable.</p>
  }

  return <ProfileAnalyticsPanelView data={result.data} />
}
