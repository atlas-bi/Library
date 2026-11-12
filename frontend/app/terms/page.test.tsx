import { render, screen } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import type { AuthUser } from "@/lib/auth"

const { getTokenMock, getCurrentUserMock, getTermsListMock, getTermByIdMock, getTermReportsMock, redirectMock } =
  vi.hoisted(() => ({
    getTokenMock: vi.fn<() => Promise<string | null>>(),
    getCurrentUserMock: vi.fn<() => Promise<AuthUser | null>>(),
    getTermsListMock: vi.fn(),
    getTermByIdMock: vi.fn(),
    getTermReportsMock: vi.fn(),
    redirectMock: vi.fn((path: string) => {
      throw new Error(`NEXT_REDIRECT:${path}`)
    }),
  }))

vi.mock("@/lib/auth", () => ({
  getToken: getTokenMock,
  getCurrentUser: getCurrentUserMock,
}))

vi.mock("@/lib/terms/api", () => ({
  getTermsList: getTermsListMock,
  getTermById: getTermByIdMock,
  getTermReports: getTermReportsMock,
}))

vi.mock("next/navigation", () => ({
  redirect: redirectMock,
}))

vi.mock("@/components/layout/library-shell", () => ({
  LibraryShell: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="library-shell">{children}</div>
  ),
}))

vi.mock("@/components/terms/terms-list-card", () => ({
  TermsListCard: ({ term }: { term: { id: number; name: string } }) => (
    <a href={`/terms?id=${term.id}`}>{term.name}</a>
  ),
}))

vi.mock("@/components/terms/term-action-rail", () => ({
  TermActionRail: () => <div data-testid="term-action-rail" />,
}))

vi.mock("@/components/terms/term-section-nav", () => ({
  TermSectionNav: () => null,
}))

vi.mock("@/components/terms/term-metadata-table", () => ({
  TermMetadataTable: () => <div data-testid="term-metadata" />,
}))

vi.mock("@/components/profile/profile-analytics-panel", () => ({
  ProfileAnalyticsPanel: () => null,
}))

import TermsPage from "./page"

const stubUser: AuthUser = {
  username: "admin",
  fullname: "Demo Admin",
  userId: "1",
  roles: ["Administrator"],
  permissions: [],
  adminEnabled: false,
}

describe("TermsPage", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getTokenMock.mockResolvedValue("token")
    getCurrentUserMock.mockResolvedValue(stubUser)
  })

  it("renders the terms list when id is omitted", async () => {
    getTermsListMock.mockResolvedValue({
      data: {
        items: [{ id: 1, name: "Average Length of Stay" }],
        permissions: { canCreateTerm: true },
      },
      error: null,
    })

    render(await TermsPage({ searchParams: Promise.resolve({}) }))

    expect(screen.getByRole("heading", { level: 1, name: "Terms" })).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "Average Length of Stay" })).toHaveAttribute(
      "href",
      "/terms?id=1",
    )
    expect(getTermByIdMock).not.toHaveBeenCalled()
  })

  it("renders term detail for /terms?id=", async () => {
    getTermByIdMock.mockResolvedValue({
      data: { id: 1, name: "Average Length of Stay", summary: "How long patients stay." },
      error: null,
    })
    getTermReportsMock.mockResolvedValue({ data: [], error: null })

    render(await TermsPage({ searchParams: Promise.resolve({ id: "1" }) }))

    expect(screen.getByRole("heading", { level: 1, name: "Average Length of Stay" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Summary" })).toBeInTheDocument()
    expect(screen.getByText("How long patients stay.")).toBeInTheDocument()
    expect(getTermsListMock).not.toHaveBeenCalled()
  })
})
