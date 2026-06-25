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
        "sticky top-0 max-h-[calc(100vh-8rem)] space-y-4 overflow-y-auto rounded-md border bg-card/60 p-4",
        className,
      )}
      aria-label="Profile filters"
    >
      <div className="flex items-center gap-2 text-sm font-semibold">
        <Filter className="size-4 text-muted-foreground" aria-hidden="true" />
        <span>Filter Profile</span>
      </div>

      <div className="space-y-2">
        <div className="text-xs font-semibold tracking-wide text-muted-foreground uppercase">
          Date range
        </div>
        <ProfileDateRangeSelect
          value={dateRangeId}
          options={dateRangeOptions}
          onChange={onDateRangeChange}
          className="w-full flex-col items-stretch gap-2"
        />
        <ul className="space-y-1">
          {dateRangeOptions.map((option) => (
            <li key={option.id}>
              <button
                type="button"
                onClick={() => onDateRangeChange(option.id)}
                className={cn(
                  "w-full rounded-md px-2 py-1.5 text-left text-sm transition-colors",
                  dateRangeId === option.id
                    ? "bg-primary/10 font-medium text-foreground"
                    : "text-muted-foreground hover:bg-muted/60 hover:text-foreground",
                )}
              >
                {option.label}
              </button>
            </li>
          ))}
        </ul>
      </div>
    </aside>
  )
}
