import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { UserPageTabs } from "./user-page-tabs"

const tabs = {
  starsVisible: true,
  subscriptionsVisible: true,
  groupsVisible: true,
  activityVisible: true,
  runListVisible: true,
  atlasHistoryVisible: true,
  analyticsVisible: true,
}

describe("UserPageTabs", () => {
  it("shows Report Runs content by default for another user's profile", () => {
    render(
      <UserPageTabs
        isCurrentUser={false}
        tabs={tabs}
        stars={<div>Stars content</div>}
        subscriptions={<div>Subscriptions content</div>}
        groups={<div>Groups content</div>}
        activity={<div>Activity content</div>}
        runList={<div>Report Runs content</div>}
        atlasHistory={<div>Atlas History content</div>}
        analytics={<div>Analytics content</div>}
      />,
    )

    expect(screen.getByText("Report Runs content")).toBeInTheDocument()
    expect(screen.queryByText("Stars content")).not.toBeInTheDocument()
  })
})
