import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import {
  updateSearchReportTypeTextAction,
  updateSearchVisibilityAction,
} from "@/app/settings/actions"
import { SearchSettingsPanel } from "./search-settings-panel"

vi.mock("@/app/settings/actions", () => ({
  updateSearchVisibilityAction: vi.fn(),
  updateSearchReportTypeTextAction: vi.fn(),
}))

const INITIAL_DATA = {
  visibility: { users: "Y", groups: "N", terms: "Y", initiatives: "Y", collections: "Y" },
  reportTypes: [{ id: 7, name: "Dashboard", shortName: "Dash", visible: true }],
}

describe("SearchSettingsPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("renders object visibility toggles", () => {
    render(<SearchSettingsPanel initialData={INITIAL_DATA} />)
    expect(screen.getByText("Users")).toBeInTheDocument()
    expect(screen.getByText("Groups")).toBeInTheDocument()
  })

  it("shows forbidden error when visibility update fails", async () => {
    const user = userEvent.setup()
    vi.mocked(updateSearchVisibilityAction).mockResolvedValueOnce({
      error: "You do not have permission to view this content.",
    })

    render(<SearchSettingsPanel initialData={INITIAL_DATA} />)

    const checkboxes = screen.getAllByRole("checkbox")
    await user.click(checkboxes[1])

    await waitFor(() => {
      expect(
        screen.getByText("You do not have permission to view this content."),
      ).toBeInTheDocument()
    })
  })

  it("saves report type text override", async () => {
    const user = userEvent.setup()
    vi.mocked(updateSearchReportTypeTextAction).mockResolvedValueOnce({ data: {} })

    render(<SearchSettingsPanel initialData={INITIAL_DATA} />)

    const input = screen.getByPlaceholderText("Dashboard")
    await user.clear(input)
    await user.type(input, "Executive Dashboard")
    await user.click(screen.getByRole("button", { name: /^save$/i }))

    await waitFor(() => {
      expect(updateSearchReportTypeTextAction).toHaveBeenCalledWith(7, "Executive Dashboard")
    })
  })
})
