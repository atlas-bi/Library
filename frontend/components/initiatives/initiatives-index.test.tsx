import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { InitiativesIndex } from "./initiatives-index"

describe("InitiativesIndex", () => {
  const emptyData = {
    items: [],
  }

  it("renders the Create Initiative button when canCreateInitiative is true", () => {
    render(<InitiativesIndex data={emptyData} canCreateInitiative={true} />)

    expect(screen.getByRole("button", { name: "+ Create an Initiative" })).toBeDefined()
  })

  it("hides the Create Initiative button when canCreateInitiative is false", () => {
    render(<InitiativesIndex data={emptyData} canCreateInitiative={false} />)

    expect(screen.queryByRole("button", { name: "+ Create an Initiative" })).toBeNull()
  })
})
