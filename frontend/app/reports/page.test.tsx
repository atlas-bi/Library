import { render, screen } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import type { AuthUser } from "@/lib/auth"
import type { ReportDetail } from "@/lib/reports/types"

const { getTokenMock, getCurrentUserMock, getReportDetailByIdMock, redirectMock } = vi.hoisted(() => ({
  getTokenMock: vi.fn<() => Promise<string | null>>(),
  getCurrentUserMock: vi.fn<() => Promise<AuthUser | null>>(),
  getReportDetailByIdMock: vi.fn(),
  redirectMock: vi.fn((path: string) => {
    throw new Error(`NEXT_REDIRECT:${path}`)
  }),
}))

vi.mock("@/lib/auth", () => ({
  getToken: getTokenMock,
  getCurrentUser: getCurrentUserMock,
}))

const { getReportsListMock } = vi.hoisted(() => ({
  getReportsListMock: vi.fn(),
}))

vi.mock("@/lib/reports/api", () => ({
  getReportDetailById: getReportDetailByIdMock,
  getReportsList: getReportsListMock,
}))

vi.mock("next/navigation", () => ({
  redirect: redirectMock,
  useRouter: () => ({ push: vi.fn(), refresh: vi.fn(), replace: vi.fn() }),
}))

vi.mock("next/image", () => ({
  default: (props: { alt?: string }) => <img alt={props.alt ?? ""} />,
}))

vi.mock("@/components/layout/library-shell", () => ({
  LibraryShell: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="library-shell">{children}</div>
  ),
}))

vi.mock("@/components/reports/report-action-rail", () => ({
  ReportActionRail: () => <div data-testid="report-action-rail" />,
}))

vi.mock("@/components/reports/report-relationships-section", () => ({
  ReportRelationshipsSection: () => <section id="relationships"><h2>Relationships</h2></section>,
}))

vi.mock("@/components/profile/profile-analytics-panel", () => ({
  ProfileAnalyticsPanel: () => null,
}))

import ReportsPage from "./page"

const stubUser: AuthUser = {
  username: "admin",
  fullname: "Demo Admin",
  userId: "1",
  roles: ["Administrator"],
  permissions: [],
  adminEnabled: false,
}

const report16: ReportDetail = {
  id: 16,
  name: "daily_emergency_department_census",
  displayTitle: "Daily Emergency Department Census",
  description: "System description",
  document: {
    developerDescription: "Developer description",
    maintenanceLogs: [{ id: 1, comment: "Checked metrics" }],
  },
  terms: [{ id: 1, name: "Average Length of Stay", summary: "ALOS" }],
  queries: [{ id: 1, name: "Main query", query: "SELECT 1" }],
  collections: [{ id: 1, name: "Patient Flow Command Center" }],
  features: { termsEnabled: true, userProfilesEnabled: true },
  maintenanceStatus: { isRequired: true, message: "Report requires maintenance." },
  canViewGroups: true,
}

describe("ReportsPage list view", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getTokenMock.mockResolvedValue("token")
    getCurrentUserMock.mockResolvedValue(stubUser)
  })

  it("renders the reports index when id is omitted", async () => {
    getReportsListMock.mockResolvedValue({
      data: {
        reports: [
          {
            id: 16,
            name: "daily_emergency_department_census",
            displayTitle: "Daily Emergency Department Census",
            type: "SSRS",
            lastModified: "2026-01-15T00:00:00Z",
          },
        ],
        total: 1,
        page: 1,
        pageSize: 20,
      },
      error: null,
    })

    render(
      await ReportsPage({
        searchParams: Promise.resolve({}),
      }),
    )

    expect(screen.getByRole("heading", { level: 1, name: "Reports" })).toBeInTheDocument()
    expect(screen.getByText("Browse documented reports in the library.")).toBeInTheDocument()
    expect(
      screen.getByRole("link", { name: "Daily Emergency Department Census" }),
    ).toHaveAttribute("href", "/reports?id=16")
    expect(getReportDetailByIdMock).not.toHaveBeenCalled()
    expect(getReportsListMock).toHaveBeenCalledWith(1, 20)
  })
})

describe("ReportsPage detail view", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getTokenMock.mockResolvedValue("token")
    getCurrentUserMock.mockResolvedValue(stubUser)
    getReportDetailByIdMock.mockResolvedValue({ data: report16, error: null })
  })

  it("renders report 16 sections and maintenance banner", async () => {
    render(
      await ReportsPage({
        searchParams: Promise.resolve({ id: "16" }),
      }),
    )

    expect(screen.getByRole("heading", { level: 1, name: "Daily Emergency Department Census" })).toBeInTheDocument()
    expect(screen.getByText("This report is past due for maintenance!")).toBeInTheDocument()
    expect(screen.getByRole("navigation", { name: "Report sections" })).toHaveTextContent("Description")
    expect(screen.getByRole("heading", { name: "Description" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Terms" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Details" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Query" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Relationships" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Maintenance" })).toBeInTheDocument()
  })
})
