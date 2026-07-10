import { render, screen } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"
import { InitiativeDetail } from "./initiative-detail"
import type { InitiativeDetailDto } from "@/lib/initiatives/types"

// Mock the next/navigation router
vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: vi.fn(),
    refresh: vi.fn(),
  }),
}))

describe("InitiativeDetail", () => {
  const mockData: InitiativeDetailDto = {
    id: 1,
    name: "Test Initiative",
    operationOwner: { id: 10, fullName: "Alice Operator", username: "alice" },
    executiveOwner: { id: 20, fullName: "Bob Exec", username: "bob" },
    lastUpdatedBy: { id: 30, fullName: "Charlie Editor", username: "charlie" },
  }

  it("renders user links when canViewOtherUser is true (default)", () => {
    render(<InitiativeDetail data={mockData} canViewOtherUser={true} />)
    
    // They should be links (<a> tags)
    expect(screen.getByRole("link", { name: "Alice Operator" })).toBeDefined()
    expect(screen.getByRole("link", { name: "Bob Exec" })).toBeDefined()
    expect(screen.getByRole("link", { name: "Charlie Editor" })).toBeDefined()
  })

  it("renders user plain text when canViewOtherUser is false", () => {
    render(<InitiativeDetail data={mockData} canViewOtherUser={false} />)
    
    // They should NOT be links
    expect(screen.queryByRole("link", { name: "Alice Operator" })).toBeNull()
    expect(screen.queryByRole("link", { name: "Bob Exec" })).toBeNull()
    expect(screen.queryByRole("link", { name: "Charlie Editor" })).toBeNull()

    // The text should still exist on the page
    expect(screen.getByText("Alice Operator")).toBeDefined()
    expect(screen.getByText("Bob Exec")).toBeDefined()
    expect(screen.getByText("Charlie Editor")).toBeDefined()
  })
})
