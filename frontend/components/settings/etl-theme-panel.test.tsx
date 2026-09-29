import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { updateEtlAction, updateThemeAction } from "@/app/settings/actions"
import { EtlThemePanel } from "./etl-theme-panel"

vi.mock("@/app/settings/actions", () => ({
  updateEtlAction: vi.fn(),
  updateThemeAction: vi.fn(),
}))

describe("EtlThemePanel", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("saves ETL script successfully", async () => {
    const user = userEvent.setup()
    vi.mocked(updateEtlAction).mockResolvedValueOnce({ data: {} })

    render(
      <EtlThemePanel
        initialEtl="SELECT 1"
        initialTheme={null}
        defaultEtl="SELECT default"
        etlOnly
      />,
    )

    await user.click(screen.getByRole("button", { name: /save etl/i }))

    await waitFor(() => {
      expect(updateEtlAction).toHaveBeenCalledWith("SELECT 1")
      expect(screen.getByText("ETL script saved.")).toBeInTheDocument()
    })
  })

  it("shows forbidden error when ETL save fails", async () => {
    const user = userEvent.setup()
    vi.mocked(updateEtlAction).mockResolvedValueOnce({
      error: "You do not have permission to view this content.",
    })

    render(<EtlThemePanel initialEtl="SELECT 1" initialTheme={null} defaultEtl={null} etlOnly />)

    await user.click(screen.getByRole("button", { name: /save etl/i }))

    await waitFor(() => {
      expect(
        screen.getByText("You do not have permission to view this content."),
      ).toBeInTheDocument()
    })
  })

  it("restores default ETL into the textarea", async () => {
    const user = userEvent.setup()

    render(
      <EtlThemePanel
        initialEtl="custom sql"
        initialTheme={null}
        defaultEtl="SELECT default"
        etlOnly
      />,
    )

    const textarea = screen.getByLabelText(/sql script/i)
    expect(textarea).toHaveValue("custom sql")

    await user.click(screen.getByRole("button", { name: /restore default/i }))

    expect(textarea).toHaveValue("SELECT default")
  })

  it("saves theme CSS successfully", async () => {
    const user = userEvent.setup()
    vi.mocked(updateThemeAction).mockResolvedValueOnce({ data: {} })

    render(
      <EtlThemePanel
        initialEtl={null}
        initialTheme="body { color: red; }"
        defaultEtl={null}
        themeOnly
      />,
    )

    await user.click(screen.getByRole("button", { name: /save theme/i }))

    await waitFor(() => {
      expect(updateThemeAction).toHaveBeenCalledWith("body { color: red; }")
      expect(screen.getByText("Theme saved.")).toBeInTheDocument()
    })
  })
})
