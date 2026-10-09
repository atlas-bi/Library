import { fireEvent, render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { AnalyticsTracesTable } from "./analytics-traces-table"

const setAnalyticsTraceResolvedActionMock = vi.fn()

vi.mock("@/app/analytics/actions", () => ({
  setAnalyticsTraceResolvedAction: (...args: unknown[]) =>
    setAnalyticsTraceResolvedActionMock(...args),
}))

const traceItem = {
  id: 9,
  userId: 1,
  userName: "Admin",
  level: 6000,
  message: "error",
  logger: "onerrorLogger",
  logDateTime: "2026-01-01",
  handled: null as number | null,
  userAgent: "Mozilla/5.0",
  referer: "http://localhost/",
}

describe("AnalyticsTracesTable", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("does not refresh when resolve fails", async () => {
    setAnalyticsTraceResolvedActionMock.mockResolvedValueOnce({
      ok: false,
      error: "service_unavailable",
    })
    const onResolvedChange = vi.fn()

    render(
      <AnalyticsTracesTable
        traces={{
          pages: 1,
          currentPage: 1,
          totalCount: 1,
          unresolvedCount: 1,
          items: [traceItem],
        }}
        onPageChange={vi.fn()}
        onResolvedChange={onResolvedChange}
      />,
    )

    fireEvent.click(screen.getByRole("checkbox", { name: /Mark trace 9 resolved/i }))

    await waitFor(() => {
      expect(
        screen.getByText("The service is temporarily unavailable. Please try again shortly."),
      ).toBeInTheDocument()
    })
    expect(onResolvedChange).not.toHaveBeenCalled()
  })

  it("reloads when resolve succeeds", async () => {
    setAnalyticsTraceResolvedActionMock.mockResolvedValueOnce({ ok: true, error: null })
    const onResolvedChange = vi.fn()

    render(
      <AnalyticsTracesTable
        traces={{
          pages: 1,
          currentPage: 1,
          totalCount: 1,
          unresolvedCount: 1,
          items: [traceItem],
        }}
        onPageChange={vi.fn()}
        onResolvedChange={onResolvedChange}
      />,
    )

    fireEvent.click(screen.getByRole("checkbox", { name: /Mark trace 9 resolved/i }))

    await waitFor(() => {
      expect(onResolvedChange).toHaveBeenCalledTimes(1)
    })
  })

  it("requests another trace page from pagination", async () => {
    const user = userEvent.setup()
    const onPageChange = vi.fn()

    render(
      <AnalyticsTracesTable
        traces={{
          pages: 3,
          currentPage: 1,
          totalCount: 25,
          unresolvedCount: 3,
          items: [traceItem],
        }}
        onPageChange={onPageChange}
        onResolvedChange={vi.fn()}
      />,
    )

    await user.click(screen.getByRole("button", { name: "2" }))

    expect(onPageChange).toHaveBeenCalledWith(1)
  })
})
