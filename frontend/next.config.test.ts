import { afterEach, describe, expect, test } from "vitest"
import nextConfig from "./next.config"

const originalApiUrl = process.env.API_URL

afterEach(() => {
  if (originalApiUrl === undefined) delete process.env.API_URL
  else process.env.API_URL = originalApiUrl
})

describe("next rewrites", () => {
  test("does not send /terms to the Razor app", async () => {
    process.env.API_URL = "https://api.example.test"
    const rewrites = await nextConfig.rewrites?.()
    const rules = Array.isArray(rewrites) ? rewrites : []

    expect(rules.some((rule) => "source" in rule && rule.source === "/terms")).toBe(false)
    expect(rules.some((rule) => "source" in rule && rule.source.startsWith("/api/"))).toBe(true)
  })
})
