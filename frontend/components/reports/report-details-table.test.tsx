import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { ReportDetailsTable } from "@/components/reports/report-details-table"
import type { ReportDetail } from "@/lib/reports/types"

const baseReport: ReportDetail = {
  id: 16,
  name: "daily_emergency_department_census",
  displayTitle: "Daily Emergency Department Census",
  typeName: "SSRS Report",
  typeShortName: "SSRS",
  visibleInSearch: true,
  orphanedReportObjectYn: "Y",
  document: {
    executiveVisibilityYn: "Y",
    enabledForHyperspace: "N",
    doNotPurge: "Y",
    hidden: "Y",
    organizationalValue: { id: 1, name: "High" },
    fragility: { id: 2, name: "Medium" },
    maintenanceSchedule: { id: 3, name: "Quarterly" },
    estimatedRunFrequency: { id: 4, name: "Daily" },
    fragilityTags: [{ id: 1, name: "Complex SQL" }],
    operationalOwner: { id: 2, fullName: "Maya Patel", username: "mpatel", email: "mpatel@example.com" },
  },
  objectTags: [{ id: 1, name: "Analytics Certified" }],
}

describe("ReportDetailsTable", () => {
  it("renders Razor parity detail rows when values are present", () => {
    render(<ReportDetailsTable report={baseReport} canViewUserProfiles />)

    expect(screen.getByText("Executive Visibility")).toBeInTheDocument()
    expect(screen.getByText("Enabled for Hyperspace")).toBeInTheDocument()
    expect(screen.getByText("Do Not Purge")).toBeInTheDocument()
    expect(screen.getByText("Hidden?")).toBeInTheDocument()
    expect(screen.getByText("Orphaned?")).toBeInTheDocument()
    expect(screen.getByText("Fragility Rating")).toBeInTheDocument()
    expect(screen.getByText("Organizational Value Rating")).toBeInTheDocument()
    expect(screen.getByText("Fragility Tags")).toBeInTheDocument()
    expect(screen.getByText("Complex SQL")).toBeInTheDocument()
    expect(screen.getByText("Report Tags")).toBeInTheDocument()
    expect(screen.getByText("Analytics Certified")).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "Maya Patel" })).toHaveAttribute("href", "/users?id=2")
  })

  it("omits optional rows when values are absent", () => {
    render(
      <ReportDetailsTable
        report={{
          id: 1,
          name: "simple_report",
          typeShortName: "Web",
        }}
      />,
    )

    expect(screen.queryByText("Executive Visibility")).not.toBeInTheDocument()
    expect(screen.queryByText("Enabled for Hyperspace")).not.toBeInTheDocument()
    expect(screen.queryByText("Orphaned?")).not.toBeInTheDocument()
  })
})
