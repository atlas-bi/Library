import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"

vi.mock("@/app/interactions/actions", () => ({
  searchRecipientsAction: vi.fn(async () => []),
  submitAccessRequestAction: vi.fn(async () => ({ data: {} })),
}))

import { searchRecipientsAction, submitAccessRequestAction } from "@/app/interactions/actions"
import { TooltipProvider } from "@/components/ui/tooltip"
import { RequestAccessDialog } from "./request-access-dialog"

const PROPS = {
  reportName: "Sales Dashboard",
  reportUrl: "https://example.com/reports/1",
}

function renderDialog() {
  return render(
    <TooltipProvider>
      <RequestAccessDialog {...PROPS} iconOnly={false} />
    </TooltipProvider>,
  )
}

async function openDialog() {
  await userEvent.click(screen.getByRole("button", { name: /request access/i }))
  await screen.findByRole("dialog")
}

async function pickDirector(name: string) {
  vi.mocked(searchRecipientsAction).mockResolvedValueOnce([
    { id: 1, name, type: "user", email: "director@example.com" },
  ])
  const input = screen.getByLabelText(/find your director/i)
  await userEvent.type(input, name.slice(0, 3))
  vi.runAllTimers()
  await screen.findByText(name)
  await userEvent.click(screen.getByText(name))
}

describe("RequestAccessDialog", () => {
  beforeEach(() => {
    vi.useFakeTimers({ shouldAdvanceTime: true })
    vi.mocked(submitAccessRequestAction).mockResolvedValue({ data: {} })
    vi.mocked(searchRecipientsAction).mockResolvedValue([])
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it("renders the trigger button and opens the dialog", async () => {
    renderDialog()

    await openDialog()

    expect(screen.getByRole("heading", { name: /request report access/i })).toBeInTheDocument()
  })

  it("displays the report name inside the dialog", async () => {
    renderDialog()

    await openDialog()

    expect(screen.getByText("Sales Dashboard")).toBeInTheDocument()
  })

  it("shows a validation error when submitting without selecting a director", async () => {
    renderDialog()

    await openDialog()
    await userEvent.click(screen.getByRole("button", { name: /^request access$/i }))

    expect(screen.getByText("Director is required.")).toBeInTheDocument()
  })

  it("retains the selected director name after picking from suggestions", async () => {
    renderDialog()

    await openDialog()
    await pickDirector("Jane Smith")

    expect(screen.getByText(/selected director/i)).toBeInTheDocument()
    expect(screen.getAllByText("Jane Smith").length).toBeGreaterThan(0)
  })

  it("shows a success message after a valid submission", async () => {
    renderDialog()

    await openDialog()
    await pickDirector("Jane Smith")
    await userEvent.click(screen.getByRole("button", { name: /^request access$/i }))

    await waitFor(() => {
      expect(screen.getByText(/your request has been submitted/i)).toBeInTheDocument()
    })
  })

  it("displays the API error message on a 400 validation failure", async () => {
    vi.mocked(submitAccessRequestAction).mockResolvedValueOnce({
      error: "Report name is required.",
    })

    renderDialog()

    await openDialog()
    await pickDirector("Jane Smith")
    await userEvent.click(screen.getByRole("button", { name: /^request access$/i }))

    await waitFor(() => {
      expect(screen.getByText("Report name is required.")).toBeInTheDocument()
    })
  })

  it("displays the API error message on a service failure", async () => {
    vi.mocked(submitAccessRequestAction).mockResolvedValueOnce({
      error: "The service is temporarily unavailable. Please try again shortly.",
    })

    renderDialog()

    await openDialog()
    await pickDirector("Jane Smith")
    await userEvent.click(screen.getByRole("button", { name: /^request access$/i }))

    await waitFor(() => {
      expect(
        screen.getByText("The service is temporarily unavailable. Please try again shortly."),
      ).toBeInTheDocument()
    })
  })
})
