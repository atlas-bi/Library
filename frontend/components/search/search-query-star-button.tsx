"use client"

import { StarToggleButton } from "@/components/interactions/star-toggle-button"

export function SearchQueryStarButton({
  query,
  initialStarred = false,
  initialCount = 0,
}: {
  query: string
  initialStarred?: boolean
  initialCount?: number
}) {
  const trimmed = query.trim()
  if (!trimmed) return null

  return (
    <StarToggleButton
      type="search"
      id={0}
      searchQuery={trimmed}
      initialStarred={initialStarred}
      initialCount={initialCount}
    />
  )
}
