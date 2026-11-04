import { afterEach, beforeEach, describe, expect, test, vi } from "vitest"
import {
  fetchRecentTerms,
  formatRecentTermName,
  normalizeRecentTerms,
  termDetailHref,
} from "./recent-terms-client-api"

const fetchMock = vi.fn<(input: RequestInfo | URL, init?: RequestInit) => Promise<Response>>()

describe("fetchRecentTerms", () => {
  beforeEach(() => {
    vi.stubGlobal("fetch", fetchMock)
  })

  afterEach(() => {
    vi.unstubAllGlobals()
    fetchMock.mockReset()
  })

  test("requests GET /api/terms/recent with credentials", async () => {
    fetchMock.mockResolvedValueOnce(
      new Response(JSON.stringify({ approved: [], pending: [] }), { status: 200 }),
    )

    const result = await fetchRecentTerms()

    expect(fetchMock).toHaveBeenCalledWith(
      "/api/terms/recent",
      expect.objectContaining({ credentials: "include" }),
    )
    expect(result.ok).toBe(true)
    if (result.ok) {
      expect(result.data).toEqual({ approved: [], pending: [] })
    }
  })

  test("returns http_error when the response is not ok", async () => {
    fetchMock.mockResolvedValueOnce(new Response("", { status: 500 }))

    const result = await fetchRecentTerms()

    expect(result).toEqual({ ok: false, error: "http_error", status: 500 })
  })
})

describe("normalizeRecentTerms", () => {
  test("treats missing arrays as empty", () => {
    expect(normalizeRecentTerms(undefined)).toEqual({ approved: [], pending: [] })
    expect(normalizeRecentTerms({})).toEqual({ approved: [], pending: [] })
  })
})

describe("formatRecentTermName", () => {
  test("replaces underscores with spaces", () => {
    expect(formatRecentTermName("Net_Promoter_Score")).toBe("Net Promoter Score")
  })
})

describe("termDetailHref", () => {
  test("builds the term detail URL", () => {
    expect(termDetailHref(42)).toBe("/terms?id=42")
  })
})
