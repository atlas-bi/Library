import { describe, expect, it } from "vitest"
import { getAnalyticsRangeParams } from "./date-ranges"

describe("getAnalyticsRangeParams", () => {
  it("defaults to last 24 hours", () => {
    const params = getAnalyticsRangeParams("2")
    expect(params.startAt).toBeLessThan(0)
    expect(params.endAt).toBe(0)
  })

  it("returns bounded range for today", () => {
    const params = getAnalyticsRangeParams("1")
    expect(params.startAt).toBeLessThanOrEqual(0)
    expect(params.endAt).toBeGreaterThanOrEqual(0)
  })
})
