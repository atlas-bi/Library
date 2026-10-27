import { render, screen } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"
import { ReportRelationshipsSection } from "@/components/reports/report-relationships-section"
import type { ReportDetail } from "@/lib/reports/types"

vi.mock("@/components/snippets/report-snippet-card", () => ({
  ReportSnippetCard: ({ report }: { report: { id: number; name?: string | null } }) => (
    <div>Report snippet {report.name}</div>
  ),
}))

vi.mock("@/components/snippets/collection-snippet-card", () => ({
  CollectionSnippetCard: ({ collection }: { collection: { name: string } }) => (
    <div>Collection snippet {collection.name}</div>
  ),
}))

const baseReport: ReportDetail = {
  id: 16,
  name: "daily_emergency_department_census",
  canViewGroups: true,
}

describe("ReportRelationshipsSection", () => {
  it("renders groups table columns and linked collection cards", () => {
    render(
      <ReportRelationshipsSection
        report={{
          ...baseReport,
          groups: [{ id: 1, name: "Clinical Ops", type: "Distribution", source: "AD" }],
          collections: [{ id: 1, name: "Patient Flow Command Center" }],
          children: [{ id: 20, name: "Child report" }],
        }}
      />,
    )

    expect(screen.getByRole("table", { name: "Related report groups" })).toBeInTheDocument()
    expect(screen.getByText("Source")).toBeInTheDocument()
    expect(screen.getByText("Clinical Ops")).toBeInTheDocument()
    expect(screen.getByText("Collection snippet Patient Flow Command Center")).toBeInTheDocument()
    expect(screen.getByText("Report snippet Child report")).toBeInTheDocument()
  })
})
