import { describe, expect, it } from "vitest"
import {
  getReportObjectImageThumbPath,
  getReportObjectImageThumbUrl,
} from "@/lib/reports/report-image-url"

describe("getReportObjectImageThumbUrl", () => {
  it("builds the Razor edit wizard thumb handler path", () => {
    expect(getReportObjectImageThumbPath(42)).toBe(
      "/data/img?handler=Thumb&id=-1&imgId=42&size=256x256",
    )
    expect(getReportObjectImageThumbUrl(42)).toBe(
      "/data/img?handler=Thumb&id=-1&imgId=42&size=256x256",
    )
  })

  it("prefixes the public API origin when apiBase is set", () => {
    expect(
      getReportObjectImageThumbUrl(42, { apiBase: "http://localhost:5000" }),
    ).toBe("http://localhost:5000/data/img?handler=Thumb&id=-1&imgId=42&size=256x256")
  })

  it("strips a trailing slash from apiBase", () => {
    expect(
      getReportObjectImageThumbUrl(1, { apiBase: "http://localhost:5000/" }),
    ).toBe("http://localhost:5000/data/img?handler=Thumb&id=-1&imgId=1&size=256x256")
  })
})
