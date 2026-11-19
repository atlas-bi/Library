import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import {
  ScopedSiteAnalyticsCta,
  buildScopedAnalyticsHref,
} from "@/components/analytics/scoped-site-analytics-cta"

describe("buildScopedAnalyticsHref", () => {
  it("builds user scope links", () => {
    expect(buildScopedAnalyticsHref("user", 2)).toBe("/analytics?userId=2")
  })

  it("builds group scope links", () => {
    expect(buildScopedAnalyticsHref("group", 5)).toBe("/analytics?groupId=5")
  })
})

describe("ScopedSiteAnalyticsCta", () => {
  it("renders a scoped link for users", () => {
    render(<ScopedSiteAnalyticsCta scope="user" id={2} />)
    const link = screen.getByRole("link", { name: "Open scoped analytics" })
    expect(link).toHaveAttribute("href", "/analytics?userId=2")
    expect(screen.getByText(/this user/i)).toBeInTheDocument()
  })

  it("renders a scoped link for groups", () => {
    render(<ScopedSiteAnalyticsCta scope="group" id={3} />)
    const link = screen.getByRole("link", { name: "Open scoped analytics" })
    expect(link).toHaveAttribute("href", "/analytics?groupId=3")
    expect(screen.getByText(/this group/i)).toBeInTheDocument()
  })
})