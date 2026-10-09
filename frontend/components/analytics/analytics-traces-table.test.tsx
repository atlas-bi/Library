import { fireEvent, render, screen, waitFor } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"
import { AnalyticsTracesTable } from "./analytics-traces-table"

const setAnalyticsTraceResolvedActionMock = vi.fn()

vi.mock("@/app/analytics/actions", () => ({
  setAnalyticsTraceResolvedAction: (...args: unknown[]) =>
    setAnalyticsTraceResolvedActionMock(...args),
}))

describe("AnalyticsTracesTable", () => {
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
          items: [
            {
              id: 9,
              userId: 1,
              userName: "Admin",
              level: 6000,
              message: "error",
              logger: "onerrorLogger",
              logDateTime: "2026-01-01",
              handled: null,
              userAgent: "Mozilla/5.0",
              referer: "http://localhost/",
            },
          ],
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
})
