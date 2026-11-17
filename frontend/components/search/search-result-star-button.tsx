"use client"

import { StarToggleButton } from "@/components/interactions/star-toggle-button"
import type { InteractionEntityType } from "@/lib/interactions/types"

export function SearchResultStarButton({
  resultType,
  atlasId,
  initialStarred,
  initialCount = 0,
}: {
  resultType: "users" | "groups"
  atlasId: number
  initialStarred: boolean
  initialCount?: number
}) {
  const type: InteractionEntityType = resultType === "users" ? "user" : "group"

  return (
    <StarToggleButton
      type={type}
      id={atlasId}
      initialStarred={initialStarred}
      initialCount={initialCount}
      iconOnly
    />
  )
}
