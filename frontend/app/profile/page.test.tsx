import { render, screen } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import type { ProfileAnalyticsData } from "@/app/profile/actions"
import type { AuthUser } from "@/lib/auth"

const { getTokenMock, getCurrentUserMock, loadProfileAnalyticsActionMock, redirectMock } =
  vi.hoisted(() => ({
    getTokenMock: vi.fn<() => Promise<string | null>>(),
    getCurrentUserMock: vi.fn<() => Promise<AuthUser | null>>(),
    loadProfileAnalyticsActionMock: vi.fn(),
    redirectMock: vi.fn((path: string) => {
      throw new Error(`NEXT_REDIRECT:${path}`)
    }),
  }))

vi.mock("@/lib/auth", () => ({
  getToken: getTokenMock,
  getCurrentUser: getCurrentUserMock,
}))

vi.mock("@/app/profile/actions", () => ({
  loadProfileAnalyticsAction: loadProfileAnalyticsActionMock,
}))

vi.mock("next/navigation", () => ({
  redirect: redirectMock,
}))

vi.mock("@/components/layout/library-shell", () => ({
  LibraryShell: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="library-shell">{children}</div>
  ),
}))

vi.mock("@/components/profile/profile-full-view", () => ({
  ProfileFullView: ({ type }: { type: string }) => (
    <div data-testid="profile-full-view">Profile view ({type})</div>
  ),
}))

import ProfilePage from "./page"

const stubUser: AuthUser = {
  username: "admin",
  fullname: "Demo Admin",
  userId: "1",
  roles: ["Administrator"],
  permissions: [],
  adminEnabled: true,
}

const emptyShell: ProfileAnalyticsData = {
  chart: { runs: 0, users: 0, runTime: 0, history: [] },
  users: [],
  reports: [],
  fails: [],
  runList: [],
  stars: [],
  subscriptions: [],
}

describe("ProfilePage", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getTokenMock.mockResolvedValue("token")
    getCurrentUserMock.mockResolvedValue(stubUser)
  })

  it("redirects unauthenticated users to login", async () => {
    getTokenMock.mockResolvedValueOnce(null)
    await expect(
      ProfilePage({ searchParams: Promise.resolve({ id: "16", type: "report" }) }),
    ).rejects.toThrow("NEXT_REDIRECT:/auth/login")
  })

  it.each(["term", "collection"] as const)(
    "renders analytics shell for %s profiles instead of the page error state",
    async (profileType) => {
      loadProfileAnalyticsActionMock.mockResolvedValueOnce({
        data: emptyShell,
        error: null,
      })

      const ui = await ProfilePage({
        searchParams: Promise.resolve({ id: "1", type: profileType }),
      })
      render(ui)

      expect(screen.queryByText(/unable to load profile/i)).toBeNull()
      expect(screen.getByTestId("profile-full-view")).toHaveTextContent(`Profile view (${profileType})`)
    },
  )

  it("shows the page error state when analytics cannot be loaded for report profiles", async () => {
    loadProfileAnalyticsActionMock.mockResolvedValueOnce({
      data: null,
      error: "not_found",
    })

    const ui = await ProfilePage({
      searchParams: Promise.resolve({ id: "999", type: "report" }),
    })
    render(ui)

    expect(screen.getByText(/unable to load profile/i)).toBeDefined()
    expect(screen.queryByTestId("profile-full-view")).toBeNull()
  })
})
