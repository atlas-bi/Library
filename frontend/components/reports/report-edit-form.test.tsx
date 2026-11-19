import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { ReportEditForm } from "@/components/reports/report-edit-form"
import type { ReportDetail, ReportUpdateBody } from "@/lib/reports/types"

const { updateReportActionMock } = vi.hoisted(() => ({
  updateReportActionMock: vi.fn(async (_id: number, _body: ReportUpdateBody) => undefined),
}))

vi.mock("@/app/reports/actions", () => ({
  searchReportTermsAction: vi.fn(async () => []),
  searchReportCollectionsAction: vi.fn(async () => []),
  searchReportUsersAction: vi.fn(async () => []),
  updateReportAction: updateReportActionMock,
}))

const report: ReportDetail = {
  id: 16,
  name: "daily_emergency_department_census",
  canEditDocumentation: true,
  document: {
    serviceRequests: [
      { id: 7, ticketNumber: "INC007", description: "Add filter" },
      { id: 8, ticketNumber: "INC008" },
    ],
  },
}

function renderForm(mode: "maintenance" | "description" = "maintenance") {
  render(<ReportEditForm reportId={16} initial={report} cancelHref="/reports?id=16" mode={mode} />)
}

function lastBody(): ReportUpdateBody {
  const call = updateReportActionMock.mock.calls.at(-1)
  if (!call) throw new Error("updateReportAction was not called")
  return call[1]
}

describe("ReportEditForm service requests", () => {
  beforeEach(() => {
    updateReportActionMock.mockClear()
  })

  it("keeps existing service requests when saving another step", async () => {
    const user = userEvent.setup()
    renderForm("description")

    await user.click(screen.getByRole("button", { name: "Save changes" }))

    expect(lastBody().serviceRequestIds).toEqual([7, 8])
    expect(lastBody().newServiceRequest).toBeNull()
  })

  it("drops a removed service request", async () => {
    const user = userEvent.setup()
    renderForm()

    await user.click(screen.getByRole("button", { name: "Remove service request INC007" }))
    await user.click(screen.getByRole("button", { name: "Save changes" }))

    expect(lastBody().serviceRequestIds).toEqual([8])
  })

  it("sends a new service request", async () => {
    const user = userEvent.setup()
    renderForm()

    await user.type(screen.getByLabelText("New service request ticket number"), " INC009 ")
    await user.type(screen.getByLabelText("New service request link"), "https://example.com/inc009")
    await user.type(screen.getByLabelText("New service request description"), "New export")
    await user.click(screen.getByRole("button", { name: "Save changes" }))

    expect(lastBody().serviceRequestIds).toEqual([7, 8])
    expect(lastBody().newServiceRequest).toEqual({
      ticketNumber: "INC009",
      description: "New export",
      ticketUrl: "https://example.com/inc009",
    })
  })
})
