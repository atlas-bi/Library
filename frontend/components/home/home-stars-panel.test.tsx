import { render, screen } from "@testing-library/react"
import type { ReactNode } from "react"
import { describe, expect, it } from "vitest"
import { HomeStarsPanelView } from "@/components/home/home-stars-panel"
import { TooltipProvider } from "@/components/ui/tooltip"

function renderWithTooltipProvider(ui: ReactNode) {
  return render(<TooltipProvider>{ui}</TooltipProvider>)
}

describe("HomeStarsPanelView", () => {
  it("renders richer card content with tags, image, and footer actions", () => {
    renderWithTooltipProvider(
      <HomeStarsPanelView
        panel={{
          kind: "stars",
          title: "Stars",
          folders: [{ id: "all", label: "All", count: 1 }],
          filters: [{ id: "report", label: "Reports" }],
          cards: [
            {
              id: 7,
              href: "/reports?id=7",
              title: "Executive Dashboard",
              itemType: "report",
              typeLabel: "Report",
              description: "Leadership reporting summary...",
              thumbnailUrl: "http://localhost:5000/data/img?handler=Thumb&id=7&size=128x128",
              tags: [
                { name: "Analytics Certified", slug: "analytics-certified", showInHeader: true },
              ],
              isCertified: true,
              starCount: 4,
              isStarred: true,
              canOpenDetails: true,
              canRun: true,
              runUrl: "http://localhost:5000/run/7",
              canEdit: true,
              editUrl: "http://localhost:5000/edit/7",
              canManage: true,
              manageUrl: "http://localhost:5000/manage/7",
              canOpenProfile: true,
              canShare: true,
              canRequestAccess: true,
            },
          ],
        }}
      />,
    )

    expect(screen.getByText("Executive Dashboard")).toBeInTheDocument()
    expect(screen.getByText("Analytics Certified")).toBeInTheDocument()
    expect(screen.getByRole("img", { name: "Executive Dashboard thumbnail" })).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "Run report" })).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "Edit" })).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "Manage" })).toBeInTheDocument()
    expect(screen.getByRole("button", { name: "Open report profile" })).toBeInTheDocument()
  })

  it("renders the plain empty-state message when there are no favorites or suggestions", () => {
    renderWithTooltipProvider(
      <HomeStarsPanelView
        panel={{
          kind: "stars",
          title: "Stars",
          emptyMessage: "You don't have any favorites! Search to get started.",
          folders: [{ id: "all", label: "All", count: 0 }],
          filters: [],
          cards: [],
        }}
      />,
    )

    expect(
      screen.getByText("You don't have any favorites! Search to get started."),
    ).toBeInTheDocument()
  })

  it("renders the Razor-style suggestion fallback when suggested reports are present", () => {
    renderWithTooltipProvider(
      <HomeStarsPanelView
        panel={{
          kind: "stars",
          title: "Stars",
          folders: [{ id: "all", label: "All", count: 0 }],
          filters: [],
          cards: [
            {
              id: 9,
              href: "/reports?id=9",
              title: "Operations Summary",
              itemType: "report",
              typeLabel: "Report",
              description: "Open to view details.",
              canOpenDetails: true,
              isStarred: false,
            },
          ],
          isSuggestionFallback: true,
          suggestionHeading: "You don't have any favorites! Here's some reports you've used.",
        }}
      />,
    )

    expect(
      screen.getByText("You don't have any favorites! Here's some reports you've used."),
    ).toBeInTheDocument()
    expect(screen.getByText("Operations Summary")).toBeInTheDocument()
  })

  it("renders the Shared With Me rail", () => {
    renderWithTooltipProvider(
      <HomeStarsPanelView
        panel={{
          kind: "stars",
          title: "Stars",
          folders: [{ id: "all", label: "All", count: 1 }],
          filters: [],
          cards: [
            {
              id: 16,
              href: "/reports?id=16",
              title: "Daily Emergency Department Census",
              itemType: "report",
              typeLabel: "Report",
              description: "Census report",
            },
          ],
          sharedWithMe: [
            {
              id: 1,
              name: "Shared Census Link",
              href: "/reports?id=16",
              sharedFrom: "Maya Patel",
            },
          ],
        }}
      />,
    )

    expect(screen.getByText("Shared With Me")).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "Shared Census Link" })).toBeInTheDocument()
  })

  it("applies collection card styling distinct from report cards", () => {
    const { container } = renderWithTooltipProvider(
      <HomeStarsPanelView
        panel={{
          kind: "stars",
          title: "Stars",
          folders: [{ id: "all", label: "All", count: 2 }],
          filters: [],
          cards: [
            {
              id: 16,
              href: "/reports?id=16",
              title: "Daily Emergency Department Census",
              itemType: "report",
              typeLabel: "Report",
              description: "Census report",
            },
            {
              id: 1,
              href: "/collections?id=1",
              title: "Patient Flow Command Center",
              itemType: "collection",
              typeLabel: "Collection",
              description: "Collection overview",
            },
          ],
        }}
      />,
    )

    expect(container.querySelector(".atlas-snippet-gold-card")).toBeTruthy()
    expect(container.querySelector(".atlas-home-card")).toBeTruthy()
  })
})
