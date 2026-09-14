import { describe, it, expect } from "vitest"
import { getDefaultUserTab, type UserTabId } from "./user-section-nav"

describe("getDefaultUserTab", () => {
  const allTabs: UserTabId[] = [
    "stars",
    "subscriptions",
    "groups",
    "activity",
    "run-list",
    "atlas-history",
    "analytics",
  ]

  it("returns stars when user is viewing their own profile and stars tab is available", () => {
    const tab = getDefaultUserTab(true, allTabs)
    expect(tab).toBe("stars")
  })

  it("returns run-list when user is viewing another profile and run-list is available", () => {
    const tab = getDefaultUserTab(false, allTabs)
    expect(tab).toBe("run-list")
  })

  it("returns the first available tab if the preferred default is not available for current user", () => {
    const tabs: UserTabId[] = ["subscriptions", "groups", "activity"]
    const tab = getDefaultUserTab(true, tabs)
    expect(tab).toBe("subscriptions")
  })

  it("returns the first available tab if the preferred default is not available for other user", () => {
    const tabs: UserTabId[] = ["subscriptions", "groups", "activity"]
    const tab = getDefaultUserTab(false, tabs)
    expect(tab).toBe("subscriptions")
  })

  it("defaults to stars if tabs array is unexpectedly empty", () => {
    const tab = getDefaultUserTab(true, [])
    expect(tab).toBe("stars")
  })
})
