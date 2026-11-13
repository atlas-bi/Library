import { render, screen } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"

const { requestAccessDialogMock } = vi.hoisted(() => ({
  requestAccessDialogMock: vi.fn(
    ({
      reportName,
      reportUrl,
      variant,
    }: {
      reportName: string
      reportUrl: string
      variant?: string
    }) => (
      <div data-testid="request-access-dialog">
        {reportName} — {reportUrl} — {variant ?? "default"}
      </div>
    ),
  ),
}))

vi.mock("@/components/interactions/request-access-dialog", () => ({
  RequestAccessDialog: requestAccessDialogMock,
}))

vi.mock("@/app/interactions/actions", () => ({
  toggleStarAction: vi.fn(async () => ({ data: { isStarred: false, count: 0 } })),
}))

import { TooltipProvider } from "@/components/ui/tooltip"
import { EntityCardFooter } from "./entity-card-footer"

function renderFooter(props: React.ComponentProps<typeof EntityCardFooter>) {
  return render(
    <TooltipProvider>
      <EntityCardFooter {...props} />
    </TooltipProvider>,
  )
}

describe("EntityCardFooter request access entry point", () => {
  it("renders RequestAccessDialog on report cards in collection listings", () => {
    renderFooter({
      entityType: "report",
      id: 7,
      title: "Monthly KPI Pack",
      href: "/reports?id=7",
      canRequestAccess: true,
      features: { requestAccessEnabled: true },
      profilePanel: <div>Profile</div>,
    })

    expect(screen.getByTestId("request-access-dialog")).toBeInTheDocument()
    const dialogProps = requestAccessDialogMock.mock.calls[0]?.[0]
    expect(dialogProps).toEqual(
      expect.objectContaining({
        reportName: "Monthly KPI Pack",
        reportUrl: "/reports?id=7",
        variant: "footer",
      }),
    )
  })

  it("does not render RequestAccessDialog when canRequestAccess is false", () => {
    renderFooter({
      entityType: "report",
      id: 7,
      title: "Monthly KPI Pack",
      href: "/reports?id=7",
      canRequestAccess: false,
      features: { requestAccessEnabled: true },
      profilePanel: <div>Profile</div>,
    })

    expect(screen.queryByTestId("request-access-dialog")).not.toBeInTheDocument()
  })

  it("does not render RequestAccessDialog for collection cards", () => {
    renderFooter({
      entityType: "collection",
      id: 3,
      title: "Finance Collection",
      href: "/collections?id=3",
      profilePanel: <div>Profile</div>,
    })

    expect(screen.queryByTestId("request-access-dialog")).not.toBeInTheDocument()
  })
})
