import Link from "next/link"
import type { AnalyticsPageFilters } from "@/lib/analytics/types"

export function AnalyticsScopeNotice({ filters }: { filters: AnalyticsPageFilters }) {
  if (!filters.userId && !filters.groupId) {
    return null
  }

  const scopeLabel = filters.userId
    ? `user ID ${filters.userId}`
    : `group ID ${filters.groupId}`

  return (
    <div className="mb-4 rounded-md border border-[#3298dc]/30 bg-[#eff5fb] px-3 py-2 text-sm text-[#363636]">
      Showing analytics scoped to {scopeLabel}.{" "}
      <Link href="/analytics" className="text-[#3273dc] hover:underline">
        View all analytics
      </Link>
    </div>
  )
}
