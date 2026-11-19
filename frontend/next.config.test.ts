import { describe, expect, it } from "vitest"
import nextConfig from "./next.config"

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
