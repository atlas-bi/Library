import { describe, expect, it } from "vitest"
import {
  getReportImagePlaceholderPath,
  getReportImagePlaceholderUrl,
  getReportThumbnailPath,
  getReportThumbnailUrl,
} from "@/lib/reports/report-image-url"

describe("getReportThumbnailPath", () => {
  it("builds the thumbnail API path with size and imgId", () => {
    expect(getReportThumbnailPath(16, { imageId: 99, size: "256x256" })).toBe(
      "/api/reports/16/thumbnail?size=256x256&imgId=99",
    )
  })

  it("defaults size when omitted", () => {
    expect(getReportThumbnailPath(7, { imageId: 1 })).toBe(
      "/api/reports/7/thumbnail?size=256x256&imgId=1",
    )
  })

  it("includes type when set", () => {
    expect(
      getReportThumbnailPath(3, {
        imageId: 10,
        size: "128x128",
        type: "webp",
      }),
    ).toBe("/api/reports/3/thumbnail?size=128x128&imgId=10&type=webp")
  })
})

describe("getReportThumbnailUrl", () => {
  it("prefixes apiBase and strips trailing slash", () => {
    expect(
      getReportThumbnailUrl(16, {
        apiBase: "http://localhost:5000/",
        imageId: 99,
        size: "256x256",
      }),
    ).toBe("http://localhost:5000/api/reports/16/thumbnail?size=256x256&imgId=99")
  })
})

describe("getReportImagePlaceholderPath", () => {
  it("builds placeholder path without size", () => {
    expect(getReportImagePlaceholderPath()).toBe("/api/reports/images/placeholder")
  })

  it("includes size query when set", () => {
    expect(getReportImagePlaceholderPath({ size: "256x256" })).toBe(
      "/api/reports/images/placeholder?size=256x256",
    )
  })
})

describe("getReportImagePlaceholderUrl", () => {
  it("prefixes apiBase", () => {
    expect(
      getReportImagePlaceholderUrl({
        apiBase: "http://localhost:5000",
        size: "256x256",
      }),
    ).toBe("http://localhost:5000/api/reports/images/placeholder?size=256x256")
  })
})
