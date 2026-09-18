import { beforeEach, describe, expect, test, vi } from "vitest"
import { AppError } from "@/lib/app-error"

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
import {
  addSiteMessage,
  createTag,
  deleteSiteMessage,
  getSiteMessages,
  updateRolePermission,
  updateSearchVisibility,
} from "./api"

const BASE = "https://api.example.test"

function mockFetch(status: number, body: unknown = {}, contentType = "application/json") {
  global.fetch = vi.fn(async () => {
    if (status === 204) {
      return new Response(null, { status: 204 })
    }
    return new Response(typeof body === "string" ? body : JSON.stringify(body), {
      status,
      headers: { "Content-Type": contentType },
    })
  }) as typeof fetch
}

describe("settings api", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getToken).mockResolvedValue("test-token")
    vi.mocked(getServerApiBase).mockReturnValue(BASE)
  })

  describe("getSiteMessages", () => {
    test("returns data on successful GET", async () => {
      vi.mocked(apiFetchJson).mockResolvedValueOnce({
        ok: true,
        data: [{ id: 1, value: "Welcome", description: null }],
      })

      const result = await getSiteMessages()

      expect(result.ok).toBe(true)
      if (result.ok) {
        expect(result.data).toHaveLength(1)
      }
      expect(apiFetchJson).toHaveBeenCalledWith(`${BASE}/api/settings/site-messages`, {
        headers: { Authorization: "Bearer test-token" },
        cache: "no-store",
      })
    })

    test("returns auth_required when token is missing", async () => {
      vi.mocked(getToken).mockResolvedValue(null)

      const result = await getSiteMessages()

      expect(result.ok).toBe(false)
      if (!result.ok) {
        expect(result.code).toBe("auth_required")
      }
    })

    test("returns forbidden message on 403 GET", async () => {
      vi.mocked(apiFetchJson).mockResolvedValueOnce({
        ok: false,
        error: new AppError({ code: "forbidden", status: 403 }),
      })

      const result = await getSiteMessages()

      expect(result.ok).toBe(false)
      if (!result.ok) {
        expect(result.code).toBe("forbidden")
        expect(result.message).toMatch(/permission/i)
      }
    })
  })

  describe("addSiteMessage", () => {
    test("POSTs to the correct endpoint with body", async () => {
      mockFetch(200, { id: 2, value: "New", description: null })

      const result = await addSiteMessage({ value: "New" })

      expect(result.ok).toBe(true)
      expect(global.fetch).toHaveBeenCalledWith(
        `${BASE}/api/settings/site-messages`,
        expect.objectContaining({
          method: "POST",
          body: JSON.stringify({ value: "New" }),
        }),
      )
    })

    test("returns validation error on 400", async () => {
      mockFetch(400, { error: "Value is required." })

      const result = await addSiteMessage({ value: "" })

      expect(result.ok).toBe(false)
      if (!result.ok) {
        expect(result.message).toBe("Value is required.")
        expect(result.code).toBe("bad_request")
      }
    })

    test("returns forbidden on 403", async () => {
      mockFetch(403, { error: "Forbidden" })

      const result = await addSiteMessage({ value: "Test" })

      expect(result.ok).toBe(false)
      if (!result.ok) {
        expect(result.code).toBe("forbidden")
      }
    })
  })

  describe("deleteSiteMessage", () => {
    test("DELETEs the correct resource", async () => {
      mockFetch(204)

      const result = await deleteSiteMessage(5)

      expect(result.ok).toBe(true)
      expect(global.fetch).toHaveBeenCalledWith(
        `${BASE}/api/settings/site-messages/5`,
        expect.objectContaining({ method: "DELETE" }),
      )
    })
  })

  describe("updateSearchVisibility", () => {
    test("PUTs visibility with reportTypeId query param", async () => {
      mockFetch(204)

      const result = await updateSearchVisibility("reports", true, 42)

      expect(result.ok).toBe(true)
      expect(global.fetch).toHaveBeenCalledWith(
        `${BASE}/api/settings/search/reports/visibility?reportTypeId=42`,
        expect.objectContaining({
          method: "PUT",
          body: JSON.stringify({ visible: true }),
        }),
      )
    })
  })

  describe("createTag", () => {
    test("POSTs tag to the correct type endpoint", async () => {
      mockFetch(200, { id: 1, name: "High", description: null, used: 0 })

      const result = await createTag("organizational-values", { name: "High" })

      expect(result.ok).toBe(true)
      expect(global.fetch).toHaveBeenCalledWith(
        `${BASE}/api/settings/tags/organizational-values`,
        expect.objectContaining({
          method: "POST",
          body: JSON.stringify({ name: "High" }),
        }),
      )
    })
  })

  describe("updateRolePermission", () => {
    test("PUTs permission toggle", async () => {
      mockFetch(204)

      const result = await updateRolePermission(2, 4, true)

      expect(result.ok).toBe(true)
      expect(global.fetch).toHaveBeenCalledWith(
        `${BASE}/api/settings/roles/2/permissions/4`,
        expect.objectContaining({
          method: "PUT",
          body: JSON.stringify({ enabled: true }),
        }),
      )
    })

    test("returns service_unavailable when api base is missing", async () => {
      vi.mocked(getServerApiBase).mockReturnValue(undefined as unknown as string)

      const result = await updateRolePermission(2, 4, true)

      expect(result.ok).toBe(false)
      if (!result.ok) {
        expect(result.code).toBe("service_unavailable")
      }
    })
  })
})
