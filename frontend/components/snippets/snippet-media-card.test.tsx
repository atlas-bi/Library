import { fireEvent, render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { SnippetMediaCard } from "./snippet-media-card"

describe("SnippetMediaCard", () => {
  it("loads a report thumbnail from the image API and falls back when it is missing", () => {
    render(
      <SnippetMediaCard
        title="Daily Census"
        tags={["report"]}
        excerpt="excerpt"
        imageSrc="/api/reports/16/thumbnail?size=128x128"
        imageFallbackSrc="/api/reports/images/placeholder?size=128x128"
        imageAlt="Daily Census thumbnail"
      />,
    )

    const img = screen.getByRole("img", { name: "Daily Census thumbnail" })
    expect(img.getAttribute("src")).toBe("/api/reports/16/thumbnail?size=128x128")
    fireEvent.error(img)
    expect(img.getAttribute("src")).toBe("/api/reports/images/placeholder?size=128x128")
  })
})
