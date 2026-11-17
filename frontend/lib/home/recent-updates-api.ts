import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type {
  RecentUpdateItem,
  RecentUpdatesBundle,
  RecentUpdatesSection,
} from "@/lib/home/recent-updates-types"

export type RecentUpdatesLoadResult = {
  data: RecentUpdatesBundle | null
  error: AppErrorCode | null
}

async function authorizedGetList(path: string): Promise<{
  items: RecentUpdateItem[]
  error: AppErrorCode | null
}> {
  const token = await getToken()
  if (!token) return { items: [], error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { items: [], error: "service_unavailable" }

  const result = await apiFetchJson<RecentUpdateItem[]>(`${apiBase}${path}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { items: [], error: result.error.code }
  return { items: result.data ?? [], error: null }
}

function toSection(items: RecentUpdateItem[], error: AppErrorCode | null): RecentUpdatesSection {
  return { items, failed: error !== null && error !== "auth_required" }
}

/** `GET /api/reports/recent` */
export async function getRecentReports(): Promise<{
  items: RecentUpdateItem[]
  error: AppErrorCode | null
}> {
  return authorizedGetList("/api/reports/recent")
}

/** `GET /api/initiatives/recent` */
export async function getRecentInitiatives(): Promise<{
  items: RecentUpdateItem[]
  error: AppErrorCode | null
}> {
  return authorizedGetList("/api/initiatives/recent")
}

/** `GET /api/collections/recent` */
export async function getRecentCollections(): Promise<{
  items: RecentUpdateItem[]
  error: AppErrorCode | null
}> {
  return authorizedGetList("/api/collections/recent")
}

export async function loadRecentUpdates(): Promise<RecentUpdatesLoadResult> {
  const [reports, initiatives, collections] = await Promise.all([
    getRecentReports(),
    getRecentInitiatives(),
    getRecentCollections(),
  ])

  const authError =
    reports.error === "auth_required" ||
    initiatives.error === "auth_required" ||
    collections.error === "auth_required"
      ? "auth_required"
      : null

  if (authError) {
    return { data: null, error: authError }
  }

  const data: RecentUpdatesBundle = {
    reports: toSection(reports.items, reports.error),
    initiatives: toSection(initiatives.items, initiatives.error),
    collections: toSection(collections.items, collections.error),
  }

  const allFailed =
    data.reports.failed && data.initiatives.failed && data.collections.failed
  if (allFailed) {
    return { data: null, error: reports.error ?? initiatives.error ?? collections.error ?? "unknown" }
  }

  return { data, error: null }
}
