import { beforeEach, describe, expect, test, vi } from "vitest"

vi.mock("@/lib/auth", () => ({
  getToken: vi.fn(async () => "test-token"),
}))

vi.mock("@/lib/api-base", () => ({
  getServerApiBase: vi.fn(() => "https://api.example.test"),
}))

import { createUserFolder } from "./api"

describe("createUserFolder", () => {
  beforeEach(() => {
    vi.restoreAllMocks()
    global.fetch = vi.fn(
      async () =>
        new Response(JSON.stringify({ id: 1, name: "Favorites" }), {
          status: 200,
          headers: { "Content-Type": "application/json" },
        }),
    ) as typeof fetch
  })

  test("uses the current-user folders endpoint when isCurrentUser is true", async () => {
    await createUserFolder(42, true, { name: "Favorites" })

    expect(global.fetch).toHaveBeenCalledWith(
      "https://api.example.test/api/users/me/folders",
      expect.objectContaining({ method: "POST" }),
    )
  })

  test("uses the target-user folders endpoint when isCurrentUser is false", async () => {
    await createUserFolder(42, false, { name: "Favorites" })

    expect(global.fetch).toHaveBeenCalledWith(
      "https://api.example.test/api/users/42/folders",
      expect.objectContaining({ method: "POST" }),
    )
  })
})
