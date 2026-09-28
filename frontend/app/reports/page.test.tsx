import { render, screen } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import type { AuthUser } from "@/lib/auth"

const { getTokenMock, getCurrentUserMock, getReportsListMock, getReportDetailByIdMock, redirectMock } =
  vi.hoisted(() => ({
    getTokenMock: vi.fn<() => Promise<string | null>>(),
    getCurrentUserMock: vi.fn<() => Promise<AuthUser | null>>(),
    getReportsListMock: vi.fn(),
    getReportDetailByIdMock: vi.fn(),
    redirectMock: vi.fn((path: string) => {
      throw new Error(`NEXT_REDIRECT:${path}`)
    }),
  }))

vi.mock("@/lib/auth", () => ({
  getToken: getTokenMock,
  getCurrentUser: getCurrentUserMock,
}))

vi.mock("@/lib/reports/api", () => ({
  getReportsList: getReportsListMock,
  getReportDetailById: getReportDetailByIdMock,
}))

vi.mock("next/navigation", () => ({
  redirect: redirectMock,
}))

vi.mock("@/components/layout/library-shell", () => ({
  LibraryShell: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="library-shell">{children}</div>
  ),
}))

vi.mock("@/components/reports/reports-list-table", () => ({
  ReportsListTable: ({ reports }: { reports: { id: number; name: string }[] }) => (
    <div data-testid="reports-list-table">{reports.map((report) => report.name).join(",")}</div>
  ),
}))

import ReportsPage from "./page"

const stubUser: AuthUser = {
  username: "jdoe",
  fullname: "Jane Doe",
  userId: "42",
  roles: ["Report Writer"],
  permissions: [],
  adminEnabled: false,
}

describe("ReportsPage", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getTokenMock.mockResolvedValue("token")
    getCurrentUserMock.mockResolvedValue(stubUser)
  })

  it("redirects unauthenticated users to login", async () => {
    getTokenMock.mockResolvedValue(null)

    await expect(
      ReportsPage({ searchParams: Promise.resolve({}) }),
    ).rejects.toThrow("NEXT_REDIRECT:/auth/login")
    expect(redirectMock).toHaveBeenCalledWith("/auth/login")
  })

  it("renders the reports index when no id is provided", async () => {
    getReportsListMock.mockResolvedValueOnce({
      data: {
        reports: [{ id: 16, name: "Daily Emergency Department Census", type: "SSRS" }],
        total: 1,
        page: 1,
        pageSize: 20,
      },
      error: null,
    })

    render(await ReportsPage({ searchParams: Promise.resolve({}) }))

    expect(screen.getByRole("heading", { level: 1, name: "Reports" })).toBeInTheDocument()
    expect(screen.getByText("Browse documented reports in the library.")).toBeInTheDocument()
    expect(screen.getByTestId("reports-list-table")).toHaveTextContent(
      "Daily Emergency Department Census",
    )
    expect(getReportsListMock).toHaveBeenCalledWith(1, 20)
    expect(getReportDetailByIdMock).not.toHaveBeenCalled()
  })

  it("shows an error state when the reports list fails to load", async () => {
    getReportsListMock.mockResolvedValueOnce({
      data: null,
      error: "server_error",
    })

    render(await ReportsPage({ searchParams: Promise.resolve({}) }))

    expect(screen.getByText("Unable to load reports")).toBeInTheDocument()
    expect(
      screen.getByText("We hit a server issue while processing your request. Please try again."),
    ).toBeInTheDocument()
  })

  it("shows invalid id messaging with a link back to the index", async () => {
    render(await ReportsPage({ searchParams: Promise.resolve({ id: "not-a-number" }) }))

    expect(screen.getByRole("heading", { name: "Report not found" })).toBeInTheDocument()
    expect(screen.getByText("Missing or invalid report id.")).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "Back to reports" })).toHaveAttribute("href", "/reports")
    expect(getReportsListMock).not.toHaveBeenCalled()
  })
})
