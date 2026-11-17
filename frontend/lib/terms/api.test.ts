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
import { apiFetchJson } from "@/lib/http"
import { getRecentTerms } from "./api"

describe("getRecentTerms", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getServerApiBase).mockReturnValue("https://api.example.test")
  })

  test("calls GET /api/terms/recent with bearer auth", async () => {
    vi.mocked(apiFetchJson).mockResolvedValueOnce({
      ok: true,
      data: {
        approved: [{ id: 1, name: "Revenue_Term" }],
        pending: [{ id: 2, name: "Draft_Term" }],
      },
    })

    const result = await getRecentTerms()

    expect(apiFetchJson).toHaveBeenCalledWith(
      "https://api.example.test/api/terms/recent",
      expect.objectContaining({
        headers: { Authorization: "Bearer test-token" },
      }),
    )
    expect(result.error).toBeNull()
    expect(result.data?.approved).toHaveLength(1)
    expect(result.data?.pending).toHaveLength(1)
  })

  test("returns empty approved and pending lists from the API", async () => {
    vi.mocked(apiFetchJson).mockResolvedValueOnce({
      ok: true,
      data: { approved: [], pending: [] },
    })

    const result = await getRecentTerms()

    expect(result.data).toEqual({ approved: [], pending: [] })
    expect(result.error).toBeNull()
  })
})
