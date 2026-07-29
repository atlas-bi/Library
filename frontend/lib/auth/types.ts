// Auth type definitions — mirrors the /api/auth/me response shape

export const PERMISSIONS = [
  "Edit Role Permissions",
  "Manage Report-Object Relationships",
  "Approve Terms",
  "Edit Report Documentation",
  "Create New Terms",
  "Edit Report-Object Relationships",
  "Edit Terms",
  "Show Advanced Search",
  "Show Report-Object Relationships",
  "Create Collection",
  "Edit Collection",
  "Delete Collection",
  "Create Initiative",
  "View Other User",
] as const

export type Permission = (typeof PERMISSIONS)[number]

export const KNOWN_ROLES = ["Term Administrator", "Report Writer", "Administrator"] as const

export type KnownRole = (typeof KNOWN_ROLES)[number]

export interface AuthUser {
  username: string
  fullname: string
  userId: string
  roles: string[]
  permissions: Permission[]
  adminEnabled: boolean
}
