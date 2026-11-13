import { fireEvent, render, screen, waitFor } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"
import { AnalyticsErrorsTable } from "./analytics-errors-table"

const setAnalyticsErrorResolvedActionMock = vi.fn()

vi.mock("@/app/analytics/actions", () => ({
  setAnalyticsErrorResolvedAction: (...args: unknown[]) =>
    setAnalyticsErrorResolvedActionMock(...args),
}))

describe("AnalyticsErrorsTable", () => {
  it("surfaces resolve failures without reloading", async () => {
    setAnalyticsErrorResolvedActionMock.mockResolvedValueOnce({
      ok: false,
      error: "server_error",
    })
    const onResolvedChange = vi.fn()

    render(
      <AnalyticsErrorsTable
        errors={{
          pages: 1,
          currentPage: 1,
          totalCount: 1,
          unresolvedCount: 1,
          items: [
            {
              id: 3,
              userId: 1,
              userName: "Admin",
              statusCode: 500,
              message: "boom",
              trace: "stack",
              logDateTime: "2026-01-01",
              handled: null,
              userAgent: "Mozilla/5.0",
              referrer: "http://localhost/reports",
            },
          ],
        }}
        onPageChange={vi.fn()}
        onResolvedChange={onResolvedChange}
      />,
    )

    fireEvent.click(screen.getByRole("checkbox", { name: /Mark error 3 resolved/i }))

    await waitFor(() => {
      expect(
        screen.getByText("We hit a server issue while processing your request. Please try again."),
      ).toBeInTheDocument()
    })
    expect(onResolvedChange).not.toHaveBeenCalled()
  })
})
