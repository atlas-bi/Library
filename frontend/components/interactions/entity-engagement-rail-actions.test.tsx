import { render, screen } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"

const { requestAccessDialogMock } = vi.hoisted(() => ({
  requestAccessDialogMock: vi.fn(
    ({ reportName, reportUrl }: { reportName: string; reportUrl: string }) => (
      <div data-testid="request-access-dialog">
        {reportName} — {reportUrl}
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
import { EntityEngagementRailActions } from "./entity-engagement-rail-actions"

function renderRail(props: React.ComponentProps<typeof EntityEngagementRailActions>) {
  return render(
    <TooltipProvider>
      <EntityEngagementRailActions {...props} />
    </TooltipProvider>,
  )
}

const BASE_PROPS = {
  entityType: "report" as const,
  entityId: 42,
  entityName: "Executive Dashboard",
  entityUrl: "/reports?id=42",
  profileLabel: "report profile",
  profilePanel: <div>Profile</div>,
  isStarred: false,
  starCount: 0,
}

describe("EntityEngagementRailActions request access entry point", () => {
  it("renders RequestAccessDialog on the report action rail when enabled", () => {
    renderRail({
      ...BASE_PROPS,
      showRequestAccess: true,
      features: { requestAccessEnabled: true },
    })

    expect(screen.getByTestId("request-access-dialog")).toBeInTheDocument()
    const dialogProps = requestAccessDialogMock.mock.calls[0]?.[0]
    expect(dialogProps).toEqual(
      expect.objectContaining({
        reportName: "Executive Dashboard",
        reportUrl: "/reports?id=42",
      }),
    )
  })

  it("does not render RequestAccessDialog when showRequestAccess is false", () => {
    renderRail({
      ...BASE_PROPS,
      features: { requestAccessEnabled: true },
    })

    expect(screen.queryByTestId("request-access-dialog")).not.toBeInTheDocument()
  })

  it("does not render RequestAccessDialog when the feature flag is disabled", () => {
    renderRail({
      ...BASE_PROPS,
      showRequestAccess: true,
      features: { requestAccessEnabled: false },
    })

    expect(screen.queryByTestId("request-access-dialog")).not.toBeInTheDocument()
  })
})
