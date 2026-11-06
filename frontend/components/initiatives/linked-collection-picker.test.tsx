import { fireEvent, render, screen, waitFor } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"
import { searchInitiativeCollectionsAction } from "@/lib/initiatives/actions"
import { LinkedCollectionPicker } from "./linked-collection-picker"

vi.mock("@/lib/initiatives/actions", () => ({
  searchInitiativeCollectionsAction: vi.fn(),
}))

describe("LinkedCollectionPicker", () => {
  it("returns the selected collection item to the form", async () => {
    vi.mocked(searchInitiativeCollectionsAction).mockResolvedValue({
      data: [{ id: 7, name: "Reports", description: "Useful reports" }],
      error: null,
    })
    const onSelectAction = vi.fn()

    render(<LinkedCollectionPicker onSelectAction={onSelectAction} />)
    fireEvent.change(screen.getByPlaceholderText("Search collections..."), {
      target: { value: "reports" },
    })
    fireEvent.click(screen.getByRole("button", { name: "Search" }))

    await waitFor(() => expect(screen.getByText("Reports")).toBeDefined())
    fireEvent.click(screen.getByRole("button", { name: "Select" }))

    expect(onSelectAction).toHaveBeenCalledWith({
      id: 7,
      name: "Reports",
      description: "Useful reports",
    })
  })
})
