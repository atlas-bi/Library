import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { addSiteMessageAction, deleteSiteMessageAction } from "@/app/settings/actions"
import { SiteMessagesPanel } from "./site-messages-panel"

vi.mock("@/app/settings/actions", () => ({
  addSiteMessageAction: vi.fn(),
  deleteSiteMessageAction: vi.fn(),
}))

const INITIAL_MESSAGES = [{ id: 1, value: "Welcome to Atlas", description: "Default banner" }]

describe("SiteMessagesPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("renders existing messages", () => {
    render(<SiteMessagesPanel initialMessages={INITIAL_MESSAGES} />)
    expect(screen.getByText("Welcome to Atlas")).toBeInTheDocument()
    expect(screen.getByText("Default banner")).toBeInTheDocument()
  })

  it("shows validation error if adding an empty message", async () => {
    const user = userEvent.setup()
    render(<SiteMessagesPanel initialMessages={INITIAL_MESSAGES} />)

    await user.type(screen.getByPlaceholderText(/message content/i), "   ")
    await user.click(screen.getByRole("button", { name: /^add$/i }))
    expect(screen.getByText("Message content is required.")).toBeInTheDocument()
    expect(addSiteMessageAction).not.toHaveBeenCalled()
  })

  it("adds a message successfully and updates the list", async () => {
    const user = userEvent.setup()
    vi.mocked(addSiteMessageAction).mockResolvedValueOnce({
      data: { id: 2, value: "New Message", description: null },
    })

    render(<SiteMessagesPanel initialMessages={INITIAL_MESSAGES} />)

    await user.type(screen.getByPlaceholderText(/message content/i), "New Message")
    await user.click(screen.getByRole("button", { name: /^add$/i }))

    await waitFor(() => {
      expect(addSiteMessageAction).toHaveBeenCalledWith({
        value: "New Message",
        description: undefined,
      })
      expect(screen.getByText("New Message")).toBeInTheDocument()
    })
  })

  it("displays server error when adding fails (e.g. forbidden)", async () => {
    const user = userEvent.setup()
    vi.mocked(addSiteMessageAction).mockResolvedValueOnce({
      error: "You do not have permission to view this content.",
    })

    render(<SiteMessagesPanel initialMessages={INITIAL_MESSAGES} />)

    await user.type(screen.getByPlaceholderText(/message content/i), "Another Message")
    await user.click(screen.getByRole("button", { name: /^add$/i }))

    await waitFor(() => {
      expect(
        screen.getByText("You do not have permission to view this content."),
      ).toBeInTheDocument()
    })
  })

  it("deletes a message successfully", async () => {
    const user = userEvent.setup()
    vi.mocked(deleteSiteMessageAction).mockResolvedValueOnce({ data: {} })

    // Mock window.confirm
    const confirmSpy = vi.spyOn(window, "confirm").mockImplementation(() => true)

    render(<SiteMessagesPanel initialMessages={INITIAL_MESSAGES} />)

    const deleteBtn = screen.getByRole("button", { name: /delete message 1/i })
    await user.click(deleteBtn)

    await waitFor(() => {
      expect(deleteSiteMessageAction).toHaveBeenCalledWith(1)
      expect(screen.queryByText("Welcome to Atlas")).not.toBeInTheDocument()
    })

    confirmSpy.mockRestore()
  })
})
