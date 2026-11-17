import { beforeEach, describe, expect, test, vi } from "vitest"

vi.mock("@/lib/auth", () => ({
  getToken: vi.fn(async () => "test-token"),
}))

vi.mock("@/lib/api-base", () => ({
  getServerApiBase: vi.fn(() => "https://api.example.test"),
}))

import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import { searchDirectors, sendShareMail, submitAccessRequest, toggleStar } from "./api"

const ACCESS_ENDPOINT = "https://api.example.test/api/interactions/access-request"
const SHARE_MAIL_ENDPOINT = "https://api.example.test/api/mail/messages"
const DIRECTORS_ENDPOINT = "https://api.example.test/api/interactions/search/directors?q=Smith"
const STAR_ENDPOINT = "https://api.example.test/api/interactions/stars/toggle"

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

const SHARE_BODY = {
  draftId: 0,
  to: [{ userId: 1, type: "u" as const }],
  subject: "Hello",
  message: "<p>Hi</p>",
  text: "Hi",
  share: true,
  shareName: "Q4 Report",
  shareUrl: "https://example.com/reports/1",
}

describe("sendShareMail", () => {
  beforeEach(() => {
    vi.mocked(getToken).mockResolvedValue("test-token")
    vi.mocked(getServerApiBase).mockReturnValue("https://api.example.test")
  })

  test("posts to POST /api/mail/messages with bearer auth", async () => {
    mockFetch(200, {
      message: "Successfully shared.",
      recipientCount: 1,
      shareCount: 1,
    })

    const result = await sendShareMail(SHARE_BODY)

    expect(result.ok).toBe(true)
    if (result.ok) {
      expect(result.data.message).toBe("Successfully shared.")
      expect(result.data.recipientCount).toBe(1)
      expect(result.data.shareCount).toBe(1)
    }

    expect(global.fetch).toHaveBeenCalledWith(
      SHARE_MAIL_ENDPOINT,
      expect.objectContaining({
        method: "POST",
        body: JSON.stringify(SHARE_BODY),
        headers: expect.objectContaining({
          "Content-Type": "application/json",
          Authorization: "Bearer test-token",
        }),
      }),
    )
  })

  test("returns ok:false with API error on failed send", async () => {
    mockFetch(400, { error: "No recipients specified." })

    const result = await sendShareMail(SHARE_BODY)

    expect(result.ok).toBe(false)
    if (!result.ok) {
      expect(result.message).toBe("No recipients specified.")
      expect(result.code).toBe("bad_request")
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

describe("toggleStar", () => {
  beforeEach(() => {
    vi.mocked(getToken).mockResolvedValue("test-token")
    vi.mocked(getServerApiBase).mockReturnValue("https://api.example.test")
  })

  test("posts user, group, and search JSON bodies to the toggle endpoint", async () => {
    mockFetch(200, { type: "user", id: 1, isStarred: true, count: 3 })

    await toggleStar({ type: "user", id: 1 })
    expect(global.fetch).toHaveBeenLastCalledWith(
      STAR_ENDPOINT,
      expect.objectContaining({
        method: "POST",
        body: JSON.stringify({ type: "user", id: 1 }),
      }),
    )

    mockFetch(200, { type: "group", id: 2, isStarred: false, count: 1 })
    await toggleStar({ type: "group", id: 2 })
    expect(global.fetch).toHaveBeenLastCalledWith(
      STAR_ENDPOINT,
      expect.objectContaining({
        body: JSON.stringify({ type: "group", id: 2 }),
      }),
    )

    mockFetch(200, { type: "search", id: 0, isStarred: true, count: 4 })
    await toggleStar({ type: "search", id: 0, search: "census" })
    expect(global.fetch).toHaveBeenLastCalledWith(
      STAR_ENDPOINT,
      expect.objectContaining({
        body: JSON.stringify({ type: "search", id: 0, search: "census" }),
      }),
    )
  })
})
