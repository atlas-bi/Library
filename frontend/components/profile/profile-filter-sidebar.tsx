"use client"

import { Filter } from "lucide-react"
import { ProfileDateRangeSelect } from "@/components/profile/profile-date-range-select"
import type { ProfileDateRangeId } from "@/lib/profile/date-ranges"
import { cn } from "@/lib/utils"

export function ProfileFilterSidebar({
  dateRangeId,
  dateRangeOptions,
  onDateRangeChange,
  className,
}: {
  dateRangeId: ProfileDateRangeId
  dateRangeOptions: Array<{ id: ProfileDateRangeId; label: string }>
  onDateRangeChange: (id: ProfileDateRangeId) => void
  className?: string
}) {
  return (
    <aside
      className={cn(
        "sticky top-0 max-h-[calc(100vh-8rem)] space-y-4 overflow-y-auto",
        className,
      )}
      aria-label="Profile filters"
    >
      <div className="flex items-center gap-2 text-sm font-semibold">
        <Filter className="size-4 text-muted-foreground" aria-hidden="true" />
        <span>Filter Profile</span>
      </div>

    </aside>
  )
}
