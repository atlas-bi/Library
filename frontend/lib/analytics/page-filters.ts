import type { AnalyticsPageFilters } from "@/lib/analytics/types"
import { DEFAULT_ANALYTICS_RANGE_ID } from "@/lib/analytics/date-ranges"

export function buildInitialAnalyticsFilters(searchParams: {
  userId?: string
  groupId?: string
}): AnalyticsPageFilters {
  const userId = searchParams.userId ? Number.parseInt(searchParams.userId, 10) : undefined
  const groupId = searchParams.groupId ? Number.parseInt(searchParams.groupId, 10) : undefined

  return {
    rangeId: DEFAULT_ANALYTICS_RANGE_ID,
    userId: Number.isFinite(userId) && userId! > 0 ? userId : undefined,
    groupId: Number.isFinite(groupId) && groupId! > 0 ? groupId : undefined,
    tracePage: 0,
    errorPage: 0,
  }
}
