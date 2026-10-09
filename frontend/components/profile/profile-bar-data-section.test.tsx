import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { ProfileBarDataSection } from "./profile-bar-data-section"
import type { ProfileBarItemDto } from "@/lib/profile/types"

describe("ProfileBarDataSection", () => {
  it("shows row labels from key instead of section title metadata", () => {
    const items: ProfileBarItemDto[] = [
      {
        key: "Jane Analyst",
        count: 3,
        percent: 1 / 3,
        title: "Top Users",
        titleOne: "Top Users",
        titleTwo: "Runs",
      },
    ]

    render(<ProfileBarDataSection title="Top Users" items={items} />)

    expect(screen.getByText("Jane Analyst")).toBeDefined()
    expect(screen.queryByRole("cell", { name: "Top Users" })).toBeNull()
  })

  it("formats fractional percents as whole percentages", () => {
    const items: ProfileBarItemDto[] = [
      {
        key: "Report Alpha",
        count: 2,
        percent: 0.3333333333333333,
        titleOne: "Top Reports",
      },
    ]

    render(<ProfileBarDataSection title="Top Reports" items={items} />)

    expect(screen.getByText("33%")).toBeDefined()
    expect(screen.queryByText("0.3333333333333333%")).toBeNull()
  })
})
