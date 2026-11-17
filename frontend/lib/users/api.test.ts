import { beforeEach, describe, expect, test, vi } from "vitest"

vi.mock("@/lib/auth", () => ({
  getToken: vi.fn(async () => "test-token"),
}))

vi.mock("@/lib/api-base", () => ({
  getServerApiBase: vi.fn(() => "https://api.example.test"),
}))

vi.mock("@/lib/http", () => ({
  apiFetchJson: vi.fn(),
}))

import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import { apiFetchJson } from "@/lib/http"
import { createUserFolder, getUserSettings, updateUserSettings } from "./api"

const BASE = "https://api.example.test"

function mockFetch(status: number, body: unknown = {}) {
  global.fetch = vi.fn(async () => {
    if (status === 204) {
      return new Response(null, { status: 204 })
    }
    return new Response(typeof body === "string" ? body : JSON.stringify(body), {
      status,
      headers: { "Content-Type": "application/json" },
    })
  }) as typeof fetch
}

describe("createUserFolder", () => {
  beforeEach(() => {
    vi.restoreAllMocks()
    vi.mocked(getToken).mockResolvedValue("test-token")
    vi.mocked(getServerApiBase).mockReturnValue(BASE)
    global.fetch = vi.fn(
      async () =>
        new Response(JSON.stringify({ id: 1, name: "Favorites" }), {
          status: 200,
          headers: { "Content-Type": "application/json" },
        }),
    ) as typeof fetch
  })

  test("uses the current-user folders endpoint when isCurrentUser is true", async () => {
    await createUserFolder(42, true, { name: "Favorites" })

    expect(global.fetch).toHaveBeenCalledWith(
      "https://api.example.test/api/users/me/folders",
      expect.objectContaining({ method: "POST" }),
    )
  })

  test("uses the target-user folders endpoint when isCurrentUser is false", async () => {
    await createUserFolder(42, false, { name: "Favorites" })

    expect(global.fetch).toHaveBeenCalledWith(
      "https://api.example.test/api/users/42/folders",
      expect.objectContaining({ method: "POST" }),
    )
  })
})

describe("user settings", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getToken).mockResolvedValue("test-token")
    vi.mocked(getServerApiBase).mockReturnValue(BASE)
  })

  describe("getUserSettings", () => {
    test("returns default-enabled state from GET", async () => {
      vi.mocked(apiFetchJson).mockResolvedValueOnce({
        ok: true,
        data: { shareNotificationEnabled: true },
      })

      const result = await getUserSettings()

      expect(result).toEqual({
        data: { shareNotificationEnabled: true },
        error: null,
      })
      expect(apiFetchJson).toHaveBeenCalledWith(`${BASE}/api/users/me/settings`, {
        headers: { Authorization: "Bearer test-token" },
        cache: "no-store",
      })
    })

    test("returns disabled state from GET", async () => {
      vi.mocked(apiFetchJson).mockResolvedValueOnce({
        ok: true,
        data: { shareNotificationEnabled: false },
      })

      const result = await getUserSettings()

      expect(result.data?.shareNotificationEnabled).toBe(false)
    })

    test("returns auth_required when token is missing", async () => {
      vi.mocked(getToken).mockResolvedValue(null)

      const result = await getUserSettings()

      expect(result).toEqual({ data: null, error: "auth_required" })
      expect(apiFetchJson).not.toHaveBeenCalled()
    })
  })

  describe("updateUserSettings", () => {
    test("enables share notifications via PUT", async () => {
      mockFetch(204)

      const result = await updateUserSettings({ shareNotificationEnabled: true })

      expect(result).toEqual({ ok: true, data: undefined })
      expect(global.fetch).toHaveBeenCalledWith(
        `${BASE}/api/users/me/settings`,
        expect.objectContaining({
          method: "PUT",
          body: JSON.stringify({ shareNotificationEnabled: true }),
          headers: expect.objectContaining({
            Authorization: "Bearer test-token",
            "Content-Type": "application/json",
          }),
        }),
      )
    })

    test("disables share notifications via PUT", async () => {
      mockFetch(204)

      const result = await updateUserSettings({ shareNotificationEnabled: false })

      expect(result.ok).toBe(true)
      expect(global.fetch).toHaveBeenCalledWith(
        `${BASE}/api/users/me/settings`,
        expect.objectContaining({
          body: JSON.stringify({ shareNotificationEnabled: false }),
        }),
      )
    })

    test("returns auth_required when token is missing", async () => {
      vi.mocked(getToken).mockResolvedValue(null)

      const result = await updateUserSettings({ shareNotificationEnabled: true })

      expect(result).toEqual({
        ok: false,
        message: expect.stringMatching(/sign in/i),
        code: "auth_required",
      })
      expect(global.fetch).not.toHaveBeenCalled()
    })

    test("surfaces invalid payload errors from PUT", async () => {
      mockFetch(400, { error: "shareNotificationEnabled is required." })

      const result = await updateUserSettings({ shareNotificationEnabled: true })

      expect(result.ok).toBe(false)
      if (!result.ok) {
        expect(result.message).toBe("shareNotificationEnabled is required.")
        expect(result.code).toBe("bad_request")
      }
    })
  })
})
