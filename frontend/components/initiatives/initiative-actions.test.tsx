import { render, screen } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"
import type { InitiativeDetailDto } from "@/lib/initiatives/types"
import { InitiativeActions } from "./initiative-actions"

// Mock the next/navigation router
vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: vi.fn(),
    refresh: vi.fn(),
  }),
}))

describe("InitiativeActions", () => {
  const baseData: InitiativeDetailDto = {
    id: 1,
    name: "Test Initiative",
  }

  it("renders Edit and Delete when permissions are true", () => {
    const data = { ...baseData, canEditInitiative: true, canDeleteInitiative: true }
    render(<InitiativeActions data={data} />)

    expect(screen.getByRole("button", { name: "Edit" })).toBeDefined()
    expect(screen.getByRole("button", { name: "Delete" })).toBeDefined()
  })

  it("hides Edit and Delete when permissions are false", () => {
    const data = { ...baseData, canEditInitiative: false, canDeleteInitiative: false }
    render(<InitiativeActions data={data} />)

    expect(screen.queryByRole("button", { name: "Edit" })).toBeNull()
    expect(screen.queryByRole("button", { name: "Delete" })).toBeNull()
  })

  it("hides Edit and Delete when permissions are missing (undefined)", () => {
    // baseData has no permission flags
    render(<InitiativeActions data={baseData} />)

    expect(screen.queryByRole("button", { name: "Edit" })).toBeNull()
    expect(screen.queryByRole("button", { name: "Delete" })).toBeNull()
  })
})
