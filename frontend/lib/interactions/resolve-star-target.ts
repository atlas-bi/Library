import type { InteractionEntityType } from "@/lib/interactions/types"

export type InteractionStarTarget = {
  type: InteractionEntityType
  id: number
  searchQuery?: string
  isStarred?: boolean
  starCount?: number
}

export function resolveInteractionStarTarget(input: {
  itemType?: string | null
  typeLabel?: string | null
  id: number
  searchQuery?: string | null
  searchString?: string | null
  name?: string | null
  isStarred?: boolean
  starCount?: number
}): InteractionStarTarget | null {
  const kind =
    input.itemType?.trim().toLowerCase() ||
    input.typeLabel?.trim().toLowerCase() ||
    ""

  const base = {
    isStarred: input.isStarred,
    starCount: input.starCount,
  }

  if (kind === "report" || kind === "collection" || kind === "term" || kind === "initiative") {
    return { type: kind as InteractionEntityType, id: input.id, ...base }
  }

  if (kind === "user" || kind === "group") {
    return { type: kind, id: input.id, ...base }
  }

  if (kind === "search") {
    const query =
      input.searchQuery?.trim() ||
      input.searchString?.trim() ||
      input.name?.trim() ||
      ""
    if (!query) return null
    return { type: "search", id: 0, searchQuery: query, ...base }
  }

  return null
}
