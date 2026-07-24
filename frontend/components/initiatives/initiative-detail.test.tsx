import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, expect, it, vi, beforeEach } from "vitest"
import type { InitiativeDetailDto } from "@/lib/initiatives/types"
import { InitiativeDetail } from "./initiative-detail"
import { toggleStarAction } from "@/lib/initiatives/actions"

// Mock the actions module
vi.mock("@/lib/initiatives/actions", () => ({
  deleteInitiativeAction: vi.fn(),
  toggleStarAction: vi.fn(),
}))

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

describe("InitiativeDetail - Star Toggle", () => {
  const mockData: InitiativeDetailDto = {
    id: 1,
    name: "Test Initiative",
    isStarred: false,
    starCount: 5,
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("applies server values on successful toggle", async () => {
    const user = userEvent.setup()
    vi.mocked(toggleStarAction).mockResolvedValue({
      data: { type: "Initiative", id: 1, isStarred: true, count: 6 },
      error: null,
    })

    render(<InitiativeDetail data={mockData} />)

    const starButton = screen.getByTitle("Star this initiative")
    await user.click(starButton)

    // After the action resolves, count should match server response
    expect(await screen.findByText("6")).toBeDefined()
  })

  it("reverts UI on failed toggle", async () => {
    const user = userEvent.setup()
    const alertMock = vi.spyOn(window, "alert").mockImplementation(() => {})
    vi.mocked(toggleStarAction).mockResolvedValue({
      data: null,
      error: "service_unavailable",
    })

    render(<InitiativeDetail data={mockData} />)

    // Should show initial count of 5
    expect(screen.getByText("5")).toBeDefined()

    const starButton = screen.getByTitle("Star this initiative")
    await user.click(starButton)

    // After error, should revert back to 5
    expect(await screen.findByText("5")).toBeDefined()
    expect(alertMock).toHaveBeenCalledWith("Error updating star: service_unavailable")

    alertMock.mockRestore()
  })
})

