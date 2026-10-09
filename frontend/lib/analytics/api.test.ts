import { beforeEach, describe, expect, it, vi } from "vitest"

const { getTokenMock, getServerApiBaseMock, apiFetchJsonMock } = vi.hoisted(() => ({
  getTokenMock: vi.fn(),
  getServerApiBaseMock: vi.fn(),
  apiFetchJsonMock: vi.fn(),
}))

vi.mock("@/lib/auth", () => ({ getToken: getTokenMock }))
vi.mock("@/lib/api-base", () => ({ getServerApiBase: getServerApiBaseMock }))
vi.mock("@/lib/http", () => ({ apiFetchJson: apiFetchJsonMock }))

import { getAnalyticsDashboard, getAnalyticsVisits, recordAnalyticsBeacon } from "./api"

describe("analytics api", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getTokenMock.mockResolvedValue("token")
    getServerApiBaseMock.mockReturnValue("https://api.test")
  })

  it("returns auth_required without a token", async () => {
    getTokenMock.mockResolvedValueOnce(null)
    const result = await getAnalyticsVisits({ startAt: -86400, endAt: 0 })
    expect(result).toEqual({ data: null, error: "auth_required" })
  })

  it("loads visits with range query params", async () => {
    apiFetchJsonMock.mockResolvedValueOnce({
      ok: true,
      data: { views: 10, visitors: 4, loadTime: 1.2, accessHistory: [] },
    })

    await getAnalyticsVisits({ startAt: -86400, endAt: 0, userId: 2 })

    expect(apiFetchJsonMock).toHaveBeenCalledWith(
      "https://api.test/api/analytics/visits?start_at=-86400&end_at=0&userId=2",
      expect.objectContaining({
        headers: { Authorization: "Bearer token" },
      }),
    )
  })

  it("posts beacon payloads", async () => {
    apiFetchJsonMock.mockResolvedValueOnce({ ok: true, data: { status: "ok" } })

    await recordAnalyticsBeacon({
      language: "en",
      userAgent: "ua",
      host: "h",
      hostname: "h",
      href: "http://h/",
      protocol: "http:",
      search: "",
      pathname: "/",
      screenHeight: 1,
      screenWidth: 1,
      origin: "http://h",
      referrer: "",
      loadTime: "0",
      zoom: 1,
      sessionId: "s",
      pageId: "p",
      pageTime: 0,
    })

    expect(apiFetchJsonMock).toHaveBeenCalledWith(
      "https://api.test/api/analytics/beacon",
      expect.objectContaining({
        method: "POST",
        headers: expect.objectContaining({
          Authorization: "Bearer token",
          "Content-Type": "application/json",
        }),
      }),
    )
  })

  it("aggregates dashboard responses", async () => {
    apiFetchJsonMock
      .mockResolvedValueOnce({
        ok: true,
        data: { views: 1, visitors: 1, loadTime: 1, accessHistory: [] },
      })
      .mockResolvedValueOnce({
        ok: true,
        data: [
          {
            key: "Chrome",
            count: 1,
            titleOne: "Browser",
            titleTwo: "Views",
            href: null,
            percent: 1,
          },
        ],
      })
      .mockResolvedValueOnce({ ok: true, data: [] })
      .mockResolvedValueOnce({ ok: true, data: [] })
      .mockResolvedValueOnce({ ok: true, data: [] })
      .mockResolvedValueOnce({ ok: true, data: [] })
      .mockResolvedValueOnce({ ok: true, data: { activeUsers: 0, items: [] } })
      .mockResolvedValueOnce({
        ok: true,
        data: { pages: 1, currentPage: 1, totalCount: 0, unresolvedCount: 0, items: [] },
      })
      .mockResolvedValueOnce({
        ok: true,
        data: { pages: 1, currentPage: 1, totalCount: 0, unresolvedCount: 0, items: [] },
      })

    const result = await getAnalyticsDashboard(
      { startAt: -86400, endAt: 0 },
      { includeTopUsers: true, tracePage: 0, errorPage: 0 },
    )

    expect(result.error).toBeNull()
    expect(result.data?.visits?.views).toBe(1)
    expect(result.data?.browsers).toHaveLength(1)
  })
})
