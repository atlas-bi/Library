import { describe, expect, it } from "vitest"
import { buildReportSectionLinks } from "@/components/reports/report-section-nav"

describe("buildReportSectionLinks", () => {
  it("includes the core Razor sections for a documented report", () => {
    const links = buildReportSectionLinks({
      description: "System description",
      document: {
        developerDescription: "Developer notes",
        keyAssumptions: "Assumptions",
        maintenanceLogs: [{ id: 1 }],
      },
      terms: [{ id: 1 }],
      queries: [{ id: 1 }],
      collections: [{ id: 1 }],
      features: { termsEnabled: true },
    })

    expect(links.map((link) => link.label)).toEqual([
      "Description",
      "Terms",
      "Details",
      "Query",
      "Relationships",
      "Maintenance",
    ])
  })
})
