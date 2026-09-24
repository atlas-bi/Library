import { beforeEach, describe, expect, test, vi } from "vitest"

vi.mock("@/lib/profile/api", () => ({
  getProfileChart: vi.fn(),
  getProfileUsers: vi.fn(),
  getProfileReports: vi.fn(),
  getProfileFails: vi.fn(),
  getProfileRunList: vi.fn(),
  getProfileStars: vi.fn(),
  getProfileSubscriptions: vi.fn(),
  getProfileFilters: vi.fn(),
}))

import {
  getProfileChart,
  getProfileFails,
  getProfileReports,
  getProfileRunList,
  getProfileStars,
  getProfileSubscriptions,
  getProfileUsers,
} from "@/lib/profile/api"
import { loadProfileAnalyticsAction } from "./actions"

describe("loadProfileAnalyticsAction", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  test("returns auth_required when any profile request is unauthorized", async () => {
    vi.mocked(getProfileChart).mockResolvedValueOnce({ data: null, error: "auth_required" })
    vi.mocked(getProfileUsers).mockResolvedValueOnce({ data: [], error: null })
    vi.mocked(getProfileReports).mockResolvedValueOnce({ data: [], error: null })
    vi.mocked(getProfileFails).mockResolvedValueOnce({ data: [], error: null })
    vi.mocked(getProfileRunList).mockResolvedValueOnce({ data: [], error: null })
    vi.mocked(getProfileStars).mockResolvedValueOnce({ data: [], error: null })
    vi.mocked(getProfileSubscriptions).mockResolvedValueOnce({ data: [], error: null })

    const result = await loadProfileAnalyticsAction(16, "report")

    expect(result).toEqual({ data: null, error: "auth_required" })
  })

  test("returns partial data when non-auth endpoints fail", async () => {
    vi.mocked(getProfileChart).mockResolvedValueOnce({
      data: { runs: 1, users: 2, runTime: 3, history: [] },
      error: null,
    })
    vi.mocked(getProfileUsers).mockResolvedValueOnce({ data: [], error: "service_unavailable" })
    vi.mocked(getProfileReports).mockResolvedValueOnce({ data: [], error: "service_unavailable" })
    vi.mocked(getProfileFails).mockResolvedValueOnce({ data: [], error: "service_unavailable" })
    vi.mocked(getProfileRunList).mockResolvedValueOnce({ data: [], error: "service_unavailable" })
    vi.mocked(getProfileStars).mockResolvedValueOnce({ data: [], error: "service_unavailable" })
    vi.mocked(getProfileSubscriptions).mockResolvedValueOnce({
      data: [],
      error: "service_unavailable",
    })

    const result = await loadProfileAnalyticsAction(16, "report")

    expect(result.error).toBeNull()
    expect(result.data?.chart?.runs).toBe(1)
    expect(result.data?.users).toEqual([])
  })

  test("returns an empty shell for term profiles even when chart data is missing", async () => {
    vi.mocked(getProfileChart).mockResolvedValueOnce({ data: null, error: "not_found" })
    vi.mocked(getProfileUsers).mockResolvedValueOnce({ data: [], error: "not_found" })
    vi.mocked(getProfileReports).mockResolvedValueOnce({ data: [], error: "not_found" })
    vi.mocked(getProfileFails).mockResolvedValueOnce({ data: [], error: "not_found" })
    vi.mocked(getProfileRunList).mockResolvedValueOnce({ data: [], error: null })
    vi.mocked(getProfileStars).mockResolvedValueOnce({ data: [], error: "not_found" })
    vi.mocked(getProfileSubscriptions).mockResolvedValueOnce({ data: [], error: null })

    const result = await loadProfileAnalyticsAction(1, "term")

    expect(result.error).toBeNull()
    expect(result.data).toEqual({
      chart: { runs: 0, users: 0, runTime: 0, history: [] },
      users: [],
      reports: [],
      fails: [],
      runList: [],
      stars: [],
      subscriptions: [],
    })
  })
})
