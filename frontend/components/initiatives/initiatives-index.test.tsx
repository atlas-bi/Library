import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, expect, it, vi, beforeEach } from "vitest"
import type { ReactNode } from "react"
import { TooltipProvider } from "@/components/ui/tooltip"
import { InitiativesIndex } from "./initiatives-index"
import { toggleStarAction } from "@/lib/initiatives/actions"

vi.mock("@/lib/initiatives/actions", () => ({
  toggleStarAction: vi.fn(),
}))

const renderIndex = (ui: ReactNode) => render(<TooltipProvider>{ui}</TooltipProvider>)

describe("InitiativesIndex", () => {
  const emptyData = {
    items: [],
    total: 0,
    page: 1,
    pageSize: 20,
  }

  it("renders the Create Initiative button when canCreateInitiative is true", () => {
    renderIndex(<InitiativesIndex data={emptyData} canCreateInitiative={true} />)

    expect(screen.getByRole("button", { name: "+ Create an Initiative" })).toBeDefined()
  })

  it("hides the Create Initiative button when canCreateInitiative is false", () => {
    renderIndex(<InitiativesIndex data={emptyData} canCreateInitiative={false} />)

    expect(screen.queryByRole("button", { name: "+ Create an Initiative" })).toBeNull()
  })

  it("renders feedback only when the API feature is enabled", () => {
    renderIndex(
      <InitiativesIndex
        data={{
          items: [{ id: 1, name: "Initiative" }],
          total: 1,
          page: 1,
          pageSize: 20,
          features: { feedbackEnabled: true },
        }}
        canCreateInitiative={false}
      />,
    )

    expect(screen.getByRole("button", { name: "Share feedback" })).toBeDefined()
  })

  it("hides feedback when the API disables it", () => {
    renderIndex(
      <InitiativesIndex
        data={{
          items: [{ id: 1, name: "Initiative" }],
          total: 1,
          page: 1,
          pageSize: 20,
          features: { feedbackEnabled: false },
        }}
        canCreateInitiative={false}
      />,
    )

    expect(screen.queryByRole("button", { name: "Share feedback" })).toBeNull()
  })
})

describe("InitiativesIndex - Star Toggle", () => {
  const mockData = {
    items: [
      { id: 1, name: "Initiative A", isStarred: false, starCount: 3 },
    ],
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("applies server values on successful toggle", async () => {
    const user = userEvent.setup()
    vi.mocked(toggleStarAction).mockResolvedValue({
      data: { type: "Initiative", id: 1, isStarred: true, count: 4 },
      error: null,
    })

    renderIndex(<InitiativesIndex data={mockData as any} canCreateInitiative={false} />)

    const starButton = screen.getByRole("button", { name: /star/i })
    await user.click(starButton)

    // Server says count is 4
    expect(await screen.findByText("4")).toBeDefined()
  })

  it("reverts UI on failed toggle", async () => {
    const user = userEvent.setup()
    const alertMock = vi.spyOn(window, "alert").mockImplementation(() => {})
    vi.mocked(toggleStarAction).mockResolvedValue({
      data: null,
      error: "service_unavailable",
    })

    renderIndex(<InitiativesIndex data={mockData as any} canCreateInitiative={false} />)

    expect(screen.getByText("3")).toBeDefined()

    const starButton = screen.getByRole("button", { name: /star/i })
    await user.click(starButton)

    // Should revert to 3
    expect(await screen.findByText("3")).toBeDefined()
    expect(alertMock).toHaveBeenCalled()

    alertMock.mockRestore()
  })
})
