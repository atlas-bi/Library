// ---------------------------------------------------------------------------
// Auth service — server‑side helpers for authentication & authorisation
// ---------------------------------------------------------------------------

import { cookies } from "next/headers"
import { getServerApiBase } from "@/lib/api-base"
import { apiFetchJson } from "@/lib/http"
import type { AuthUser, Permission } from "./types"

const API_URL = getServerApiBase()

// ---- Token helpers -------------------------------------------------------

/** Read the JWT stored in cookies (server context only). */
export async function getToken(): Promise<string | null> {
  const cookieStore = await cookies()
  return cookieStore.get("atlas_token")?.value ?? null
}

// ---- User fetching -------------------------------------------------------

/**
 * Fetch the current user from the C# API.
 * Returns `null` when the token is missing or the request fails.
 */
export async function getCurrentUser(): Promise<AuthUser | null> {
  const token = await getToken()
  if (!token) return null

  if (!API_URL) return null
  const result = await apiFetchJson<AuthUser>(`${API_URL}/api/auth/me`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return null
  return result.data
}

// ---- Permission & role checks (server‑side) --------------------------------

/** Check whether the user has a specific permission. */
export function hasPermission(user: AuthUser, permission: Permission): boolean {
  // Full admin has every permission
  if (user.adminEnabled) return true
  return user.permissions.includes(permission)
}

/** Check whether the user has ALL of the given permissions. */
export function hasAllPermissions(user: AuthUser, permissions: Permission[]): boolean {
  return permissions.every((p) => hasPermission(user, p))
}

/** Check whether the user has at least ONE of the given permissions. */
export function hasAnyPermission(user: AuthUser, permissions: Permission[]): boolean {
  return permissions.some((p) => hasPermission(user, p))
}

/** Check whether the user has a specific role. */
export function hasRole(user: AuthUser, role: string): boolean {
  return user.roles.includes(role)
}

/** Check whether the user has any of the given roles. */
export function hasAnyRole(user: AuthUser, roles: string[]): boolean {
  return roles.some((r) => user.roles.includes(r))
}

/** Check whether the user is effectively an administrator. */
export function isAdmin(user: AuthUser): boolean {
  return user.adminEnabled
}
