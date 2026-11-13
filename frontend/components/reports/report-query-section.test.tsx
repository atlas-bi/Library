import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, expect, it, vi } from "vitest"
import { ReportQuerySection } from "@/components/reports/report-query-section"
import type { ReportDetail } from "@/lib/reports/types"

describe("ReportQuerySection", () => {
  it("numbers primary queries and copies query text", async () => {
    const user = userEvent.setup()
    const writeText = vi.fn().mockResolvedValue(undefined)
    vi.stubGlobal("navigator", { clipboard: { writeText } })

    const report = {
      id: 16,
      name: "report",
      queries: [{ id: 1, name: "Daily census", query: "SELECT 1" }],
    } satisfies ReportDetail

    render(<ReportQuerySection report={report} />)

    expect(screen.getByRole("heading", { name: /Query #1 \| Daily census/i })).toBeInTheDocument()
    await user.click(screen.getByRole("button", { name: "Copy Query" }))
    expect(writeText).toHaveBeenCalledWith("SELECT 1")
  })

  it("renders component queries with report links and descriptions", () => {
    const report = {
      id: 16,
      name: "report",
      componentQueries: [
        {
          id: 9,
          reportObjectId: 99,
          name: "Metric query",
          description: "Child report description",
          query: "SELECT 2",
          epicMasterFile: "IDN",
          epicRecordId: 42,
        },
      ],
    } satisfies ReportDetail

    render(<ReportQuerySection report={report} />)

    expect(screen.getByRole("link", { name: /Metric query \(IDN 42\)/i })).toHaveAttribute(
      "href",
      "/reports?id=99",
    )
    expect(screen.getByText("Child report description")).toBeInTheDocument()
  })
})
