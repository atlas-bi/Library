import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { HomeStarsPanelView } from "@/components/home/home-stars-panel"

describe("HomeStarsPanelView", () => {
  it("renders richer card content with tags, image, and footer actions", () => {
    render(
      <HomeStarsPanelView
        panel={{
          kind: "stars",
          title: "Stars",
          folders: [{ id: "all", label: "All", count: 1 }],
          filters: [{ id: "reports", label: "Reports" }],
          cards: [
            {
              id: 7,
              href: "/reports?id=7",
              title: "Executive Dashboard",
              typeLabel: "Rpt",
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
    expect(screen.getByLabelText("Open report profile")).toBeInTheDocument()
  })

  it("renders the plain empty-state message when there are no favorites or suggestions", () => {
    render(
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
    render(
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
})
