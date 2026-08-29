// Settings API client — consumes /api/settings/*
// All functions run server-side (use `getServerApiBase` + `getToken`).
// Mutation helpers return a discriminated union so callers can distinguish
// success / forbidden / validation error / network error without try/catch.

import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import { getUserFriendlyErrorMessage, mapHttpStatusToErrorCode } from "@/lib/errors"
import type { AppErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type {
  GroupRoleAssignmentDto,
  ParameterDto,
  ParameterRequest,
  PermissionDto,
  RoleDto,
  RoleRequest,
  SearchSettingsDto,
  SettingValueDto,
  SiteMessageDto,
  SiteMessageRequest,
  UserRoleAssignmentDto,
} from "./types"

// ---------------------------------------------------------------------------
// Shared result types
// ---------------------------------------------------------------------------

export type SettingsOk<T> = { ok: true; data: T }
export type SettingsErr = { ok: false; message: string; code: AppErrorCode }
export type SettingsResult<T> = SettingsOk<T> | SettingsErr

// ---------------------------------------------------------------------------
// Internal helpers
// ---------------------------------------------------------------------------

async function getAuthHeaders(): Promise<Record<string, string> | null> {
  const token = await getToken()
  if (!token) return null
  return { Authorization: `Bearer ${token}`, "Content-Type": "application/json" }
}

function noToken(): SettingsErr {
  return {
    ok: false,
    message: getUserFriendlyErrorMessage("auth_required"),
    code: "auth_required",
  }
}

function noBase(): SettingsErr {
  return {
    ok: false,
    message: getUserFriendlyErrorMessage("service_unavailable"),
    code: "service_unavailable",
  }
}

async function parseApiError(res: Response): Promise<string> {
  const fallback = getUserFriendlyErrorMessage(mapHttpStatusToErrorCode(res.status))
  try {
    const text = await res.text()
    if (!text.trim()) return fallback
    try {
      const parsed = JSON.parse(text) as { error?: string; message?: string }
      return parsed.error ?? parsed.message ?? fallback
    } catch {
      return text.length > 300 ? `${text.slice(0, 300)}…` : text
    }
  } catch {
    return fallback
  }
}

function errFromStatus(res: Response, message: string): SettingsErr {
  return { ok: false, message, code: mapHttpStatusToErrorCode(res.status) }
}

// Generic GET helper
async function get<T>(path: string): Promise<SettingsResult<T>> {
  const token = await getToken()
  if (!token) return noToken()
  const base = getServerApiBase()
  if (!base) return noBase()

  const result = await apiFetchJson<T>(`${base}${path}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })
  if (!result.ok) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage(result.error.code),
      code: result.error.code,
    }
  }
  return { ok: true, data: result.data }
}

// Generic mutation helper (PUT / POST / DELETE)
async function mutate<T = void>(
  path: string,
  method: "GET" | "POST" | "PUT" | "DELETE",
  body?: unknown,
): Promise<SettingsResult<T>> {
  const headers = await getAuthHeaders()
  if (!headers) return noToken()
  const base = getServerApiBase()
  if (!base) return noBase()

  try {
    const res = await fetch(`${base}${path}`, {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined,
    })
    if (res.ok) {
      if (res.status === 204) return { ok: true, data: undefined as T }
      return { ok: true, data: (await res.json()) as T }
    }
    return errFromStatus(res, await parseApiError(res))
  } catch {
    return noBase()
  }
}

// ---------------------------------------------------------------------------
// Permissions (full list from DB)
// ---------------------------------------------------------------------------

export async function getPermissions(): Promise<SettingsResult<PermissionDto[]>> {
  return get<PermissionDto[]>("/api/settings/permissions")
}

// ---------------------------------------------------------------------------
// Site Messages
// ---------------------------------------------------------------------------

export async function getSiteMessages(): Promise<SettingsResult<SiteMessageDto[]>> {
  return get<SiteMessageDto[]>("/api/settings/site-messages")
}

export async function addSiteMessage(
  body: SiteMessageRequest,
): Promise<SettingsResult<SiteMessageDto>> {
  return mutate<SiteMessageDto>("/api/settings/site-messages", "POST", body)
}

export async function deleteSiteMessage(id: number): Promise<SettingsResult<void>> {
  return mutate<void>(`/api/settings/site-messages/${id}`, "DELETE")
}

// ---------------------------------------------------------------------------
// ETL
// ---------------------------------------------------------------------------

export async function getEtl(): Promise<SettingsResult<SettingValueDto>> {
  return get<SettingValueDto>("/api/settings/etl")
}

export async function getDefaultEtl(): Promise<SettingsResult<string>> {
  const token = await getToken()
  if (!token) return noToken()
  const base = getServerApiBase()
  if (!base) return noBase()
  try {
    const res = await fetch(`${base}/api/settings/etl/default`, {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    })
    if (res.ok) return { ok: true, data: await res.text() }
    return errFromStatus(res, await parseApiError(res))
  } catch {
    return noBase()
  }
}

export async function updateEtl(value: string | null): Promise<SettingsResult<void>> {
  return mutate<void>("/api/settings/etl", "PUT", { value })
}

// ---------------------------------------------------------------------------
// Theme
// ---------------------------------------------------------------------------

export async function getTheme(): Promise<SettingsResult<SettingValueDto>> {
  return get<SettingValueDto>("/api/settings/theme")
}

export async function updateTheme(value: string | null): Promise<SettingsResult<void>> {
  return mutate<void>("/api/settings/theme", "PUT", { value })
}

// ---------------------------------------------------------------------------
// Search
// ---------------------------------------------------------------------------

export async function getSearch(): Promise<SettingsResult<SearchSettingsDto>> {
  return get<SearchSettingsDto>("/api/settings/search")
}

export async function updateSearchVisibility(
  type: string,
  visible: boolean,
  reportTypeId?: number,
): Promise<SettingsResult<void>> {
  const params =
    type === "reports" && reportTypeId !== undefined
      ? `?reportTypeId=${reportTypeId}`
      : ""
  return mutate<void>(`/api/settings/search/${type}/visibility${params}`, "PUT", { visible })
}

export async function updateSearchReportTypeText(
  id: number,
  text: string | null,
): Promise<SettingsResult<void>> {
  return mutate<void>(`/api/settings/search/report-types/${id}/text`, "PUT", { text })
}

// ---------------------------------------------------------------------------
// Tags / metadata parameters
// ---------------------------------------------------------------------------

export const TAG_TYPES = [
  "organizational-values",
  "estimated-run-frequencies",
  "maintenance-schedules",
  "fragilities",
  "fragility-tags",
  "tags",
  "maintenance-log-statuses",
  "financial-impacts",
  "strategic-importances",
] as const

export type TagType = (typeof TAG_TYPES)[number]

export async function getTags(type: TagType): Promise<SettingsResult<ParameterDto[]>> {
  return get<ParameterDto[]>(`/api/settings/tags/${type}`)
}

export async function createTag(
  type: TagType,
  body: ParameterRequest,
): Promise<SettingsResult<ParameterDto>> {
  return mutate<ParameterDto>(`/api/settings/tags/${type}`, "POST", body)
}

export async function deleteTag(type: TagType, id: number): Promise<SettingsResult<void>> {
  return mutate<void>(`/api/settings/tags/${type}/${id}`, "DELETE")
}

// ---------------------------------------------------------------------------
// Roles
// ---------------------------------------------------------------------------

export async function getRoles(): Promise<SettingsResult<RoleDto[]>> {
  return get<RoleDto[]>("/api/settings/roles")
}

export async function createRole(body: RoleRequest): Promise<SettingsResult<RoleDto>> {
  return mutate<RoleDto>("/api/settings/roles", "POST", body)
}

export async function deleteRole(id: number): Promise<SettingsResult<void>> {
  return mutate<void>(`/api/settings/roles/${id}`, "DELETE")
}

export async function updateRolePermission(
  roleId: number,
  permissionId: number,
  enabled: boolean,
): Promise<SettingsResult<void>> {
  return mutate<void>(`/api/settings/roles/${roleId}/permissions/${permissionId}`, "PUT", {
    enabled,
  })
}

// ---------------------------------------------------------------------------
// User Roles
// ---------------------------------------------------------------------------

export async function getUserRoles(): Promise<SettingsResult<UserRoleAssignmentDto[]>> {
  return get<UserRoleAssignmentDto[]>("/api/settings/user-roles")
}

export async function addUserRole(userId: number, roleId: number): Promise<SettingsResult<void>> {
  return mutate<void>(`/api/settings/user-roles/${userId}`, "POST", { roleId })
}

export async function removeUserRole(
  userId: number,
  roleId: number,
): Promise<SettingsResult<void>> {
  return mutate<void>(`/api/settings/user-roles/${userId}/${roleId}`, "DELETE")
}

// ---------------------------------------------------------------------------
// Group Roles
// ---------------------------------------------------------------------------

export async function getGroupRoles(): Promise<SettingsResult<GroupRoleAssignmentDto[]>> {
  return get<GroupRoleAssignmentDto[]>("/api/settings/group-roles")
}

export async function addGroupRole(
  groupId: number,
  roleId: number,
): Promise<SettingsResult<void>> {
  return mutate<void>(`/api/settings/group-roles/${groupId}`, "POST", { roleId })
}

export async function removeGroupRole(
  groupId: number,
  roleId: number,
): Promise<SettingsResult<void>> {
  return mutate<void>(`/api/settings/group-roles/${groupId}/${roleId}`, "DELETE")
}

// ---------------------------------------------------------------------------
// Available permissions list (re-used by the Roles UI)
// ---------------------------------------------------------------------------

export const ALL_PERMISSIONS: PermissionDto[] = [
  { id: 1, name: "Manage Global Site Settings" },
  { id: 2, name: "Create Parameters" },
  { id: 3, name: "Delete Parameters" },
  { id: 4, name: "Edit Role Permissions" },
  { id: 5, name: "Edit User Permissions" },
  { id: 6, name: "Edit Group Permissions" },
  { id: 7, name: "Approve Terms" },
  { id: 8, name: "Edit Report Documentation" },
  { id: 9, name: "Create New Terms" },
  { id: 10, name: "Edit Terms" },
  { id: 11, name: "Create Collection" },
  { id: 12, name: "Edit Collection" },
  { id: 13, name: "Delete Collection" },
  { id: 14, name: "Create Initiative" },
  { id: 15, name: "View Other User" },
  { id: 16, name: "Show Advanced Search" },
  { id: 17, name: "Show Report-Object Relationships" },
  { id: 18, name: "Manage Report-Object Relationships" },
  { id: 19, name: "Edit Report-Object Relationships" },
]
