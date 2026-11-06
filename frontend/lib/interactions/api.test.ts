import { beforeEach, describe, expect, test, vi } from "vitest"

vi.mock("@/lib/auth", () => ({
  getToken: vi.fn(async () => "test-token"),
}))

vi.mock("@/lib/api-base", () => ({
  getServerApiBase: vi.fn(() => "https://api.example.test"),
}))

import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import { searchDirectors, submitAccessRequest } from "./api"

const ACCESS_ENDPOINT = "https://api.example.test/api/interactions/access-request"
const DIRECTORS_ENDPOINT = "https://api.example.test/api/interactions/search/directors?q=Smith"

const VALID_BODY = {
  reportName: "Sales Dashboard",
  reportUrl: "https://example.com/reports/1",
  directorName: "Jane Smith",
}

function mockFetch(status: number, body: unknown = {}) {
  global.fetch = vi.fn(
    async () =>
      new Response(JSON.stringify(body), {
        status,
        headers: { "Content-Type": "application/json" },
      }),
  ) as typeof fetch
}

describe("submitAccessRequest", () => {
  beforeEach(() => {
    vi.mocked(getToken).mockResolvedValue("test-token")
    vi.mocked(getServerApiBase).mockReturnValue("https://api.example.test")
  })

  test("returns ok:true when the API responds 200", async () => {
    mockFetch(200, {})

    const result = await submitAccessRequest(VALID_BODY)

    expect(result.ok).toBe(true)
  })

  test("posts to the correct endpoint with the correct JSON body", async () => {
    mockFetch(200, {})

    await submitAccessRequest(VALID_BODY)

    expect(global.fetch).toHaveBeenCalledWith(
      ACCESS_ENDPOINT,
      expect.objectContaining({
        method: "POST",
        body: JSON.stringify(VALID_BODY),
        headers: expect.objectContaining({
          "Content-Type": "application/json",
          Authorization: "Bearer test-token",
        }),
      }),
    )
  })

  test("returns ok:false with the API error message on 400 (validation error)", async () => {
    mockFetch(400, { error: "Report name is required." })

    const result = await submitAccessRequest(VALID_BODY)

    expect(result.ok).toBe(false)
    if (!result.ok) {
      expect(result.message).toBe("Report name is required.")
      expect(result.code).toBe("bad_request")
    }
  })

  test("returns ok:false with friendly message on 503 (service unavailable)", async () => {
    global.fetch = vi.fn(async () => new Response("", { status: 503 })) as typeof fetch

    const result = await submitAccessRequest(VALID_BODY)

    expect(result.ok).toBe(false)
    if (!result.ok) {
      expect(result.message).toMatch(/temporarily unavailable/i)
      expect(result.code).toBe("service_unavailable")
    }
  })

  test("returns ok:false with auth_required when token is missing", async () => {
    vi.mocked(getToken).mockResolvedValue(null)

    const result = await submitAccessRequest(VALID_BODY)

    expect(result.ok).toBe(false)
    if (!result.ok) {
      expect(result.code).toBe("auth_required")
    }
  })

  test("returns ok:false with service_unavailable when apiBase is missing", async () => {
    vi.mocked(getServerApiBase).mockReturnValue(undefined as unknown as string)

    const result = await submitAccessRequest(VALID_BODY)

    expect(result.ok).toBe(false)
    if (!result.ok) {
      expect(result.code).toBe("service_unavailable")
    }
  })

  test("returns ok:false with service_unavailable when fetch rejects due to network failure", async () => {
    global.fetch = vi.fn(async () => Promise.reject(new Error("Failed to fetch"))) as typeof fetch

    const result = await submitAccessRequest(VALID_BODY)

    expect(result.ok).toBe(false)
    if (!result.ok) {
      expect(result.message).toMatch(/temporarily unavailable/i)
      expect(result.code).toBe("service_unavailable")
    }
  })
})

describe("searchDirectors", () => {
  beforeEach(() => {
    vi.mocked(getToken).mockResolvedValue("test-token")
    vi.mocked(getServerApiBase).mockReturnValue("https://api.example.test")
  })

  test("returns an empty list without calling fetch when the query is empty or whitespace", async () => {
    global.fetch = vi.fn() as typeof fetch

    expect(await searchDirectors("")).toEqual([])
    expect(await searchDirectors("   \t")).toEqual([])

    expect(global.fetch).not.toHaveBeenCalled()
  })

  test("GETs /api/interactions/search/directors with bearer auth", async () => {
    mockFetch(200, [{ id: 1, name: "Pat Smith", type: "u", email: null }])

    const result = await searchDirectors("Smith")

    expect(result).toEqual([{ id: 1, name: "Pat Smith", type: "u", email: null }])
    expect(global.fetch).toHaveBeenCalledWith(
      DIRECTORS_ENDPOINT,
      expect.objectContaining({
        headers: expect.objectContaining({
          Authorization: "Bearer test-token",
        }),
        cache: "no-store",
      }),
    )
  })

  test("trims the query before requesting", async () => {
    mockFetch(200, [])

    await searchDirectors("  Smith  ")

    expect(global.fetch).toHaveBeenCalledWith(
      "https://api.example.test/api/interactions/search/directors?q=Smith",
      expect.anything(),
    )
  })
})
