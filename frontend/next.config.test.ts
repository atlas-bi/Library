import { afterEach, describe, expect, it, test } from "vitest"
import nextConfig from "./next.config"

const originalApiUrl = process.env.API_URL

afterEach(() => {
  if (originalApiUrl === undefined) delete process.env.API_URL
  else process.env.API_URL = originalApiUrl
})

describe("legacy report object redirects", () => {
  it("sends old report object urls to /reports and keeps the query", async () => {
    if (!nextConfig.redirects) {
      throw new Error("next config redirects are missing")
    }
    const redirects = await nextConfig.redirects()
    const sources = [
      "/ReportObjects/Details",
      "/ReportObjects/Detials",
      "/reportobjects/details",
      "/reportobjects/detials",
    ]

    for (const source of sources) {
      const rule = redirects.find((redirect) => redirect.source === source)
      expect(rule?.destination).toBe("/reports")
      expect(rule?.permanent).toBe(false)
    }
  })
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
