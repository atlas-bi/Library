import { fireEvent, render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { ReportImagesSection } from "@/components/reports/report-images-section"
import type { ReportDetail } from "@/lib/reports/types"

const report: ReportDetail = {
  id: 16,
  name: "daily_emergency_department_census",
  images: [
    { id: 22, ordinal: 1, source: null },
    { id: 21, ordinal: 0, source: null },
  ],
}

describe("ReportImagesSection", () => {
  it("loads uploaded images in ordinal order from the thumbnail API", () => {
    render(<ReportImagesSection report={report} />)

    const images = screen.getAllByRole("img")
    expect(images.map((img) => img.getAttribute("src"))).toEqual([
      "/api/reports/16/thumbnail?size=1200x400&imgId=21",
      "/api/reports/16/thumbnail?size=1200x400&imgId=22",
    ])
    expect(screen.getAllByRole("link")[0]?.getAttribute("href")).toBe(
      "/api/reports/16/thumbnail?size=1200x2000&imgId=21",
    )
  })

  it("falls back to the placeholder when an image is missing", () => {
    render(<ReportImagesSection report={report} />)

    const [first] = screen.getAllByRole("img")
    if (!first) throw new Error("image not rendered")
    fireEvent.error(first)
    fireEvent.error(first)

    expect(first.getAttribute("src")).toBe("/api/reports/images/placeholder?size=1200x400")
  })

  it("renders nothing without images", () => {
    const { container } = render(<ReportImagesSection report={{ ...report, images: [] }} />)

    expect(container.innerHTML).toBe("")
  })
})
