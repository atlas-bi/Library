"use server"

import { revalidatePath } from "next/cache"
import { searchReportUsers } from "@/lib/reports/api"
import { searchLibrary } from "@/lib/search/api"
import type { TagType } from "@/lib/settings/api"
import {
  addGroupRole,
  addSiteMessage,
  addUserRole,
  createRole,
  createTag,
  deleteRole,
  deleteSiteMessage,
  deleteTag,
  removeGroupRole,
  removeUserRole,
  updateEtl,
  updateRolePermission,
  updateSearchReportTypeText,
  updateSearchVisibility,
  updateTheme,
} from "@/lib/settings/api"
import type { ParameterRequest, RoleRequest, SiteMessageRequest } from "@/lib/settings/types"

function revalidateSettings() {
  revalidatePath("/settings")
}

// ---------------------------------------------------------------------------
// Site Messages
// ---------------------------------------------------------------------------

export async function addSiteMessageAction(body: SiteMessageRequest) {
  const result = await addSiteMessage(body)
  if (!result.ok) return { error: result.message }
  revalidateSettings()
  return { data: result.data }
}

export async function deleteSiteMessageAction(id: number) {
  const result = await deleteSiteMessage(id)
  if (!result.ok) return { error: result.message }
  revalidateSettings()
  return { data: {} }
}

// ---------------------------------------------------------------------------
// ETL
// ---------------------------------------------------------------------------

export async function updateEtlAction(value: string | null) {
  const result = await updateEtl(value)
  if (!result.ok) return { error: result.message }
  revalidateSettings()
  return { data: {} }
}

// ---------------------------------------------------------------------------
// Theme
// ---------------------------------------------------------------------------

export async function updateThemeAction(value: string | null) {
  const result = await updateTheme(value)
  if (!result.ok) return { error: result.message }
  revalidateSettings()
  return { data: {} }
}

// ---------------------------------------------------------------------------
// Search
// ---------------------------------------------------------------------------

export async function updateSearchVisibilityAction(
  type: string,
  visible: boolean,
  reportTypeId?: number,
) {
  const result = await updateSearchVisibility(type, visible, reportTypeId)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: {} }
}

export async function updateSearchReportTypeTextAction(id: number, text: string | null) {
  const result = await updateSearchReportTypeText(id, text)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: {} }
}

// ---------------------------------------------------------------------------
// Tags
// ---------------------------------------------------------------------------

export async function createTagAction(type: TagType, body: ParameterRequest) {
  const result = await createTag(type, body)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: result.data }
}

export async function deleteTagAction(type: TagType, id: number) {
  const result = await deleteTag(type, id)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: {} }
}

// ---------------------------------------------------------------------------
// Roles
// ---------------------------------------------------------------------------

export async function createRoleAction(body: RoleRequest) {
  const result = await createRole(body)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: result.data }
}

export async function deleteRoleAction(id: number) {
  const result = await deleteRole(id)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: {} }
}

export async function updateRolePermissionAction(
  roleId: number,
  permissionId: number,
  enabled: boolean,
) {
  const result = await updateRolePermission(roleId, permissionId, enabled)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: {} }
}

// ---------------------------------------------------------------------------
// User Roles
// ---------------------------------------------------------------------------

export async function addUserRoleAction(userId: number, roleId: number) {
  const result = await addUserRole(userId, roleId)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: {} }
}

export async function removeUserRoleAction(userId: number, roleId: number) {
  const result = await removeUserRole(userId, roleId)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: {} }
}

// ---------------------------------------------------------------------------
// Group Roles
// ---------------------------------------------------------------------------

export async function addGroupRoleAction(groupId: number, roleId: number) {
  const result = await addGroupRole(groupId, roleId)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: {} }
}

export async function removeGroupRoleAction(groupId: number, roleId: number) {
  const result = await removeGroupRole(groupId, roleId)
  if (!result.ok) return { error: result.message }
  revalidatePath("/settings")
  return { data: {} }
}

// ---------------------------------------------------------------------------
// Lookup helpers for user/group assignment
// ---------------------------------------------------------------------------

export async function searchSettingsUsersAction(query: string) {
  return searchReportUsers(query)
}

export async function searchSettingsGroupsAction(query: string) {
  const trimmed = query.trim()
  if (!trimmed) return []

  const result = await searchLibrary({ q: trimmed, type: "groups" })
  if (!result.data?.results) return []

  return result.data.results
    .filter((item) => item.type === "groups")
    .map((item) => ({
      id: item.atlasId,
      name: item.name,
      description: item.description,
    }))
}
