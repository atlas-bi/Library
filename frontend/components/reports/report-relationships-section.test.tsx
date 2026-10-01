import { render, screen } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"
import { ReportRelationshipsSection } from "@/components/reports/report-relationships-section"

vi.mock("@/components/snippets/report-snippet-card", () => ({
  ReportSnippetCard: ({ report }: { report: { id: number; name?: string | null } }) => (
    <div data-testid={`report-snippet-${report.id}`}>{report.name}</div>
  ),
}))

vi.mock("@/components/snippets/collection-snippet-card", () => ({
  CollectionSnippetCard: ({ collection }: { collection: { id: number; name: string } }) => (
    <div data-testid={`collection-snippet-${collection.id}`}>{collection.name}</div>
  ),
}))

describe("ReportRelationshipsSection", () => {
  it("renders groups table, linked report cards, and collection cards", () => {
    render(
      <ReportRelationshipsSection
        report={{
          id: 16,
          name: "census",
          canViewGroups: true,
          groups: [{ id: 1, name: "ED Analytics", type: "Distribution" }],
          children: [{ id: 20, name: "Child Report", attachmentCount: 1 }],
          parents: [{ id: 21, displayTitle: "Parent Dashboard", attachmentCount: 0 }],
          collections: [{ id: 30, name: "Emergency Metrics" }],
        }}
      />,
    )

    expect(screen.getByRole("heading", { name: "Relationships" })).toBeInTheDocument()
    expect(screen.getByRole("columnheader", { name: "Group Name" })).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "ED Analytics" })).toHaveAttribute(
      "href",
      "/groups?id=1",
    )
    expect(screen.getByText("Distribution")).toBeInTheDocument()
    expect(screen.getByTestId("report-snippet-20")).toHaveTextContent("Child Report")
    expect(screen.getByTestId("report-snippet-21")).toHaveTextContent("Parent Dashboard")
    expect(screen.getByTestId("collection-snippet-30")).toHaveTextContent("Emergency Metrics")
    expect(screen.getByRole("heading", { name: "Linked Collections" })).toBeInTheDocument()
  })

  it("returns null when there is no relationship data", () => {
    const { container } = render(<ReportRelationshipsSection report={{ id: 1, name: "solo" }} />)

    expect(container).toBeEmptyDOMElement()
  })
})
