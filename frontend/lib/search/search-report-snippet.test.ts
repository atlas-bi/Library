import { describe, expect, it } from "vitest"
import { searchResultToCollectionReport } from "@/lib/search/search-report-snippet"
import type { SearchResultDto } from "@/lib/search/types"

const base: SearchResultDto = {
  id: "report-16",
  atlasId: 16,
  type: "reports",
  name: "Daily ED Census",
  description: "Census report",
  url: "https://example.test/run/16",
  reportType: "SSRS",
  email: null,
  epicMasterFile: null,
  epicRecordId: null,
  epicTemplateId: null,
  reportServerPath: null,
  executiveVisibility: null,
  sourceServer: null,
  groupType: null,
  isStarred: true,
  certifications: [],
  documented: null,
}

describe("searchResultToCollectionReport", () => {
  it("maps run URL and canRun from search payload", () => {
    const mapped = searchResultToCollectionReport(base)
    expect(mapped).toMatchObject({
      id: 16,
      name: "Daily ED Census",
      runUrl: "https://example.test/run/16",
      canRun: true,
      isStarred: true,
    })
  })

  it("sets canRun false when run URL is missing", () => {
    const mapped = searchResultToCollectionReport({ ...base, url: null })
    expect(mapped.canRun).toBe(false)
    expect(mapped.runUrl).toBeNull()
  })
})