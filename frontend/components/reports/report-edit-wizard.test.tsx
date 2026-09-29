import { render, screen } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"
import { ReportEditWizard } from "@/components/reports/report-edit-wizard"
import type { ReportDetail } from "@/lib/reports/types"

vi.mock("@/app/reports/actions", () => ({
  searchReportTermsAction: vi.fn(async () => []),
  searchReportCollectionsAction: vi.fn(async () => []),
  searchReportUsersAction: vi.fn(async () => []),
  updateReportAction: vi.fn(async () => ({ data: {} })),
}))

const lookupOptions = {
  organizationalValues: [{ id: 1, name: "High" }],
  runFrequencies: [{ id: 2, name: "Daily" }],
  fragilities: [{ id: 3, name: "Medium" }],
  maintenanceSchedules: [{ id: 4, name: "Quarterly" }],
  fragilityTags: [{ id: 5, name: "Complex SQL" }],
  maintenanceLogStatuses: [{ id: 6, name: "Reviewed" }],
}

const report: ReportDetail = {
  id: 16,
  name: "daily_emergency_department_census",
  displayTitle: "Daily Emergency Department Census",
  canEditDocumentation: true,
  document: {
    developerDescription: "Developer copy",
    keyAssumptions: "Assumptions copy",
  },
}

describe("ReportEditWizard", () => {
  it("loads seeded description fields on the first step", () => {
    render(
      <ReportEditWizard
        report={report}
        cancelHref="/reports?id=16"
        lookupOptions={lookupOptions}
      />,
    )

    expect(screen.getByDisplayValue("Developer copy")).toBeInTheDocument()
    expect(screen.getByDisplayValue("Assumptions copy")).toBeInTheDocument()
    expect(screen.getByRole("button", { name: "Description" })).toBeInTheDocument()
    expect(screen.getByRole("button", { name: "Meta" })).toBeInTheDocument()
  })
})
