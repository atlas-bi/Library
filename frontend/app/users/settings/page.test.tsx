import { render, screen } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import type { AuthUser } from "@/lib/auth"
import { metadata } from "./page"

const { getTokenMock, getCurrentUserMock, getUserSettingsMock, redirectMock } = vi.hoisted(() => ({
  getTokenMock: vi.fn<() => Promise<string | null>>(),
  getCurrentUserMock: vi.fn<() => Promise<AuthUser | null>>(),
  getUserSettingsMock: vi.fn(),
  redirectMock: vi.fn((path: string) => {
    throw new Error(`NEXT_REDIRECT:${path}`)
  }),
}))

vi.mock("@/lib/auth", () => ({
  getToken: getTokenMock,
  getCurrentUser: getCurrentUserMock,
}))

vi.mock("@/lib/users/api", () => ({
  getUserSettings: getUserSettingsMock,
}))

vi.mock("next/navigation", () => ({
  redirect: redirectMock,
}))

vi.mock("@/components/layout/library-shell", () => ({
  LibraryShell: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="library-shell">{children}</div>
  ),
}))

vi.mock("@/components/users/user-settings-panel", () => ({
  UserSettingsPanel: ({
    initialShareNotificationEnabled,
  }: {
    initialShareNotificationEnabled: boolean
  }) => (
    <div data-testid="user-settings-panel">
      shareNotificationEnabled={String(initialShareNotificationEnabled)}
    </div>
  ),
}))

import UserSettingsPage from "./page"

const stubUser: AuthUser = {
  username: "jdoe",
  fullname: "Jane Doe",
  userId: "42",
  roles: ["Report Writer"],
  permissions: [],
  adminEnabled: false,
}

describe("UserSettingsPage", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getTokenMock.mockResolvedValue("token")
    getCurrentUserMock.mockResolvedValue(stubUser)
  })

  it("redirects unauthenticated users to login", async () => {
    getTokenMock.mockResolvedValue(null)

    await expect(UserSettingsPage()).rejects.toThrow("NEXT_REDIRECT:/auth/login")
    expect(redirectMock).toHaveBeenCalledWith("/auth/login")
  })

  it("renders the settings panel when settings load successfully", async () => {
    getUserSettingsMock.mockResolvedValueOnce({
      data: { shareNotificationEnabled: true },
      error: null,
    })

    render(await UserSettingsPage())

    expect(screen.getByTestId("library-shell")).toBeInTheDocument()
    expect(screen.getByRole("heading", { level: 1, name: "Settings" })).toBeInTheDocument()
    expect(screen.getByTestId("user-settings-panel")).toHaveTextContent(
      "shareNotificationEnabled=true",
    )
  })

  it("renders disabled settings when share notifications are off", async () => {
    getUserSettingsMock.mockResolvedValueOnce({
      data: { shareNotificationEnabled: false },
      error: null,
    })

    render(await UserSettingsPage())

    expect(screen.getByTestId("user-settings-panel")).toHaveTextContent(
      "shareNotificationEnabled=false",
    )
  })

  it("shows an API error when settings fail to load", async () => {
    getUserSettingsMock.mockResolvedValueOnce({
      data: null,
      error: "server_error",
    })

    render(await UserSettingsPage())

    expect(
      screen.getByText("We hit a server issue while processing your request. Please try again."),
    ).toBeInTheDocument()
    expect(screen.queryByTestId("user-settings-panel")).not.toBeInTheDocument()
  })

  it("exports the correct page metadata title", () => {
    expect(metadata.title).toBe("Settings")
  })
})
