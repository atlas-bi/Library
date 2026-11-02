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
import { searchGroups } from "./api"

describe("searchGroups", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getServerApiBase).mockReturnValue("https://api.example.test")
  })

  test("returns empty array without calling the API for blank query", async () => {
    const result = await searchGroups("   ")

    expect(result).toEqual([])
    expect(apiFetchJson).not.toHaveBeenCalled()
  })

  test("calls GET /api/groups/search with bearer auth", async () => {
    vi.mocked(apiFetchJson).mockResolvedValueOnce({
      ok: true,
      data: [{ id: 2, name: "Clinical Operations", email: "clinical-ops@example.com" }],
    })

    const result = await searchGroups("clinical")

    expect(apiFetchJson).toHaveBeenCalledWith(
      "https://api.example.test/api/groups/search?q=clinical",
      expect.objectContaining({
        headers: { Authorization: "Bearer test-token" },
      }),
    )
    expect(result).toEqual([
      { id: 2, name: "Clinical Operations", email: "clinical-ops@example.com" },
    ])
  })
})
