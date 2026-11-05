import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { HomeRecentTermsView } from "@/components/home/home-recent-terms-view"

describe("HomeRecentTermsView", () => {
  it("shows an empty state when both lists are empty", () => {
    render(<HomeRecentTermsView status="ready" data={{ approved: [], pending: [] }} />)

    expect(screen.getByText("No recent terms to show.")).toBeInTheDocument()
  })

  it("renders approved and pending lists with term links", () => {
    render(
      <HomeRecentTermsView
        status="ready"
        data={{
          approved: [{ id: 1, name: "Approved_Term" }],
          pending: [{ id: 2, name: "Pending_Term" }],
        }}
      />,
    )

    expect(screen.getByText("Approved")).toBeInTheDocument()
    expect(screen.getByText("Pending")).toBeInTheDocument()
    expect(screen.getByRole("link", { name: /Approved Term/i })).toHaveAttribute(
      "href",
      "/terms?id=1",
    )
    expect(screen.getByRole("link", { name: "Pending Term" })).toHaveAttribute(
      "href",
      "/terms?id=2",
    )
  })

  it("shows a load failure message", () => {
    render(<HomeRecentTermsView status="error" error="unknown" />)

    expect(screen.getByText("Unable to load recent terms.")).toBeInTheDocument()
  })
})
