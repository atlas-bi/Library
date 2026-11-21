import { describe, expect, it } from "vitest"
import { buildReportUpdateBodyFromDetail } from "@/lib/reports/report-update-body"
import type { ReportDetail } from "@/lib/reports/types"

const report: ReportDetail = {
  id: 16,
  name: "demo_report",
  terms: [{ id: 1, name: "Term A" }],
  collections: [{ id: 2, name: "Collection B" }],
  images: [
    { id: 30, ordinal: 1 },
    { id: 20, ordinal: 0 },
  ],
  document: {
    developerDescription: "Desc",
    fragilityTags: [{ id: 5, name: "SQL" }],
    serviceRequests: [{ id: 9, ticketNumber: "INC-1" }],
    executiveVisibilityYn: "Y",
  },
}

describe("buildReportUpdateBodyFromDetail", () => {
  it("orders imageIds by ordinal and maps document fields", () => {
    const body = buildReportUpdateBodyFromDetail(report)
    expect(body.imageIds).toEqual([20, 30])
    expect(body.termIds).toEqual([1])
    expect(body.collectionIds).toEqual([2])
    expect(body.fragilityTagIds).toEqual([5])
    expect(body.serviceRequestIds).toEqual([9])
    expect(body.executiveVisibilityYn).toBe("Y")
  })

  it("accepts imageIds override for reorder and delete", () => {
    const body = buildReportUpdateBodyFromDetail(report, { imageIds: [30] })
    expect(body.imageIds).toEqual([30])
  })
})