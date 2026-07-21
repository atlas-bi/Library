"use client"

import type { ProfileDateRangeId } from "@/lib/profile/date-ranges"
import { cn } from "@/lib/utils"

export function ProfileDateRangeSelect({
  value,
  options,
  onChange,
  className,
}: {
  value: ProfileDateRangeId
  options: Array<{ id: ProfileDateRangeId; label: string }>
  onChange: (id: ProfileDateRangeId) => void
  className?: string
}) {
  const active = options.find((option) => option.id === value)

  return (
    <label className={cn("inline-flex items-center gap-2 text-sm", className)}>
      <span className="text-muted-foreground">Range</span>
      <select
        value={value}
        onChange={(event) => onChange(event.target.value as ProfileDateRangeId)}
        className="rounded-md border border-input bg-background px-3 py-1.5 text-sm shadow-xs outline-none focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50"
        aria-label="Profile date range"
      >
        {options.map((option) => (
          <option key={option.id} value={option.id}>
            {option.label}
          </option>
        ))}
      </select>
      {active ? <span className="sr-only">Selected range: {active.label}</span> : null}
    </label>
  )
}
