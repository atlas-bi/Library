import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { createTagAction, deleteTagAction } from "@/app/settings/actions"
import { TagsSettingsPanel } from "./tags-settings-panel"

vi.mock("@/app/settings/actions", () => ({
  createTagAction: vi.fn(),
  deleteTagAction: vi.fn(),
}))

const EMPTY_PROPS = {
  organizationalValues: [],
  estimatedRunFrequencies: [],
  fragilities: [],
  fragilityTags: [],
  maintenanceSchedules: [],
  maintenanceLogStatuses: [],
  financialImpacts: [],
  strategicImportances: [],
  tags: [],
}

describe("TagsSettingsPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.spyOn(window, "confirm").mockReturnValue(true)
  })

  it("adds a tag successfully", async () => {
    const user = userEvent.setup()
    vi.mocked(createTagAction).mockResolvedValueOnce({
      data: { id: 9, name: "Critical", description: null, used: 0 },
    })

    render(<TagsSettingsPanel {...EMPTY_PROPS} />)

    const input = screen.getByPlaceholderText(/add organizational value/i)
    await user.type(input, "Critical")
    await user.click(screen.getAllByRole("button", { name: /^add$/i })[0])

    await waitFor(() => {
      expect(createTagAction).toHaveBeenCalledWith("organizational-values", { name: "Critical" })
      expect(screen.getByText("Critical")).toBeInTheDocument()
    })
  })

  it("shows forbidden error when create fails", async () => {
    const user = userEvent.setup()
    vi.mocked(createTagAction).mockResolvedValueOnce({
      error: "You do not have permission to view this content.",
    })

    render(<TagsSettingsPanel {...EMPTY_PROPS} />)

    const input = screen.getByPlaceholderText(/add organizational value/i)
    await user.type(input, "Blocked")
    await user.click(screen.getAllByRole("button", { name: /^add$/i })[0])

    await waitFor(() => {
      expect(
        screen.getByText("You do not have permission to view this content."),
      ).toBeInTheDocument()
    })
  })

  it("deletes a tag successfully", async () => {
    const user = userEvent.setup()
    vi.mocked(deleteTagAction).mockResolvedValueOnce({ data: {} })

    render(
      <TagsSettingsPanel
        {...EMPTY_PROPS}
        organizationalValues={[{ id: 3, name: "Legacy", description: null, used: 2 }]}
      />,
    )

    await user.click(screen.getByRole("button", { name: /delete legacy/i }))

    await waitFor(() => {
      expect(deleteTagAction).toHaveBeenCalledWith("organizational-values", 3)
      expect(screen.queryByText("Legacy")).not.toBeInTheDocument()
    })
  })
})
