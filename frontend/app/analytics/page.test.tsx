import { render, screen } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import type { AnalyticsDashboardData } from "@/lib/analytics/types"
import type { AuthUser } from "@/lib/auth"

const { getTokenMock, getCurrentUserMock, loadAnalyticsDashboardActionMock, redirectMock } =
  vi.hoisted(() => ({
    getTokenMock: vi.fn<() => Promise<string | null>>(),
    getCurrentUserMock: vi.fn<() => Promise<AuthUser | null>>(),
    loadAnalyticsDashboardActionMock: vi.fn(),
    redirectMock: vi.fn((path: string) => {
      throw new Error(`NEXT_REDIRECT:${path}`)
    }),
  }))

vi.mock("@/lib/auth", () => ({
  getToken: getTokenMock,
  getCurrentUser: getCurrentUserMock,
}))

vi.mock("@/app/analytics/actions", () => ({
  loadAnalyticsDashboardAction: loadAnalyticsDashboardActionMock,
  refreshAnalyticsLiveUsersAction: vi.fn(),
  setAnalyticsTraceResolvedAction: vi.fn(),
  setAnalyticsErrorResolvedAction: vi.fn(),
}))

vi.mock("next/navigation", () => ({
  redirect: redirectMock,
  useRouter: () => ({ replace: vi.fn() }),
}))

vi.mock("@/components/layout/library-shell", () => ({
  LibraryShell: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="library-shell">{children}</div>
  ),
}))

vi.mock("chart.js", () => ({
  default: vi.fn().mockImplementation(() => ({ destroy: vi.fn() })),
}))

import AnalyticsPage from "./page"

const stubUser: AuthUser = {
  username: "admin",
  fullname: "Demo Admin",
  userId: "1",
  roles: ["Administrator"],
  permissions: [],
  adminEnabled: true,
}

const stubDashboard: AnalyticsDashboardData = {
  visits: { views: 120, visitors: 45, loadTime: 1.4, accessHistory: [] },
  browsers: [],
  os: [],
  resolution: [],
  users: [],
  loadTimes: [],
  liveUsers: { activeUsers: 0, items: [] },
  traces: { pages: 1, currentPage: 1, totalCount: 0, unresolvedCount: 0, items: [] },
  errors: { pages: 1, currentPage: 1, totalCount: 0, unresolvedCount: 0, items: [] },
}

describe("AnalyticsPage", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getTokenMock.mockResolvedValue("token")
    getCurrentUserMock.mockResolvedValue(stubUser)
    loadAnalyticsDashboardActionMock.mockResolvedValue({ data: stubDashboard, error: null })
  })

  it("redirects unauthenticated users to login", async () => {
    getTokenMock.mockResolvedValueOnce(null)
    await expect(AnalyticsPage({ searchParams: Promise.resolve({}) })).rejects.toThrow(
      "NEXT_REDIRECT:/auth/login",
    )
  })

  it("renders analytics heading and summary stats", async () => {
    render(await AnalyticsPage({ searchParams: Promise.resolve({}) }))
    expect(screen.getByRole("heading", { name: "Analytics", level: 1 })).toBeInTheDocument()
    expect(screen.getByText("Views")).toBeInTheDocument()
    expect(screen.getByText("Visitors")).toBeInTheDocument()
    expect(screen.getByText("Load Time")).toBeInTheDocument()
  })
})
