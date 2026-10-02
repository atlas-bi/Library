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

vi.mock("@/components/reports/report-image-upload", () => ({
  ReportImageUpload: () => <div data-testid="report-image-upload">Image upload</div>,
}))

vi.mock("@/components/content/markdown-field", () => ({
  MarkdownField: ({
    id,
    label,
    value,
    onChange,
  }: {
    id: string
    label: string
    value: string
    onChange: (value: string) => void
  }) => (
    <label htmlFor={id}>
      {label}
      <textarea
        id={id}
        aria-label={label}
        value={value}
        onChange={(event) => {
          onChange(event.target.value)
        }}
      />
    </label>
  ),
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
    gitLabProjectUrl: "https://gitlab.com/example/repo",
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

    expect(screen.getByLabelText("Description")).toHaveValue("Developer copy")
    expect(screen.getByLabelText("Key assumptions")).toHaveValue("Assumptions copy")
    expect(screen.getByRole("button", { name: "Description" })).toHaveAttribute(
      "aria-current",
      "step",
    )
    expect(screen.getByRole("button", { name: "Meta" })).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "Cancel" })).toHaveAttribute("href", "/reports?id=16")
  })

  it("shows meta fields when the Meta step is selected", async () => {
    const user = userEvent.setup()
    render(
      <ReportEditWizard
        report={report}
        cancelHref="/reports?id=16"
        lookupOptions={lookupOptions}
      />,
    )

    await user.click(screen.getByRole("button", { name: "Meta" }))

    expect(screen.getByLabelText("GitLab project URL")).toHaveValue(
      "https://gitlab.com/example/repo",
    )
  })

  it("shows the images step and keeps description edits when navigating back", async () => {
    const user = userEvent.setup()
    render(
      <ReportEditWizard
        report={report}
        cancelHref="/reports?id=16"
        lookupOptions={lookupOptions}
      />,
    )

    await user.clear(screen.getByLabelText("Description"))
    await user.type(screen.getByLabelText("Description"), "Updated description")

    await user.click(screen.getByRole("button", { name: "Images" }))
    expect(screen.getByTestId("report-image-upload")).toBeInTheDocument()

    await user.click(screen.getByRole("button", { name: "Description" }))
    expect(screen.getByLabelText("Description")).toHaveValue("Updated description")
  })

  it("shows maintenance and complete steps", async () => {
    const user = userEvent.setup()
    render(
      <ReportEditWizard
        report={report}
        cancelHref="/reports?id=16"
        lookupOptions={lookupOptions}
      />,
    )

    await user.click(screen.getByRole("button", { name: "Maintenance" }))
    expect(screen.getByText("Maintenance notes")).toBeInTheDocument()
    expect(screen.getByLabelText("Add maintenance log comment")).toBeInTheDocument()

    await user.click(screen.getByRole("button", { name: "Complete" }))
    expect(screen.getByRole("link", { name: "Return to report" })).toHaveAttribute(
      "href",
      "/reports?id=16",
    )
  })
})
