import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import { buildAnalyticsLogQueryString, buildAnalyticsQueryString } from "./query"
import type {
  AnalyticsBarItemDto,
  AnalyticsDashboardData,
  AnalyticsErrorListResponseDto,
  AnalyticsLiveUsersResponseDto,
  AnalyticsLogFilters,
  AnalyticsQueryFilters,
  AnalyticsTraceListResponseDto,
  AnalyticsVisitsResponseDto,
} from "./types"

export type AnalyticsResult<T> = {
  data: T | null
  error: AppErrorCode | null
}

async function analyticsGet<T>(path: string): Promise<AnalyticsResult<T>> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<T>(`${apiBase}${path}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code }
  return { data: result.data, error: null }
}

async function analyticsPost(path: string): Promise<AnalyticsResult<{ status: string }>> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<{ status: string }>(`${apiBase}${path}`, {
    method: "POST",
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code }
  return { data: result.data, error: null }
}

export async function getAnalyticsVisits(
  filters: AnalyticsQueryFilters,
): Promise<AnalyticsResult<AnalyticsVisitsResponseDto>> {
  return analyticsGet(`/api/analytics/visits${buildAnalyticsQueryString(filters)}`)
}

export async function getAnalyticsBrowsers(
  filters: AnalyticsQueryFilters,
): Promise<AnalyticsResult<AnalyticsBarItemDto[]>> {
  return analyticsGet(`/api/analytics/visits/browsers${buildAnalyticsQueryString(filters)}`)
}

export async function getAnalyticsOs(
  filters: AnalyticsQueryFilters,
): Promise<AnalyticsResult<AnalyticsBarItemDto[]>> {
  return analyticsGet(`/api/analytics/visits/os${buildAnalyticsQueryString(filters)}`)
}

export async function getAnalyticsResolution(
  filters: AnalyticsQueryFilters,
): Promise<AnalyticsResult<AnalyticsBarItemDto[]>> {
  return analyticsGet(`/api/analytics/visits/resolution${buildAnalyticsQueryString(filters)}`)
}

export async function getAnalyticsTopUsers(
  filters: AnalyticsQueryFilters,
): Promise<AnalyticsResult<AnalyticsBarItemDto[]>> {
  return analyticsGet(`/api/analytics/visits/users${buildAnalyticsQueryString(filters)}`)
}

export async function getAnalyticsLoadTimes(
  filters: AnalyticsQueryFilters,
): Promise<AnalyticsResult<AnalyticsBarItemDto[]>> {
  return analyticsGet(`/api/analytics/visits/load-times${buildAnalyticsQueryString(filters)}`)
}

export async function getAnalyticsLiveUsers(): Promise<
  AnalyticsResult<AnalyticsLiveUsersResponseDto>
> {
  return analyticsGet("/api/analytics/live-users")
}

export async function getAnalyticsTraces(
  filters: AnalyticsLogFilters,
): Promise<AnalyticsResult<AnalyticsTraceListResponseDto>> {
  return analyticsGet(`/api/analytics/traces${buildAnalyticsLogQueryString(filters)}`)
}

export async function getAnalyticsErrors(
  filters: AnalyticsLogFilters,
): Promise<AnalyticsResult<AnalyticsErrorListResponseDto>> {
  return analyticsGet(`/api/analytics/errors${buildAnalyticsLogQueryString(filters)}`)
}

export async function resolveAnalyticsTrace(
  id: number,
  type: 1 | 2,
): Promise<AnalyticsResult<{ status: string }>> {
  return analyticsPost(`/api/analytics/traces/${id}/resolve?type=${type}`)
}

export async function resolveAnalyticsError(
  id: number,
  type: 1 | 2,
): Promise<AnalyticsResult<{ status: string }>> {
  return analyticsPost(`/api/analytics/errors/${id}/resolve?type=${type}`)
}

export async function getAnalyticsDashboard(
  queryFilters: AnalyticsQueryFilters,
  options: {
    includeTopUsers: boolean
    tracePage: number
    errorPage: number
  },
): Promise<AnalyticsResult<AnalyticsDashboardData>> {
  const logBase = { ...queryFilters }
  const [visits, browsers, os, resolution, users, loadTimes, liveUsers, traces, errors] =
    await Promise.all([
      getAnalyticsVisits(queryFilters),
      getAnalyticsBrowsers(queryFilters),
      getAnalyticsOs(queryFilters),
      getAnalyticsResolution(queryFilters),
      options.includeTopUsers
        ? getAnalyticsTopUsers(queryFilters)
        : Promise.resolve({ data: [], error: null }),
      getAnalyticsLoadTimes(queryFilters),
      getAnalyticsLiveUsers(),
      getAnalyticsTraces({ ...logBase, page: options.tracePage }),
      getAnalyticsErrors({ ...logBase, page: options.errorPage }),
    ])

  const firstError =
    visits.error ??
    browsers.error ??
    os.error ??
    resolution.error ??
    users.error ??
    loadTimes.error ??
    liveUsers.error ??
    traces.error ??
    errors.error

  if (firstError) {
    return { data: null, error: firstError }
  }

  return {
    data: {
      visits: visits.data,
      browsers: browsers.data ?? [],
      os: os.data ?? [],
      resolution: resolution.data ?? [],
      users: users.data ?? [],
      loadTimes: loadTimes.data ?? [],
      liveUsers: liveUsers.data,
      traces: traces.data,
      errors: errors.data,
    },
    error: null,
  }
}
