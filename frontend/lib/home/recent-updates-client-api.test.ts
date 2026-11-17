import { afterEach, beforeEach, describe, expect, test, vi } from "vitest"
import {
  collectionDetailHref,
  fetchRecentUpdatesList,
  formatRecentUpdateName,
  initiativeDetailHref,
  reportDetailHref,
} from "./recent-updates-client-api"

const fetchMock = vi.fn<(input: RequestInfo | URL, init?: RequestInit) => Promise<Response>>()

describe("fetchRecentUpdatesList", () => {
  beforeEach(() => {
    vi.stubGlobal("fetch", fetchMock)
  })

  afterEach(() => {
    vi.unstubAllGlobals()
    fetchMock.mockReset()
  })

  test("requests GET /api/reports/recent with credentials", async () => {
    fetchMock.mockResolvedValueOnce(new Response(JSON.stringify([]), { status: 200 }))

    const result = await fetchRecentUpdatesList("reports")

    expect(fetchMock).toHaveBeenCalledWith(
      "/api/reports/recent",
      expect.objectContaining({ credentials: "include" }),
    )
    expect(result).toEqual({ ok: true, data: [] })
  })

  test("requests GET /api/initiatives/recent", async () => {
    fetchMock.mockResolvedValueOnce(new Response(JSON.stringify([]), { status: 200 }))

    await fetchRecentUpdatesList("initiatives")

    expect(fetchMock).toHaveBeenCalledWith("/api/initiatives/recent", expect.any(Object))
  })

  test("returns http_error when the response is not ok", async () => {
    fetchMock.mockResolvedValueOnce(new Response("", { status: 503 }))

    const result = await fetchRecentUpdatesList("collections")

    expect(result).toEqual({ ok: false, error: "http_error", status: 503 })
  })
})

describe("formatRecentUpdateName", () => {
  test("replaces underscores with spaces", () => {
    expect(formatRecentUpdateName("Report_10")).toBe("Report 10")
  })
})

describe("detail href helpers", () => {
  test("builds report, initiative, and collection links", () => {
    expect(reportDetailHref(1)).toBe("/reports?id=1")
    expect(initiativeDetailHref(2)).toBe("/initiatives?id=2")
    expect(collectionDetailHref(3)).toBe("/collections?id=3")
  })
})
