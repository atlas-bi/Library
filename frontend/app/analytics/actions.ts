"use server"

import {
  getAnalyticsDashboard,
  getAnalyticsLiveUsers,
  resolveAnalyticsError,
  resolveAnalyticsTrace,
} from "@/lib/analytics/api"
import { getAnalyticsRangeParams } from "@/lib/analytics/date-ranges"
import type { AnalyticsDashboardData, AnalyticsPageFilters } from "@/lib/analytics/types"

export type { AnalyticsPageFilters }

function buildQueryFilters(filters: AnalyticsPageFilters) {
  const { startAt, endAt } = getAnalyticsRangeParams(filters.rangeId)
  return {
    startAt,
    endAt,
    userId: filters.userId,
    groupId: filters.groupId,
  }
}

export async function loadAnalyticsDashboardAction(
  filters: AnalyticsPageFilters,
): Promise<{ data: AnalyticsDashboardData | null; error: string | null }> {
  const queryFilters = buildQueryFilters(filters)

  const result = await getAnalyticsDashboard(queryFilters, {
    includeTopUsers: !filters.userId && !filters.groupId,
    tracePage: filters.tracePage,
    errorPage: filters.errorPage,
  })

  if (result.error) {
    return { data: null, error: result.error }
  }

  return { data: result.data, error: null }
}

export async function refreshAnalyticsLiveUsersAction(): Promise<{
  data: AnalyticsDashboardData["liveUsers"]
  error: string | null
}> {
  const result = await getAnalyticsLiveUsers()
  if (result.error) return { data: null, error: result.error }
  return { data: result.data, error: null }
}

export async function setAnalyticsTraceResolvedAction(
  id: number,
  resolved: boolean,
): Promise<{ ok: boolean; error: string | null }> {
  const result = await resolveAnalyticsTrace(id, resolved ? 1 : 2)
  if (result.error) return { ok: false, error: result.error }
  return { ok: true, error: null }
}

export async function setAnalyticsErrorResolvedAction(
  id: number,
  resolved: boolean,
): Promise<{ ok: boolean; error: string | null }> {
  const result = await resolveAnalyticsError(id, resolved ? 1 : 2)
  if (result.error) return { ok: false, error: result.error }
  return { ok: true, error: null }
}
