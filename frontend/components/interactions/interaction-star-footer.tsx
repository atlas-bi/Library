"use client"

import { StarToggleButton } from "@/components/interactions/star-toggle-button"
import type { InteractionStarTarget } from "@/lib/interactions/resolve-star-target"

export function InteractionStarFooter({ target }: { target: InteractionStarTarget }) {
  return (
    <div className="grid border-t border-[var(--atlas-home-border-soft)] grid-cols-1">
      <StarToggleButton
        type={target.type}
        id={target.type === "search" ? 0 : target.id}
        searchQuery={target.searchQuery}
        initialStarred={target.isStarred ?? false}
        initialCount={target.starCount ?? 0}
        variant="card-footer"
      />
    </div>
  )
}
