import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type {
  ProfileBarItemDto,
  ProfileChartResponseDto,
  ProfileFilters,
  ProfileFiltersResponseDto,
  ProfileRunListItemDto,
  ProfileStarUserDto,
  ProfileSubscriptionDto,
} from "./types"

export type ProfileResult<T> = {
  data: T | null
  error: AppErrorCode | null
}

function buildProfileQuery(filters: ProfileFilters): string {
  const params = new URLSearchParams()
  params.set("id", String(filters.id))
  params.set("type", filters.type)
  params.set("start_at", String(filters.start_at ?? -31536000))
  params.set("end_at", String(filters.end_at ?? 0))

  const appendRepeated = (key: string, values?: string[] | number[]) => {
    if (!values) return
    for (const value of values) {
      params.append(key, String(value))
    }
  }

  appendRepeated("server", filters.server)
  appendRepeated("database", filters.database)
  appendRepeated("masterFile", filters.masterFile)
  appendRepeated("visible", filters.visible)
  appendRepeated("certification", filters.certification)
  appendRepeated("availability", filters.availability)
  appendRepeated("reportType", filters.reportType)

  return params.toString()
}

async function fetchProfile<T>(path: string, filters: ProfileFilters): Promise<ProfileResult<T>> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const query = buildProfileQuery(filters)
  const result = await apiFetchJson<T>(`${apiBase}/api/profile/${path}?${query}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code }
  return { data: result.data, error: null }
}

export function getProfileChart(filters: ProfileFilters) {
  return fetchProfile<ProfileChartResponseDto>("chart", filters)
}

export function getProfileUsers(filters: ProfileFilters) {
  return fetchProfile<ProfileBarItemDto[]>("users", filters)
}

export function getProfileReports(filters: ProfileFilters) {
  return fetchProfile<ProfileBarItemDto[]>("reports", filters)
}

export function getProfileFails(filters: ProfileFilters) {
  return fetchProfile<ProfileBarItemDto[]>("fails", filters)
}

export function getProfileRunList(filters: ProfileFilters) {
  return fetchProfile<ProfileRunListItemDto[]>("run-list", filters)
}

export function getProfileStars(filters: ProfileFilters) {
  return fetchProfile<ProfileStarUserDto[]>("stars", filters)
}

export function getProfileSubscriptions(filters: ProfileFilters) {
  return fetchProfile<ProfileSubscriptionDto[]>("subscriptions", filters)
}

/**
 * Fetches the available filter options (server, database, masterFile, etc.) for
 * the sidebar. Returns null when the backend endpoint is unavailable (404) so
 * the sidebar can fall back to free-text TagInputs gracefully.
 */
export function getProfileFilters(filters: ProfileFilters) {
  return fetchProfile<ProfileFiltersResponseDto>("filters", filters)
}
