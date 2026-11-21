import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { ReportEditImagesSection } from "@/components/reports/report-edit-images-section"
import type { ReportDetail, ReportUpdateBody } from "@/lib/reports/types"

const { updateReportActionMock } = vi.hoisted(() => ({
  updateReportActionMock: vi.fn(async () => undefined),
}))

vi.mock("@/app/reports/actions", () => ({
  updateReportAction: updateReportActionMock,
}))

vi.mock("next/navigation", () => ({
  useRouter: () => ({ refresh: vi.fn() }),
}))

const report: ReportDetail = {
  id: 16,
  name: "demo",
  images: [
    { id: 10, ordinal: 0 },
    { id: 20, ordinal: 1 },
  ],
  document: {},
}

function lastBody(): ReportUpdateBody {
  const call = updateReportActionMock.mock.calls.at(-1)
  if (!call) throw new Error("updateReportAction was not called")
  return call[1]
}

describe("ReportEditImagesSection", () => {
  beforeEach(() => {
    updateReportActionMock.mockClear()
  })

  it("deletes an image by omitting its id from imageIds", async () => {
    const user = userEvent.setup()
    render(<ReportEditImagesSection report={report} onBack={vi.fn()} onContinue={vi.fn()} />)

    await user.click(screen.getByRole("button", { name: "Delete image 10" }))

    expect(lastBody().imageIds).toEqual([20])
  })

  it("reorders images when move down is clicked", async () => {
    const user = userEvent.setup()
    render(<ReportEditImagesSection report={report} onBack={vi.fn()} onContinue={vi.fn()} />)

    await user.click(screen.getByRole("button", { name: "Move image 10 down" }))

    expect(lastBody().imageIds).toEqual([20, 10])
  })
})