import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, expect, it, vi, beforeEach } from "vitest"
import { ShareModal } from "./share-modal"

vi.mock("@/lib/initiatives/actions", () => ({
  shareMailAction: vi.fn(),
}))

import { shareMailAction } from "@/lib/initiatives/actions"

describe("ShareModal", () => {
  const defaultProps = {
    isOpen: true,
    onClose: vi.fn(),
    initiativeName: "Test Initiative",
    initiativeId: 42,
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("rejects non-numeric input with an alert", async () => {
    const user = userEvent.setup()
    const alertMock = vi.spyOn(window, "alert").mockImplementation(() => {})

    render(<ShareModal {...defaultProps} />)

    const toInput = screen.getByPlaceholderText("search for someone..")
    await user.type(toInput, "john@example.com")

    const sendButton = screen.getByRole("button", { name: "Send" })
    await user.click(sendButton)

    // Should alert and NOT call the API
    expect(alertMock).toHaveBeenCalledWith(
      "Please enter a valid numeric user ID. User lookup by name/email is not yet supported."
    )
    expect(shareMailAction).not.toHaveBeenCalled()

    alertMock.mockRestore()
  })

  it("rejects zero and negative input", async () => {
    const user = userEvent.setup()
    const alertMock = vi.spyOn(window, "alert").mockImplementation(() => {})

    render(<ShareModal {...defaultProps} />)

    const toInput = screen.getByPlaceholderText("search for someone..")
    await user.type(toInput, "0")

    const sendButton = screen.getByRole("button", { name: "Send" })
    await user.click(sendButton)

    expect(alertMock).toHaveBeenCalled()
    expect(shareMailAction).not.toHaveBeenCalled()

    alertMock.mockRestore()
  })

  it("sends correct payload with valid numeric userId", async () => {
    const user = userEvent.setup()
    vi.mocked(shareMailAction).mockResolvedValue({ data: null, error: null } as any)

    render(<ShareModal {...defaultProps} />)

    const toInput = screen.getByPlaceholderText("search for someone..")
    await user.type(toInput, "123")

    const sendButton = screen.getByRole("button", { name: "Send" })
    await user.click(sendButton)

    expect(shareMailAction).toHaveBeenCalledWith(
      expect.objectContaining({
        to: [{ type: "u", userId: 123 }],
        text: expect.any(String),
        subject: expect.stringContaining("[Share]"),
        share: true,
      })
    )
  })
})
