import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { HomeRecentUpdatesView } from "@/components/home/home-recent-updates-view"

describe("HomeRecentUpdatesView", () => {
  it("shows an empty state when all lists are empty", () => {
    render(
      <HomeRecentUpdatesView
        status="ready"
        data={{
          reports: { items: [], failed: false },
          initiatives: { items: [], failed: false },
          collections: { items: [], failed: false },
        }}
      />,
    )

    expect(screen.getByText("No recent updates to show.")).toBeInTheDocument()
  })

  it("renders names as returned and links to detail pages", () => {
    render(
      <HomeRecentUpdatesView
        status="ready"
        data={{
          reports: { items: [{ id: 10, name: "Report_10" }], failed: false },
          initiatives: { items: [{ id: 20, name: "Initiative_20" }], failed: false },
          collections: { items: [{ id: 30, name: "Collection_30" }], failed: false },
        }}
      />,
    )

    expect(screen.getByRole("link", { name: "Report_10" })).toHaveAttribute("href", "/reports?id=10")
    expect(screen.getByRole("link", { name: "Initiative_20" })).toHaveAttribute(
      "href",
      "/initiatives?id=20",
    )
    expect(screen.getByRole("link", { name: "Collection_30" })).toHaveAttribute(
      "href",
      "/collections?id=30",
    )
  })

  it("shows a load failure message", () => {
    render(<HomeRecentUpdatesView status="error" error="unknown" />)

    expect(screen.getByText("Unable to load recent updates.")).toBeInTheDocument()
  })
})
