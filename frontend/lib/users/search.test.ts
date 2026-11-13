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
import {
  mapUserSearchResultsToTypeahead,
  searchUsers,
  searchUsersForTypeahead,
} from "@/lib/users/search"

describe("searchUsers", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getServerApiBase).mockReturnValue("https://api.example.test")
  })

  test("returns an empty array without calling the API for a blank query", async () => {
    expect(await searchUsers("")).toEqual([])
    expect(await searchUsers("  \t")).toEqual([])
    expect(apiFetchJson).not.toHaveBeenCalled()
  })

  test("calls GET /api/users/search with bearer auth", async () => {
    vi.mocked(apiFetchJson).mockResolvedValueOnce({
      ok: true,
      data: [{ id: 7, name: "Pat Smith", email: "pat@example.com" }],
    })

    const result = await searchUsers("pat")

    expect(apiFetchJson).toHaveBeenCalledWith(
      "https://api.example.test/api/users/search?q=pat",
      expect.objectContaining({
        headers: { Authorization: "Bearer test-token" },
        cache: "no-store",
      }),
    )
    expect(result).toEqual([{ id: 7, name: "Pat Smith", email: "pat@example.com" }])
  })
})

describe("mapUserSearchResultsToTypeahead", () => {
  test("maps email into the typeahead description", () => {
    expect(
      mapUserSearchResultsToTypeahead([{ id: 3, name: "Alex", email: "alex@example.com" }]),
    ).toEqual([{ id: 3, name: "Alex", description: "alex@example.com" }])
  })
})

describe("searchUsersForTypeahead", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getServerApiBase).mockReturnValue("https://api.example.test")
  })

  test("returns mapped typeahead rows", async () => {
    vi.mocked(apiFetchJson).mockResolvedValueOnce({
      ok: true,
      data: [{ id: 1, name: "Jordan", email: "jordan@example.com" }],
    })

    const rows = await searchUsersForTypeahead("jor")

    expect(rows).toEqual([{ id: 1, name: "Jordan", description: "jordan@example.com" }])
  })
})
