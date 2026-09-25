import { render, screen } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import type { AuthUser } from "@/lib/auth"
import { metadata } from "./page"

// ── Hoist mock factories so they are available before vi.mock calls ─────────
const { getCurrentUserMock } = vi.hoisted(() => ({
  getCurrentUserMock: vi.fn<() => Promise<AuthUser | null>>(),
}))

// ── Module mocks ─────────────────────────────────────────────────────────────
vi.mock("@/lib/auth", () => ({
  getCurrentUser: getCurrentUserMock,
}))

vi.mock("@/components/layout/library-shell", () => ({
  LibraryShell: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="library-shell">{children}</div>
  ),
}))

// ── Import SUT after mocks are registered ────────────────────────────────────
import AboutAnalyticsPage from "./page"

// ── Helpers ──────────────────────────────────────────────────────────────────
const stubUser: AuthUser = {
  username: "jdoe",
  fullname: "Jane Doe",
  userId: "42",
  roles: ["Report Writer"],
  permissions: [],
  adminEnabled: false,
}

// ── Tests ─────────────────────────────────────────────────────────────────────
describe("AboutAnalyticsPage", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("renders the library shell", async () => {
    getCurrentUserMock.mockResolvedValue(null)
    render(await AboutAnalyticsPage())
    expect(screen.getByTestId("library-shell")).toBeInTheDocument()
  })

  it("renders the page heading", async () => {
    getCurrentUserMock.mockResolvedValue(null)
    render(await AboutAnalyticsPage())
    expect(
      screen.getByRole("heading", { level: 1, name: "Analytics Cheat Sheet" }),
    ).toBeInTheDocument()
  })

  it("renders the Data Sources section", async () => {
    getCurrentUserMock.mockResolvedValue(null)
    render(await AboutAnalyticsPage())
    expect(screen.getByRole("heading", { name: "Data Sources" })).toBeInTheDocument()
    // Each name appears in both an <h3> and the body text, so use getAllByText
    expect(screen.getAllByText(/Chronicles/)[0]).toBeInTheDocument()
    expect(screen.getAllByText(/Clarity/)[0]).toBeInTheDocument()
    expect(screen.getAllByText(/Caboodle/)[0]).toBeInTheDocument()
  })

  it("renders the Reporting Tools section", async () => {
    getCurrentUserMock.mockResolvedValue(null)
    render(await AboutAnalyticsPage())
    expect(screen.getByRole("heading", { name: "Reporting Tools" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Reporting Workbench" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Tableau" })).toBeInTheDocument()
  })

  it("renders the Other Tools section", async () => {
    getCurrentUserMock.mockResolvedValue(null)
    render(await AboutAnalyticsPage())
    expect(screen.getByRole("heading", { name: "Other Tools" })).toBeInTheDocument()
    // Both tool names appear as a <span> inside a <li>, so multiple nodes match
    expect(screen.getAllByText(/Cobalt/)[0]).toBeInTheDocument()
    expect(screen.getAllByText(/SlicerDicer/)[0]).toBeInTheDocument()
  })

  it("renders the Validation Tags section", async () => {
    getCurrentUserMock.mockResolvedValue(null)
    render(await AboutAnalyticsPage())
    expect(screen.getByRole("heading", { name: "Validation Tags" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Analytics Certified" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "High Risk" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Self-Service" })).toBeInTheDocument()
  })

  it("renders for an authenticated user without throwing", async () => {
    getCurrentUserMock.mockResolvedValue(stubUser)
    render(await AboutAnalyticsPage())
    expect(screen.getByTestId("library-shell")).toBeInTheDocument()
  })

  it("exports the correct page metadata title", () => {
    // metadata is a statically imported named export — no dynamic require needed
    expect(metadata.title).toBe("Analytics Cheat Sheet")
  })
})
