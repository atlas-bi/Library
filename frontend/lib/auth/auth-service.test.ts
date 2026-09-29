import { beforeEach, describe, expect, it, vi } from "vitest"
import type { AuthUser } from "./types"

const { cookiesMock, getServerApiBaseMock, apiFetchJsonMock } = vi.hoisted(() => ({
  cookiesMock: vi.fn(),
  getServerApiBaseMock: vi.fn(),
  apiFetchJsonMock: vi.fn(),
}))

vi.mock("next/headers", () => ({
  cookies: cookiesMock,
}))

vi.mock("@/lib/api-base", () => ({
  getServerApiBase: getServerApiBaseMock,
}))

vi.mock("@/lib/http", () => ({
  apiFetchJson: apiFetchJsonMock,
}))

import {
  getCurrentUser,
  getToken,
  hasAllPermissions,
  hasAnyPermission,
  hasAnyRole,
  hasPermission,
  isAdmin,
} from "./auth-service"

describe("auth-service", () => {
  const user: AuthUser = {
    username: "viewer",
    fullname: "Viewer Name",
    userId: "1",
    roles: ["Report Writer"],
    permissions: ["Create New Terms", "Edit Terms"],
    adminEnabled: false,
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("reads atlas_token from cookies", async () => {
    cookiesMock.mockResolvedValue({
      get: vi.fn(() => ({ value: "jwt-token" })),
    })

    await expect(getToken()).resolves.toBe("jwt-token")
  })

  it("returns null from getCurrentUser when token is missing", async () => {
    cookiesMock.mockResolvedValue({
      get: vi.fn(() => undefined),
    })

    await expect(getCurrentUser()).resolves.toBeNull()
    expect(apiFetchJsonMock).not.toHaveBeenCalled()
  })

  it("returns null from getCurrentUser when api base is unavailable", async () => {
    cookiesMock.mockResolvedValue({
      get: vi.fn(() => ({ value: "jwt-token" })),
    })
    getServerApiBaseMock.mockReturnValue("")

    await expect(getCurrentUser()).resolves.toBeNull()
    expect(apiFetchJsonMock).not.toHaveBeenCalled()
  })

  it("fetches the current user with the bearer token", async () => {
    cookiesMock.mockResolvedValue({
      get: vi.fn(() => ({ value: "jwt-token" })),
    })
    getServerApiBaseMock.mockReturnValue("https://api.example.test")
    apiFetchJsonMock.mockResolvedValue({ ok: true, data: user })

    await expect(getCurrentUser()).resolves.toEqual(user)
    expect(apiFetchJsonMock).toHaveBeenCalledWith("https://api.example.test/api/auth/me", {
      headers: { Authorization: "Bearer jwt-token" },
      cache: "no-store",
    })
  })

  it("treats admin users as having every permission", () => {
    expect(hasPermission({ ...user, adminEnabled: true }, "Delete Collection")).toBe(true)
  })

  it("checks permission and role helpers", () => {
    expect(hasPermission(user, "Create New Terms")).toBe(true)
    expect(hasAllPermissions(user, ["Create New Terms", "Edit Terms"])).toBe(true)
    expect(hasAnyPermission(user, ["Delete Collection", "Edit Terms"])).toBe(true)
    expect(hasAnyRole(user, ["Administrator", "Report Writer"])).toBe(true)
    expect(isAdmin(user)).toBe(false)
  })
})
