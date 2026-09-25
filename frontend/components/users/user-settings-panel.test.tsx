import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { updateUserSettingsAction } from "@/app/users/actions"
import { UserSettingsPanel } from "./user-settings-panel"

vi.mock("@/app/users/actions", () => ({
  updateUserSettingsAction: vi.fn(),
}))

describe("UserSettingsPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("renders enabled by default when initialShareNotificationEnabled is true", () => {
    render(<UserSettingsPanel initialShareNotificationEnabled={true} />)

    expect(screen.getByRole("switch", { name: /enable share notification/i })).toBeChecked()
  })

  it("renders disabled when initialShareNotificationEnabled is false", () => {
    render(<UserSettingsPanel initialShareNotificationEnabled={false} />)

    expect(screen.getByRole("switch", { name: /enable share notification/i })).not.toBeChecked()
  })

  it("enables share notifications on toggle", async () => {
    const user = userEvent.setup()
    vi.mocked(updateUserSettingsAction).mockResolvedValueOnce({ data: {} })

    render(<UserSettingsPanel initialShareNotificationEnabled={false} />)

    await user.click(screen.getByRole("switch", { name: /enable share notification/i }))

    await waitFor(() => {
      expect(updateUserSettingsAction).toHaveBeenCalledWith({ shareNotificationEnabled: true })
      expect(screen.getByText("Settings saved.")).toBeInTheDocument()
    })
  })

  it("disables share notifications on toggle", async () => {
    const user = userEvent.setup()
    vi.mocked(updateUserSettingsAction).mockResolvedValueOnce({ data: {} })

    render(<UserSettingsPanel initialShareNotificationEnabled={true} />)

    await user.click(screen.getByRole("switch", { name: /enable share notification/i }))

    await waitFor(() => {
      expect(updateUserSettingsAction).toHaveBeenCalledWith({ shareNotificationEnabled: false })
      expect(screen.getByText("Settings saved.")).toBeInTheDocument()
    })
  })

  it("disables the switch while the update is pending", async () => {
    let resolveUpdate!: (value: Awaited<ReturnType<typeof updateUserSettingsAction>>) => void
    vi.mocked(updateUserSettingsAction).mockImplementationOnce(
      () =>
        new Promise((resolve) => {
          resolveUpdate = resolve
        }),
    )

    const user = userEvent.setup()
    render(<UserSettingsPanel initialShareNotificationEnabled={true} />)

    const toggle = screen.getByRole("switch", { name: /enable share notification/i })
    await user.click(toggle)

    expect(toggle).toBeDisabled()

    resolveUpdate({ data: {} })

    await waitFor(() => {
      expect(toggle).not.toBeDisabled()
      expect(screen.getByText("Settings saved.")).toBeInTheDocument()
    })
  })

  it("reverts and shows error when update fails", async () => {
    const user = userEvent.setup()
    vi.mocked(updateUserSettingsAction).mockResolvedValueOnce({
      error: "You do not have permission to view this content.",
    })

    render(<UserSettingsPanel initialShareNotificationEnabled={true} />)

    const toggle = screen.getByRole("switch", { name: /enable share notification/i })
    await user.click(toggle)

    await waitFor(() => {
      expect(
        screen.getByText("You do not have permission to view this content."),
      ).toBeInTheDocument()
      expect(toggle).toBeChecked()
    })
  })
})
