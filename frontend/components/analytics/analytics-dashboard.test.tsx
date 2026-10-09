import { render, screen } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"
import type { AnalyticsDashboardData } from "@/lib/analytics/types"
import { AnalyticsDashboard } from "./analytics-dashboard"

vi.mock("next/navigation", () => ({
  useRouter: () => ({ replace: vi.fn() }),
}))

vi.mock("next/dynamic", () => ({
  default: () => () => null,
}))

vi.mock("@/app/analytics/actions", () => ({
  loadAnalyticsDashboardAction: vi.fn(),
  refreshAnalyticsLiveUsersAction: vi.fn(),
}))

const stubDashboard: AnalyticsDashboardData = {
  visits: { views: 1, visitors: 1, loadTime: 1, accessHistory: [] },
  browsers: [],
  os: [],
  resolution: [],
  users: [],
  loadTimes: [],
  liveUsers: { activeUsers: 0, items: [] },
  traces: { pages: 1, currentPage: 1, totalCount: 0, unresolvedCount: 0, items: [] },
  errors: { pages: 1, currentPage: 1, totalCount: 0, unresolvedCount: 0, items: [] },
}

describe("AnalyticsDashboard", () => {
  it("shows service unavailable messaging for non-auth load errors", () => {
    render(
      <AnalyticsDashboard
        initialData={null}
        initialFilters={{ rangeId: "2", tracePage: 0, errorPage: 0 }}
        loadError="service_unavailable"
      />,
    )

    expect(
      screen.getByText("The service is temporarily unavailable. Please try again shortly."),
    ).toBeInTheDocument()
  })

  it("shows scope notice when user filter is active", () => {
    render(
      <AnalyticsDashboard
        initialData={stubDashboard}
        initialFilters={{ rangeId: "2", userId: 5, tracePage: 0, errorPage: 0 }}
        loadError={null}
      />,
    )

    expect(screen.getByText(/scoped to user ID 5/i)).toBeInTheDocument()
  })
})
