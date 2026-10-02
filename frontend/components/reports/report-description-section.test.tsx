import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { ReportDescriptionSection } from "@/components/reports/report-description-section"
import type { ReportDetail } from "@/lib/reports/types"

describe("ReportDescriptionSection", () => {
  it("renders Razor description subsections when content exists", () => {
    const report: ReportDetail = {
      id: 16,
      name: "census",
      description: "System overview",
      detailedDescription: "Detailed overview",
      document: {
        developerDescription: "Developer notes",
        keyAssumptions: "Assumes daily load",
      },
    }

    render(<ReportDescriptionSection report={report} />)

    expect(screen.getByRole("heading", { name: "Description" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Developer Description" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "Key Assumptions" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "System Description" })).toBeInTheDocument()
    expect(screen.getByRole("heading", { name: "System Detailed Description" })).toBeInTheDocument()
    expect(screen.getByText("Developer notes")).toBeInTheDocument()
  })
})
