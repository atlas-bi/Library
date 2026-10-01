import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { ReportQuerySection } from "@/components/reports/report-query-section"
import type { ReportDetail } from "@/lib/reports/types"

describe("ReportQuerySection", () => {
  it("renders query text from the API query field", () => {
    const report: ReportDetail = {
      id: 16,
      name: "census",
      queries: [
        {
          id: 1,
          name: "Daily census",
          language: "sql",
          query: "SELECT 1 FROM census",
        },
      ],
    }

    render(<ReportQuerySection report={report} />)

    expect(screen.getByText("Query")).toBeInTheDocument()
    expect(screen.getByText("SELECT 1 FROM census")).toBeInTheDocument()
  })

  it("falls back to legacy source field when query is absent", () => {
    render(
      <ReportQuerySection
        report={{
          id: 1,
          name: "legacy",
          componentQueries: [{ id: 2, name: "Metric", source: "SELECT metric" }],
        }}
      />,
    )

    expect(screen.getByText("SELECT metric")).toBeInTheDocument()
  })
})
