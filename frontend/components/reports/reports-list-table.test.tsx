import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { ReportsListTable } from "@/components/reports/reports-list-table"

describe("ReportsListTable", () => {
  it("renders report type from the API type field", () => {
    render(
      <ReportsListTable
        reports={[
          {
            id: 16,
            name: "Daily Emergency Department Census",
            type: "SSRS",
            lastModified: "2026-01-15T00:00:00Z",
          },
        ]}
      />,
    )

    expect(screen.getByRole("link", { name: "Daily Emergency Department Census" })).toHaveAttribute(
      "href",
      "/reports?id=16",
    )
    expect(screen.getByText("SSRS")).toBeInTheDocument()
  })

  it("shows an empty state when there are no reports", () => {
    render(<ReportsListTable reports={[]} />)

    expect(screen.getByText("No reports found.")).toBeInTheDocument()
  })
})
