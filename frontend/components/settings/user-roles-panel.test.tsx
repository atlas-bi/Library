import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { removeUserRoleAction } from "@/app/settings/actions"
import { UserRolesPanel } from "./user-roles-panel"

vi.mock("@/app/settings/actions", () => ({
  addUserRoleAction: vi.fn(),
  removeUserRoleAction: vi.fn(),
  searchSettingsUsersAction: vi.fn(),
}))

vi.mock("@/components/settings/settings-typeahead", () => ({
  SettingsTypeahead: () => <div data-testid="user-typeahead" />,
}))

const AVAILABLE_ROLES = [
  { id: 1, name: "Administrator", permissions: [] },
  { id: 10, name: "Manager", permissions: [] },
]

const INITIAL_ASSIGNMENTS = [{ userId: 42, name: "Jane Doe", roles: [{ id: 10, name: "Manager" }] }]

describe("UserRolesPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("renders privileged users", () => {
    render(
      <UserRolesPanel initialAssignments={INITIAL_ASSIGNMENTS} availableRoles={AVAILABLE_ROLES} />,
    )
    expect(screen.getByText("Jane Doe")).toBeInTheDocument()
    expect(screen.getAllByText("Manager").length).toBeGreaterThan(0)
  })

  it("shows validation error when user and role are missing", async () => {
    const user = userEvent.setup()

    render(<UserRolesPanel initialAssignments={[]} availableRoles={AVAILABLE_ROLES} />)

    await user.click(screen.getByRole("button", { name: /^save$/i }))

    expect(screen.getByText("User and role are required.")).toBeInTheDocument()
  })

  it("shows forbidden error when remove fails", async () => {
    const user = userEvent.setup()
    vi.mocked(removeUserRoleAction).mockResolvedValueOnce({
      error: "You do not have permission to view this content.",
    })

    render(
      <UserRolesPanel initialAssignments={INITIAL_ASSIGNMENTS} availableRoles={AVAILABLE_ROLES} />,
    )

    await user.click(screen.getByRole("button", { name: /remove role manager from jane doe/i }))

    await waitFor(() => {
      expect(
        screen.getByText("You do not have permission to view this content."),
      ).toBeInTheDocument()
    })
  })

  it("removes a user role successfully", async () => {
    const user = userEvent.setup()
    vi.mocked(removeUserRoleAction).mockResolvedValueOnce({ data: {} })

    render(
      <UserRolesPanel initialAssignments={INITIAL_ASSIGNMENTS} availableRoles={AVAILABLE_ROLES} />,
    )

    await user.click(screen.getByRole("button", { name: /remove role manager from jane doe/i }))

    await waitFor(() => {
      expect(removeUserRoleAction).toHaveBeenCalledWith(42, 10)
      expect(screen.queryByText("Jane Doe")).not.toBeInTheDocument()
    })
  })
})
