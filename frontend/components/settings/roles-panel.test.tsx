import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"
import { createRoleAction, deleteRoleAction, updateRolePermissionAction } from "@/app/settings/actions"
import { RolesPanel } from "./roles-panel"
import type { PermissionDto, RoleDto } from "@/lib/settings/types"

vi.mock("@/app/settings/actions", () => ({
  createRoleAction: vi.fn(),
  deleteRoleAction: vi.fn(),
  updateRolePermissionAction: vi.fn(),
}))

const PERMISSIONS: PermissionDto[] = [
  { id: 4, name: "Edit Role Permissions" },
  { id: 5, name: "Manage Global Site Settings" },
]

const INITIAL_ROLES: RoleDto[] = [
  { id: 1, name: "Administrator", permissions: PERMISSIONS },
  { id: 2, name: "User", permissions: [] },
  { id: 10, name: "Manager", permissions: [{ id: 4, name: "Edit Role Permissions" }] },
]

describe("RolesPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.spyOn(window, "confirm").mockReturnValue(true)
  })

  it("renders roles as columns and permissions as rows", () => {
    render(<RolesPanel initialRoles={INITIAL_ROLES} permissions={PERMISSIONS} />)
    // Roles as column headers
    expect(screen.getByText("Administrator")).toBeInTheDocument()
    expect(screen.getByText("Manager")).toBeInTheDocument()
    // Permissions as row headers
    expect(screen.getAllByText("Edit Role Permissions")[0]).toBeInTheDocument()

    // Manager should have perm 4 checked
    const checkbox = screen.getByLabelText(/edit role permissions for manager/i) as HTMLInputElement
    expect(checkbox).toBeInTheDocument()
    expect(checkbox.checked).toBe(true)
  })

  it("toggles permission optimistically and calls the server action", async () => {
    const user = userEvent.setup()
    vi.mocked(updateRolePermissionAction).mockResolvedValueOnce({ data: {} })

    render(<RolesPanel initialRoles={INITIAL_ROLES} permissions={PERMISSIONS} />)

    // Toggle on for User (id=2, perm id=4)
    const checkbox = screen.getByLabelText(/edit role permissions for user/i)
    await user.click(checkbox)

    await waitFor(() => {
      expect(updateRolePermissionAction).toHaveBeenCalledWith(2, 4, true)
    })
  })

  it("reverts permission on server failure", async () => {
    const user = userEvent.setup()
    vi.mocked(updateRolePermissionAction).mockResolvedValueOnce({ error: "Access denied" })

    render(<RolesPanel initialRoles={INITIAL_ROLES} permissions={PERMISSIONS} />)

    const checkbox = screen.getByLabelText(/edit role permissions for manager/i) as HTMLInputElement
    await user.click(checkbox)

    await waitFor(() => {
      expect(screen.getByText("Access denied")).toBeInTheDocument()
      expect(checkbox.checked).toBe(true) // reverted
    })
  })

  it("deletes a non-protected role via link click", async () => {
    const user = userEvent.setup()
    vi.mocked(deleteRoleAction).mockResolvedValueOnce({ data: {} })

    render(<RolesPanel initialRoles={INITIAL_ROLES} permissions={PERMISSIONS} />)

    const deleteLink = screen.getByRole("link", { name: /delete role manager/i })
    await user.click(deleteLink)

    await waitFor(() => {
      expect(deleteRoleAction).toHaveBeenCalledWith(10)
    })
  })
})
