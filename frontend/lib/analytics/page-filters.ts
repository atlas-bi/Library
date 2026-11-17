import {
  ANALYTICS_RANGE_OPTIONS,
  DEFAULT_ANALYTICS_RANGE_ID,
  type AnalyticsRangeId,
} from "@/lib/analytics/date-ranges"
import type { AnalyticsPageFilters } from "@/lib/analytics/types"

export function parseAnalyticsRangeId(value: string | undefined): AnalyticsRangeId {
  if (value && ANALYTICS_RANGE_OPTIONS.some((option) => option.id === value)) {
    return value as AnalyticsRangeId
  }
  return DEFAULT_ANALYTICS_RANGE_ID
}

export function buildInitialAnalyticsFilters(searchParams: {
  userId?: string
  groupId?: string
  range?: string
  tracePage?: string
  errorPage?: string
}): AnalyticsPageFilters {
  const userId = searchParams.userId ? Number.parseInt(searchParams.userId, 10) : undefined
  const groupId = searchParams.groupId ? Number.parseInt(searchParams.groupId, 10) : undefined
  const tracePage = searchParams.tracePage ? Number.parseInt(searchParams.tracePage, 10) : 0
  const errorPage = searchParams.errorPage ? Number.parseInt(searchParams.errorPage, 10) : 0

  return {
    rangeId: parseAnalyticsRangeId(searchParams.range),
    userId: Number.isFinite(userId) && userId! > 0 ? userId : undefined,
    groupId: Number.isFinite(groupId) && groupId! > 0 ? groupId : undefined,
    tracePage: Number.isFinite(tracePage) && tracePage >= 0 ? tracePage : 0,
    errorPage: Number.isFinite(errorPage) && errorPage >= 0 ? errorPage : 0,
  }
}

export function buildAnalyticsUrlQuery(filters: AnalyticsPageFilters): string {
  const params = new URLSearchParams()
  if (filters.userId) params.set("userId", String(filters.userId))
  if (filters.groupId) params.set("groupId", String(filters.groupId))
  if (filters.rangeId !== DEFAULT_ANALYTICS_RANGE_ID) {
    params.set("range", filters.rangeId)
  }
  if (filters.tracePage > 0) params.set("tracePage", String(filters.tracePage))
  if (filters.errorPage > 0) params.set("errorPage", String(filters.errorPage))
  const query = params.toString()
  return query ? `?${query}` : ""
}
