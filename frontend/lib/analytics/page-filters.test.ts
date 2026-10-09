import { describe, expect, it } from "vitest"
import { buildAnalyticsUrlQuery, buildInitialAnalyticsFilters } from "./page-filters"

describe("buildInitialAnalyticsFilters", () => {
  it("parses user, group, range, and log pages from search params", () => {
    expect(
      buildInitialAnalyticsFilters({
        userId: "12",
        groupId: "3",
        range: "4",
        tracePage: "2",
        errorPage: "1",
      }),
    ).toEqual({
      rangeId: "4",
      userId: 12,
      groupId: 3,
      tracePage: 2,
      errorPage: 1,
    })
  })
})

describe("buildAnalyticsUrlQuery", () => {
  it("keeps scoped filters and non-default range in the query string", () => {
    expect(
      buildAnalyticsUrlQuery({
        rangeId: "4",
        userId: 12,
        groupId: undefined,
        tracePage: 0,
        errorPage: 0,
      }),
    ).toBe("?userId=12&range=4")
  })
})
