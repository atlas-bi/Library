import { fireEvent, render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, expect, it, vi } from "vitest"
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

  it("renders Audit Only panel copy when that tab is selected", () => {
    vi.useFakeTimers()
    vi.setSystemTime(new Date("2026-09-30T12:00:00Z"))

    render(
      <TasksPageTabs
        data={{
          ...emptyTasks,
          recommendRetire: [],
          audit: [
            {
              reportId: 10,
              name: "Audit Report",
              user: "Alex Maintainer",
              date: "01/15/2025",
            },
          ],
        }}
        canViewGroups={false}
      />,
    )

    fireEvent.click(screen.getByRole("link", { name: "Audit Only" }))

    expect(screen.getByText("Past Due")).toBeInTheDocument()
    expect(
      screen.getByText(/Last updated\/maintained by Alex Maintainer on 01\/15\/2025/),
    ).toBeInTheDocument()
    expect(screen.queryByText(/Due on/i)).not.toBeInTheDocument()

    vi.useRealTimers()
  })

  it("renders No Maint Schedule panel copy when that tab is selected", () => {
    render(
      <TasksPageTabs
        data={{
          ...emptyTasks,
          recommendRetire: [],
          missingSchedule: [
            {
              reportId: 12,
              name: "Missing Schedule Report",
              user: "Alex Maintainer",
              date: "03/03/2024",
            },
          ],
        }}
        canViewGroups={false}
      />,
    )

    fireEvent.click(screen.getByRole("link", { name: "No Maint Schedule" }))

    expect(
      screen.getByText(/Last updated\/maintained by Alex Maintainer on 03\/03\/2024/),
    ).toBeInTheDocument()
    expect(screen.queryByText("Past Due")).not.toBeInTheDocument()
    expect(screen.queryByText(/Due on/i)).not.toBeInTheDocument()
  })

  it("switches to Users with Create when that tab is clicked", async () => {
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
