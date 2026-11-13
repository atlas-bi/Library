import { describe, expect, it } from "vitest"
import { buildStarToggleRequest } from "@/lib/interactions/star-toggle-request"

describe("buildStarToggleRequest", () => {
  it("builds a user star payload", () => {
    expect(buildStarToggleRequest("user", 42)).toEqual({ type: "user", id: 42 })
  })

  it("builds a group star payload", () => {
    expect(buildStarToggleRequest("group", 7)).toEqual({ type: "group", id: 7 })
  })

  it("builds a saved-search payload with id 0 and the query string", () => {
    expect(buildStarToggleRequest("search", 99, { search: "  census  " })).toEqual({
      type: "search",
      id: 0,
      search: "census",
    })
  })
})
