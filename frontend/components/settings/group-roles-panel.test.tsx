import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { removeGroupRoleAction } from "@/app/settings/actions"
import { GroupRolesPanel } from "./group-roles-panel"

vi.mock("@/app/settings/actions", () => ({
  addGroupRoleAction: vi.fn(),
  removeGroupRoleAction: vi.fn(),
  searchSettingsGroupsAction: vi.fn(),
}))

vi.mock("@/components/settings/settings-typeahead", () => ({
  SettingsTypeahead: () => <div data-testid="group-typeahead" />,
}))

const AVAILABLE_ROLES = [
  { id: 1, name: "Administrator", permissions: [] },
  { id: 10, name: "Manager", permissions: [] },
]

const INITIAL_ASSIGNMENTS = [
  { groupId: 5, name: "Finance Team", roles: [{ id: 10, name: "Manager" }] },
]

describe("GroupRolesPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("renders privileged groups", () => {
    render(
      <GroupRolesPanel initialAssignments={INITIAL_ASSIGNMENTS} availableRoles={AVAILABLE_ROLES} />,
    )
    expect(screen.getByText("Finance Team")).toBeInTheDocument()
    expect(screen.getAllByText("Manager").length).toBeGreaterThan(0)
  })

  it("shows validation error when group and role are missing", async () => {
    const user = userEvent.setup()

    render(<GroupRolesPanel initialAssignments={[]} availableRoles={AVAILABLE_ROLES} />)

    await user.click(screen.getByRole("button", { name: /^save$/i }))

    expect(screen.getByText("Group and role are required.")).toBeInTheDocument()
  })

  it("shows forbidden error when remove fails", async () => {
    const user = userEvent.setup()
    vi.mocked(removeGroupRoleAction).mockResolvedValueOnce({
      error: "You do not have permission to view this content.",
    })

    render(
      <GroupRolesPanel initialAssignments={INITIAL_ASSIGNMENTS} availableRoles={AVAILABLE_ROLES} />,
    )

    await user.click(screen.getByRole("button", { name: /remove role manager from finance team/i }))

    await waitFor(() => {
      expect(
        screen.getByText("You do not have permission to view this content."),
      ).toBeInTheDocument()
    })
  })

  it("removes a group role successfully", async () => {
    const user = userEvent.setup()
    vi.mocked(removeGroupRoleAction).mockResolvedValueOnce({ data: {} })

    render(
      <GroupRolesPanel initialAssignments={INITIAL_ASSIGNMENTS} availableRoles={AVAILABLE_ROLES} />,
    )

    await user.click(screen.getByRole("button", { name: /remove role manager from finance team/i }))

    await waitFor(() => {
      expect(removeGroupRoleAction).toHaveBeenCalledWith(5, 10)
      expect(screen.queryByText("Finance Team")).not.toBeInTheDocument()
    })
  })
})
