import { describe, expect, it } from "vitest"
import { getDefaultUserTab } from "@/components/users/user-section-nav"

describe("getDefaultUserTab", () => {
  const allTabs = [
    "stars",
    "subscriptions",
    "groups",
    "activity",
    "run-list",
    "atlas-history",
    "analytics",
  ] as const

  it("opens stars for the current user", () => {
    expect(getDefaultUserTab(true, [...allTabs])).toBe("stars")
  })

  it("opens report runs for another user's profile", () => {
    expect(getDefaultUserTab(false, [...allTabs])).toBe("run-list")
  })

  it("falls back to the first visible tab when run-list is hidden", () => {
    expect(getDefaultUserTab(false, ["stars", "subscriptions", "activity"])).toBe("stars")
  })
})
