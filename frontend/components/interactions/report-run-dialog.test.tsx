import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import type { ReactNode } from "react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { ReportRunDialog } from "@/components/interactions/report-run-dialog"
import { TooltipProvider } from "@/components/ui/tooltip"

function renderDialog(ui: ReactNode) {
  return render(<TooltipProvider>{ui}</TooltipProvider>)
}

vi.mock("@/app/reports/file-actions", () => ({
  downloadReportAttachmentAction: vi.fn(),
}))

import { downloadReportAttachmentAction } from "@/app/reports/file-actions"
import { saveDownloadedFile } from "@/lib/reports/save-downloaded-file"

vi.mock("@/lib/reports/save-downloaded-file", () => ({
  saveDownloadedFile: vi.fn(),
}))

describe("ReportRunDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("downloads crystal attachments through the reports API", async () => {
    vi.mocked(downloadReportAttachmentAction).mockResolvedValueOnce({
      ok: true,
      data: {
        base64: btoa("pdf"),
        fileName: "Run Output.pdf",
        contentType: "application/pdf",
      },
    })

    renderDialog(
      <ReportRunDialog
        reportName="Crystal Report"
        attachments={[{ id: 5, name: "Run Output.pdf", creationDate: "2026-01-01T00:00:00Z" }]}
      />,
    )

    await userEvent.click(screen.getByRole("button", { name: "Run report" }))
    await userEvent.click(screen.getByRole("button", { name: "Run Output.pdf" }))

    await waitFor(() => {
      expect(downloadReportAttachmentAction).toHaveBeenCalledWith(5, "Run Output.pdf")
      expect(saveDownloadedFile).toHaveBeenCalled()
    })
  })

  it("shows a forbidden message when the attachment download fails", async () => {
    vi.mocked(downloadReportAttachmentAction).mockResolvedValueOnce({
      ok: false,
      code: "forbidden",
      status: 403,
    })

    renderDialog(
      <ReportRunDialog
        reportName="Crystal Report"
        attachments={[{ id: 6, name: "Locked Output.pdf" }]}
      />,
    )

    await userEvent.click(screen.getByRole("button", { name: "Run report" }))
    await userEvent.click(screen.getByRole("button", { name: "Locked Output.pdf" }))

    expect(
      await screen.findByText("You do not have permission to open this run output."),
    ).toBeInTheDocument()
  })

  it("shows a not found message when the attachment is missing", async () => {
    vi.mocked(downloadReportAttachmentAction).mockResolvedValueOnce({
      ok: false,
      code: "not_found",
      status: 404,
    })

    renderDialog(
      <ReportRunDialog
        reportName="Crystal Report"
        attachments={[{ id: 7, name: "Missing Output.pdf" }]}
      />,
    )

    await userEvent.click(screen.getByRole("button", { name: "Run report" }))
    await userEvent.click(screen.getByRole("button", { name: "Missing Output.pdf" }))

    expect(
      await screen.findByText("That run output is no longer available."),
    ).toBeInTheDocument()
  })
})
