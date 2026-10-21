import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, expect, it, vi } from "vitest"
import { ReportEditWizard } from "@/components/reports/report-edit-wizard"
import type { ReportDetail } from "@/lib/reports/types"

vi.mock("@/app/reports/actions", () => ({
  searchReportTermsAction: vi.fn(async () => []),
  searchReportCollectionsAction: vi.fn(async () => []),
  searchReportUsersAction: vi.fn(async () => []),
  updateReportAction: vi.fn(async () => ({ data: {} })),
}))

vi.mock("next/image", () => ({
  default: (props: { alt?: string }) => <img alt={props.alt ?? ""} />,
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
    expect(screen.getByRole("link", { name: "Description" })).toHaveAttribute("aria-current", "step")
    expect(window.location.hash).toBe("#description")
  })

  it("navigates to the meta step from the step nav", async () => {
    const user = userEvent.setup()
    window.history.replaceState(null, "", "/reports/edit?id=16")

    render(
      <ReportEditWizard
        report={report}
        cancelHref="/reports?id=16"
        lookupOptions={lookupOptions}
      />,
    )

    await user.click(screen.getByRole("link", { name: "Meta" }))

    expect(window.location.hash).toBe("#meta")
    expect(screen.getByLabelText("GitLab project URL")).toBeInTheDocument()
    expect(screen.queryByLabelText("Developer description")).not.toBeInTheDocument()
  })

  it("lists existing images on the images step", async () => {
    const user = userEvent.setup()
    const reportWithImages: ReportDetail = {
      ...report,
      images: [{ id: 1, source: "https://example.com/report.png" }],
    }

    render(
      <ReportEditWizard
        report={reportWithImages}
        cancelHref="/reports?id=16"
        lookupOptions={lookupOptions}
      />,
    )

    await user.click(screen.getByRole("link", { name: "Images" }))

    expect(window.location.hash).toBe("#images")
    expect(screen.getByAltText("Report image 1")).toBeInTheDocument()
  })
})
