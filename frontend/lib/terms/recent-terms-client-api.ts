import type { RecentTermsDto } from "@/lib/terms/types"

export type FetchRecentTermsResult =
  | { ok: true; data: RecentTermsDto }
  | { ok: false; error: "http_error" | "network"; status?: number }

/** Browser fetch for `GET /api/terms/recent` (Next rewrite → C#). */
export async function fetchRecentTerms(): Promise<FetchRecentTermsResult> {
  try {
    const response = await fetch("/api/terms/recent", {
      credentials: "include",
      cache: "no-store",
    })

    if (!response.ok) {
      return { ok: false, error: "http_error", status: response.status }
    }

    const data = (await response.json()) as RecentTermsDto
    return { ok: true, data }
  } catch {
    return { ok: false, error: "network" }
  }
}

export function normalizeRecentTerms(data: RecentTermsDto | null | undefined): {
  approved: RecentTermsDto["approved"]
  pending: RecentTermsDto["pending"]
} {
  return {
    approved: data?.approved ?? [],
    pending: data?.pending ?? [],
  }
}

export function formatRecentTermName(name: string): string {
  return name.replace(/_/g, " ")
}

export function termDetailHref(id: number): string {
  return `/terms?id=${id}`
}
