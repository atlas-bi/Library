import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, expect, it } from "vitest"
import { TasksPageTabs } from "./tasks-page-tabs"
import type { TasksResponseDto } from "@/lib/tasks/types"

const emptyTasks: TasksResponseDto = {
  canMakeReports: [],
  recommendRetire: [{ reportId: 1, name: "Retire me", maintenanceDate: null, maintenanceDateString: "01/01/2024", comment: null, fullName: "Maintainer" }],
  unused: [],
  maintenanceRequired: [],
  audit: [],
  missingSchedule: [],
  notInAnalytics: [],
  topUndocumented: [],
  newUndocumented: [],
}

describe("TasksPageTabs", () => {
  it("shows the default To Retire panel", () => {
    render(<TasksPageTabs data={emptyTasks} canViewGroups={false} />)

    expect(screen.getByRole("link", { name: "Retire me" })).toBeInTheDocument()
  })

  it("switches panels when a tab is clicked", async () => {
    const user = userEvent.setup()
    render(
      <TasksPageTabs
        data={{
          ...emptyTasks,
          canMakeReports: [{ name: "Creator User", userId: 5, role: "Writers", roleId: 9 }],
        }}
        canViewGroups={true}
      />,
    )

    await user.click(screen.getByRole("link", { name: "Users with Create" }))

    expect(screen.getByRole("link", { name: "Creator User" })).toBeInTheDocument()
    expect(screen.queryByRole("link", { name: "Retire me" })).not.toBeInTheDocument()
  })
})
