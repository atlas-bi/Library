import type { AnalyticsLogFilters, AnalyticsQueryFilters } from "./types"

function appendFilterParams(params: URLSearchParams, filters: AnalyticsQueryFilters) {
  params.set("start_at", String(filters.startAt))
  params.set("end_at", String(filters.endAt))
  if (filters.userId != null && filters.userId > 0) {
    params.set("userId", String(filters.userId))
  }
  if (filters.groupId != null && filters.groupId > 0) {
    params.set("groupId", String(filters.groupId))
  }
}

export function buildAnalyticsQueryString(filters: AnalyticsQueryFilters): string {
  const params = new URLSearchParams()
  appendFilterParams(params, filters)
  const qs = params.toString()
  return qs ? `?${qs}` : ""
}

export function buildAnalyticsLogQueryString(filters: AnalyticsLogFilters): string {
  const params = new URLSearchParams()
  appendFilterParams(params, filters)
  params.set("p", String(filters.page))
  const qs = params.toString()
  return qs ? `?${qs}` : ""
}
