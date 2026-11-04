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

import { apiFetchJson } from "@/lib/http"
import { getRecentCollections, getRecentInitiatives, getRecentReports, loadRecentUpdates } from "./recent-updates-api"

describe("getRecentReports", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  test("calls GET /api/reports/recent with bearer auth", async () => {
    vi.mocked(apiFetchJson).mockResolvedValueOnce({
      ok: true,
      data: [{ id: 3, name: "Report_3" }],
    })

    const result = await getRecentReports()

    expect(apiFetchJson).toHaveBeenCalledWith(
      "https://api.example.test/api/reports/recent",
      expect.objectContaining({
        headers: { Authorization: "Bearer test-token" },
      }),
    )
    expect(result.items).toEqual([{ id: 3, name: "Report_3" }])
    expect(result.error).toBeNull()
  })
})

describe("loadRecentUpdates", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  test("loads all three recent endpoints in parallel", async () => {
    vi.mocked(apiFetchJson)
      .mockResolvedValueOnce({ ok: true, data: [{ id: 1, name: "Report_1" }] })
      .mockResolvedValueOnce({ ok: true, data: [{ id: 2, name: "Initiative_2" }] })
      .mockResolvedValueOnce({ ok: true, data: [{ id: 4, name: "Collection_4" }] })

    const result = await loadRecentUpdates()

    expect(apiFetchJson).toHaveBeenCalledTimes(3)
    expect(apiFetchJson).toHaveBeenCalledWith(
      "https://api.example.test/api/initiatives/recent",
      expect.any(Object),
    )
    expect(apiFetchJson).toHaveBeenCalledWith(
      "https://api.example.test/api/collections/recent",
      expect.any(Object),
    )
    expect(result.error).toBeNull()
    expect(result.data?.reports.items).toHaveLength(1)
    expect(result.data?.initiatives.items).toHaveLength(1)
    expect(result.data?.collections.items).toHaveLength(1)
  })

  test("returns empty sections when APIs return empty arrays", async () => {
    vi.mocked(apiFetchJson)
      .mockResolvedValueOnce({ ok: true, data: [] })
      .mockResolvedValueOnce({ ok: true, data: [] })
      .mockResolvedValueOnce({ ok: true, data: [] })

    const result = await loadRecentUpdates()

    expect(result.error).toBeNull()
    expect(result.data).toEqual({
      reports: { items: [], failed: false },
      initiatives: { items: [], failed: false },
      collections: { items: [], failed: false },
    })
  })
})

describe("getRecentInitiatives", () => {
  test("calls GET /api/initiatives/recent", async () => {
    vi.mocked(apiFetchJson).mockResolvedValueOnce({ ok: true, data: [] })

    await getRecentInitiatives()

    expect(apiFetchJson).toHaveBeenCalledWith(
      "https://api.example.test/api/initiatives/recent",
      expect.any(Object),
    )
  })
})

describe("getRecentCollections", () => {
  test("calls GET /api/collections/recent", async () => {
    vi.mocked(apiFetchJson).mockResolvedValueOnce({ ok: true, data: [] })

    await getRecentCollections()

    expect(apiFetchJson).toHaveBeenCalledWith(
      "https://api.example.test/api/collections/recent",
      expect.any(Object),
    )
  })
})
