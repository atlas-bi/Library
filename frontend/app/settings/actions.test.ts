import { beforeEach, describe, expect, test, vi } from "vitest"

vi.mock("next/cache", () => ({
  revalidatePath: vi.fn(),
}))

vi.mock("@/lib/groups/api", () => ({
  searchGroups: vi.fn(),
}))

vi.mock("@/lib/settings/api", () => ({
  addSiteMessage: vi.fn(),
  deleteSiteMessage: vi.fn(),
  updateEtl: vi.fn(),
  updateTheme: vi.fn(),
  updateSearchVisibility: vi.fn(),
  createRole: vi.fn(),
  updateRolePermission: vi.fn(),
}))

import { revalidatePath } from "next/cache"
import { searchGroups } from "@/lib/groups/api"
import {
  addSiteMessage,
  createRole,
  updateRolePermission,
  updateSearchVisibility,
} from "@/lib/settings/api"
import {
  addSiteMessageAction,
  createRoleAction,
  searchSettingsGroupsAction,
  updateRolePermissionAction,
  updateSearchVisibilityAction,
} from "./actions"

describe("settings actions", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  test("addSiteMessageAction returns data and revalidates on success", async () => {
    vi.mocked(addSiteMessage).mockResolvedValueOnce({
      ok: true,
      data: { id: 1, value: "Hello", description: null },
    })

    const result = await addSiteMessageAction({ value: "Hello" })

    expect(result).toEqual({ data: { id: 1, value: "Hello", description: null } })
    expect(revalidatePath).toHaveBeenCalledWith("/settings")
  })

  test("addSiteMessageAction returns error without revalidating on failure", async () => {
    vi.mocked(addSiteMessage).mockResolvedValueOnce({
      ok: false,
      message: "You do not have permission to view this content.",
      code: "forbidden",
    })

    const result = await addSiteMessageAction({ value: "Hello" })

    expect(result).toEqual({ error: "You do not have permission to view this content." })
    expect(revalidatePath).not.toHaveBeenCalled()
  })

  test("updateSearchVisibilityAction delegates to api layer", async () => {
    vi.mocked(updateSearchVisibility).mockResolvedValueOnce({ ok: true, data: undefined })

    const result = await updateSearchVisibilityAction("users", false)

    expect(updateSearchVisibility).toHaveBeenCalledWith("users", false, undefined)
    expect(result).toEqual({ data: {} })
    expect(revalidatePath).toHaveBeenCalledWith("/settings")
  })

  test("createRoleAction returns validation error from api", async () => {
    vi.mocked(createRole).mockResolvedValueOnce({
      ok: false,
      message: "Name is required.",
      code: "bad_request",
    })

    const result = await createRoleAction({ name: "" })

    expect(result).toEqual({ error: "Name is required." })
  })

  test("updateRolePermissionAction returns forbidden error", async () => {
    vi.mocked(updateRolePermission).mockResolvedValueOnce({
      ok: false,
      message: "You do not have permission to view this content.",
      code: "forbidden",
    })

    const result = await updateRolePermissionAction(2, 4, true)

    expect(result).toEqual({ error: "You do not have permission to view this content." })
    expect(revalidatePath).not.toHaveBeenCalled()
  })

  test("searchSettingsGroupsAction returns empty array for blank query", async () => {
    const result = await searchSettingsGroupsAction("  ")

    expect(result).toEqual([])
    expect(searchGroups).not.toHaveBeenCalled()
  })

  test("searchSettingsGroupsAction maps group search results for the typeahead", async () => {
    vi.mocked(searchGroups).mockResolvedValueOnce([
      { id: 2, name: "Clinical Operations", email: "clinical-ops@example.com" },
    ])

    const result = await searchSettingsGroupsAction("clinical")

    expect(searchGroups).toHaveBeenCalledWith("clinical")
    expect(result).toEqual([
      {
        id: 2,
        name: "Clinical Operations",
        description: "clinical-ops@example.com",
      },
    ])
  })
})
