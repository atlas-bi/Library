import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { loadAnalyticsDashboardAction } from "@/app/analytics/actions"
import type { AnalyticsDashboardData } from "@/lib/analytics/types"
import { AnalyticsDashboard } from "./analytics-dashboard"

const replaceMock = vi.fn()

vi.mock("next/navigation", () => ({
  useRouter: () => ({ replace: replaceMock }),
}))

vi.mock("next/dynamic", () => ({
  default: () => () => null,
}))

vi.mock("@/app/analytics/actions", () => ({
  loadAnalyticsDashboardAction: vi.fn(),
  refreshAnalyticsLiveUsersAction: vi.fn(),
}))

const loadMock = vi.mocked(loadAnalyticsDashboardAction)

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
  beforeEach(() => {
    vi.clearAllMocks()
    loadMock.mockResolvedValue({ data: stubDashboard, error: null })
  })

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

  it("shows forbidden messaging when the API denies access", () => {
    render(
      <AnalyticsDashboard
        initialData={null}
        initialFilters={{ rangeId: "2", tracePage: 0, errorPage: 0 }}
        loadError="forbidden"
      />,
    )

    expect(
      screen.getByText("You do not have permission to view this content."),
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

  it("shows empty active-users copy when nobody is live", () => {
    render(
      <AnalyticsDashboard
        initialData={stubDashboard}
        initialFilters={{ rangeId: "2", tracePage: 0, errorPage: 0 }}
        loadError={null}
      />,
    )

    expect(screen.getByText(/No active sessions in the last minute/i)).toBeInTheDocument()
  })

  it("reloads dashboard data when the date range changes", async () => {
    const user = userEvent.setup()

    render(
      <AnalyticsDashboard
        initialData={stubDashboard}
        initialFilters={{ rangeId: "2", tracePage: 0, errorPage: 0 }}
        loadError={null}
      />,
    )

    await user.click(screen.getByRole("button", { name: /Last 24 hours/i }))
    await user.click(screen.getByRole("option", { name: "Last 7 days" }))

    await waitFor(() => {
      expect(loadMock).toHaveBeenCalledWith(
        expect.objectContaining({ rangeId: "4", tracePage: 0, errorPage: 0 }),
      )
    })
    expect(replaceMock).toHaveBeenCalledWith("/analytics?range=4", { scroll: false })
  })

  it("keeps existing dashboard visible when a reload fails", async () => {
    loadMock.mockResolvedValueOnce({ data: null, error: "forbidden" })
    const user = userEvent.setup()

    render(
      <AnalyticsDashboard
        initialData={stubDashboard}
        initialFilters={{ rangeId: "2", tracePage: 0, errorPage: 0 }}
        loadError={null}
      />,
    )

    await user.click(screen.getByRole("button", { name: /Last 24 hours/i }))
    await user.click(screen.getByRole("option", { name: "Today" }))

    await waitFor(() => {
      expect(
        screen.getByText("You do not have permission to view this content."),
      ).toBeInTheDocument()
    })
    expect(screen.getByText("Views")).toBeInTheDocument()
  })
})
