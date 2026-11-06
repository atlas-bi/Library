"use client"

import { ChevronDown } from "lucide-react"
import { useEffect, useRef, useState } from "react"
import {
  ANALYTICS_RANGE_OPTIONS,
  type AnalyticsRangeId,
} from "@/lib/analytics/date-ranges"

export function AnalyticsRangeSelect({
  value,
  onChange,
  disabled,
}: {
  value: AnalyticsRangeId
  onChange: (rangeId: AnalyticsRangeId) => void
  disabled?: boolean
}) {
  const [open, setOpen] = useState(false)
  const rootRef = useRef<HTMLDivElement>(null)

  const label =
    ANALYTICS_RANGE_OPTIONS.find((option) => option.id === value)?.label ?? "Last 24 hours"

  useEffect(() => {
    const onDocClick = (event: MouseEvent) => {
      if (!rootRef.current?.contains(event.target as Node)) {
        setOpen(false)
      }
    }
    document.addEventListener("click", onDocClick)
    return () => document.removeEventListener("click", onDocClick)
  }, [])

  return (
    <div ref={rootRef} className={`relative mb-5 ${disabled ? "opacity-60" : ""}`}>
      <button
        type="button"
        className="inline-flex min-w-[11rem] items-center justify-between gap-2 rounded border border-[#dbdbdb] bg-white px-3 py-2 text-sm text-[#363636] shadow-sm hover:border-[#b5b5b5]"
        aria-haspopup="listbox"
        aria-expanded={open}
        disabled={disabled}
        onClick={() => setOpen((current) => !current)}
      >
        <span>{label}</span>
        <ChevronDown className="size-4 text-[#7a7a7a]" aria-hidden />
      </button>
      {open ? (
        <div
          role="listbox"
          className="absolute right-0 z-20 mt-1 min-w-[12rem] rounded-md border border-[#dbdbdb] bg-white py-1 shadow-lg"
        >
          {ANALYTICS_RANGE_OPTIONS.map((option) => (
            <button
              key={option.id}
              type="button"
              role="option"
              aria-selected={option.id === value}
              className={`block w-full px-4 py-2 text-left text-sm hover:bg-[#f5f5f5] ${
                option.id === value ? "bg-[#f5f5f5] font-medium" : "text-[#363636]"
              }`}
              onClick={() => {
                onChange(option.id)
                setOpen(false)
              }}
            >
              {option.label}
            </button>
          ))}
        </div>
      ) : null}
    </div>
  )
}
