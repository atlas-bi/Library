import { describe, expect, it } from "vitest"
import { getHomeTabById, HOME_TABS } from "@/lib/home/constants"

describe("HOME_TABS", () => {
  it("locks the homepage tabs to the C# order", () => {
    expect(HOME_TABS.map((tab) => tab.id)).toEqual([
      "stars",
      "subscriptions",
      "report-runs",
      "groups",
    ])
  })

  it("can resolve a known tab by id", () => {
    expect(getHomeTabById("report-runs")?.label).toBe("Report Runs")
  })

  it("returns undefined for unknown tabs", () => {
    expect(getHomeTabById("missing")).toBeUndefined()
  })
})
