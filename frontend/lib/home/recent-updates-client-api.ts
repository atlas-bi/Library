import type { RecentUpdateItem } from "@/lib/home/recent-updates-types"

export type RecentUpdatesEndpoint = "reports" | "initiatives" | "collections"

const ENDPOINT_PATH: Record<RecentUpdatesEndpoint, string> = {
  reports: "/api/reports/recent",
  initiatives: "/api/initiatives/recent",
  collections: "/api/collections/recent",
}

export type FetchRecentUpdatesListResult =
  | { ok: true; data: RecentUpdateItem[] }
  | { ok: false; error: "http_error" | "network"; status?: number }

/** Browser fetch for recent-update list routes (Next rewrite → C#). */
export async function fetchRecentUpdatesList(
  endpoint: RecentUpdatesEndpoint,
): Promise<FetchRecentUpdatesListResult> {
  try {
    const response = await fetch(ENDPOINT_PATH[endpoint], {
      credentials: "include",
      cache: "no-store",
    })

    if (!response.ok) {
      return { ok: false, error: "http_error", status: response.status }
    }

    const data = (await response.json()) as RecentUpdateItem[]
    return { ok: true, data: Array.isArray(data) ? data : [] }
  } catch {
    return { ok: false, error: "network" }
  }
}

export function reportDetailHref(id: number): string {
  return `/reports?id=${id}`
}

export function initiativeDetailHref(id: number): string {
  return `/initiatives?id=${id}`
}

export function collectionDetailHref(id: number): string {
  return `/collections?id=${id}`
}

/** Match Razor recent-update partials and the Recent terms sidebar (#895). */
export function formatRecentUpdateName(name: string): string {
  return name.replace(/_/g, " ")
}
