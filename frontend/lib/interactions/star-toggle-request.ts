import type { InteractionEntityType, StarToggleRequest } from "@/lib/interactions/types"

/** Builds the POST body for `/api/interactions/stars/toggle`. */
export function buildStarToggleRequest(
  type: InteractionEntityType,
  id: number,
  options?: { search?: string },
): StarToggleRequest {
  if (type === "search") {
    return {
      type: "search",
      id: 0,
      search: options?.search?.trim() ?? "",
    }
  }

  return { type, id }
}
