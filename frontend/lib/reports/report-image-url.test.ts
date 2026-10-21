import { describe, expect, it } from "vitest"
import { getReportObjectImageThumbUrl } from "@/lib/reports/report-image-url"

describe("getReportObjectImageThumbUrl", () => {
  it("builds the Razor edit wizard thumb handler URL", () => {
    expect(getReportObjectImageThumbUrl(42)).toBe(
      "/data/img?handler=Thumb&id=-1&imgId=42&size=256x256",
    )
  })
})
