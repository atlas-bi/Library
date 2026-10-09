import { describe, expect, it } from "vitest"
import { buildAnalyticsBeaconPayload } from "./beacon-payload"

describe("buildAnalyticsBeaconPayload", () => {
  it("builds camelCase fields expected by the analytics API", () => {
    const payload = buildAnalyticsBeaconPayload({
      sessionId: "sid",
      pageId: "pid",
      pageStartedAtMs: 1000,
      nowMs: 2500,
      loadTime: 42,
      language: "en-US",
      userAgent: "test-agent",
      location: {
        host: "localhost:3001",
        hostname: "localhost",
        href: "http://localhost:3001/terms",
        protocol: "http:",
        search: "",
        pathname: "/terms",
        origin: "http://localhost:3001",
      },
      referrer: "",
      screenHeight: 800,
      screenWidth: 1200,
      devicePixelRatio: 1,
    })

    expect(payload.sessionId).toBe("sid")
    expect(payload.pageId).toBe("pid")
    expect(payload.pageTime).toBe(1500)
    expect(payload.loadTime).toBe("42")
    expect(payload.pathname).toBe("/terms")
  })
})
