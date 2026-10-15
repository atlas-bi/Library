import { describe, expect, it } from "vitest"
import type { AuthUser } from "@/lib/auth/types"
import {
  getDefaultSettingsTab,
  getSettingsAccess,
  getSettingsNavItems,
  hasAnySettingsAccess,
} from "@/lib/settings/nav"

function user(permissions: AuthUser["permissions"]): AuthUser {
  return {
    username: "tester",
    fullname: "Test User",
    userId: "1",
    roles: [],
    permissions,
    adminEnabled: false,
  }
}

describe("settings nav", () => {
  it("shows only role tabs when the user can edit role permissions", () => {
    const access = getSettingsAccess(user(["Edit Role Permissions"]))

    expect(getSettingsNavItems(access).map((item) => item.id)).toEqual(["roles"])
    expect(hasAnySettingsAccess(access)).toBe(true)
  })

  it("shows site settings tabs only with Manage Global Site Settings", () => {
    const access = getSettingsAccess(user(["Manage Global Site Settings"]))

    expect(getSettingsNavItems(access).map((item) => item.id)).toEqual([
      "site-message",
      "search",
      "theme",
      "etl",
    ])
  })

  it("shows meta fields when the user can create or delete parameters", () => {
    const access = getSettingsAccess(user(["Create Parameters"]))

    expect(getSettingsNavItems(access).map((item) => item.id)).toEqual(["meta-fields"])
  })

  it("returns no nav items and no default tab when the user lacks settings permissions", () => {
    const access = getSettingsAccess(user(["Create New Terms"]))
    const navItems = getSettingsNavItems(access)

    expect(navItems).toEqual([])
    expect(getDefaultSettingsTab(navItems)).toBeNull()
    expect(hasAnySettingsAccess(access)).toBe(false)
  })

  it("defaults to the first visible tab", () => {
    const access = getSettingsAccess(user(["Edit User Permissions", "Edit Group Permissions"]))
    const navItems = getSettingsNavItems(access)

    expect(getDefaultSettingsTab(navItems)).toBe("user-roles")
  })
})
