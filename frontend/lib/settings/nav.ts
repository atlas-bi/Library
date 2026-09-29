import { hasPermission } from "@/lib/auth/auth-service"
import type { AuthUser } from "@/lib/auth/types"

export type SettingsTabId =
  | "roles"
  | "user-roles"
  | "user-groups"
  | "meta-fields"
  | "site-message"
  | "search"
  | "theme"
  | "etl"

export interface SettingsNavItem {
  id: SettingsTabId
  href: string
  label: string
  icon: string
}

export interface SettingsAccess {
  canEditRoles: boolean
  canEditUsers: boolean
  canEditGroups: boolean
  canManageMetaFields: boolean
  canManageSiteSettings: boolean
}

export const META_FIELD_TAG_TYPES = [
  "organizational-values",
  "estimated-run-frequencies",
  "fragilities",
  "fragility-tags",
  "maintenance-schedules",
  "maintenance-log-statuses",
  "financial-impacts",
  "strategic-importances",
  "tags",
] as const

export type MetaFieldTagType = (typeof META_FIELD_TAG_TYPES)[number]

const NAV_DEFINITIONS: Array<SettingsNavItem & { show: (access: SettingsAccess) => boolean }> = [
  {
    id: "roles",
    href: "#roles",
    label: "Role Configuration",
    icon: "fa-lock",
    show: (access) => access.canEditRoles,
  },
  {
    id: "user-roles",
    href: "#user-roles",
    label: "User Roles",
    icon: "fa-user-lock",
    show: (access) => access.canEditUsers,
  },
  {
    id: "user-groups",
    href: "#user-groups",
    label: "Group Roles",
    icon: "fa-users",
    show: (access) => access.canEditGroups,
  },
  {
    id: "meta-fields",
    href: "#meta-fields",
    label: "Meta Fields",
    icon: "fa-list-ul",
    show: (access) => access.canManageMetaFields,
  },
  {
    id: "site-message",
    href: "#site-message",
    label: "Site Message",
    icon: "fa-comment",
    show: (access) => access.canManageSiteSettings,
  },
  {
    id: "search",
    href: "#search",
    label: "Search",
    icon: "fa-search",
    show: (access) => access.canManageSiteSettings,
  },
  {
    id: "theme",
    href: "#theme",
    label: "Theme",
    icon: "fa-palette",
    show: (access) => access.canManageSiteSettings,
  },
  {
    id: "etl",
    href: "#etl",
    label: "ETL",
    icon: "fa-database",
    show: (access) => access.canManageSiteSettings,
  },
]

export function getSettingsAccess(user: AuthUser | null): SettingsAccess {
  return {
    canEditRoles: !!user && hasPermission(user, "Edit Role Permissions"),
    canEditUsers: !!user && hasPermission(user, "Edit User Permissions"),
    canEditGroups: !!user && hasPermission(user, "Edit Group Permissions"),
    canManageMetaFields:
      !!user &&
      (hasPermission(user, "Create Parameters") || hasPermission(user, "Delete Parameters")),
    canManageSiteSettings: !!user && hasPermission(user, "Manage Global Site Settings"),
  }
}

export function hasAnySettingsAccess(access: SettingsAccess): boolean {
  return (
    access.canEditRoles ||
    access.canEditUsers ||
    access.canEditGroups ||
    access.canManageMetaFields ||
    access.canManageSiteSettings
  )
}

export function getSettingsNavItems(access: SettingsAccess): SettingsNavItem[] {
  return NAV_DEFINITIONS.filter((item) => item.show(access)).map(({ show: _show, ...item }) => item)
}

export function getDefaultSettingsTab(navItems: SettingsNavItem[]): SettingsTabId | null {
  return navItems[0]?.id ?? null
}
