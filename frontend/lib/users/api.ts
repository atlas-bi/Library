import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { getUserFriendlyErrorMessage, mapHttpStatusToErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type {
  CreateUserFavoriteFolderRequest,
  ReorderUserFavoriteFolderItem,
  ReorderUserFavoriteItem,
  ToggleAdminModeResponse,
  ToggleUserFavoriteRequest,
  ToggleUserFavoriteResponse,
  UpdateUserFavoriteFolderAssignmentRequest,
  UpdateUserFavoriteFolderRequest,
  UserFavoriteFolder,
  UserGroup,
  UserHistorySection,
  UserPage,
  UserSearchHistoryItem,
  UserSharedObjects,
  UserStars,
  UserSubscription,
} from "./types"

export type UsersReadResult<T> = {
  data: T | null
  error: AppErrorCode | null
  status?: number
}

export type UsersMutationOk<T> = { ok: true; data: T }
export type UsersMutationErr = { ok: false; message: string; code: AppErrorCode }
export type UsersMutationResult<T> = UsersMutationOk<T> | UsersMutationErr

async function authorizedGet<T>(path: string): Promise<UsersReadResult<T>> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<T>(`${apiBase}${path}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) {
    return { data: null, error: result.error.code, status: result.error.status }
  }
  return { data: result.data, error: null }
}

async function parseErrorMessage(res: Response): Promise<string> {
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

async function authorizedMutation<T>(
  path: string,
  init: RequestInit,
): Promise<UsersMutationResult<T>> {
  const token = await getToken()
  if (!token) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("auth_required"),
      code: "auth_required",
    }
  }

  const apiBase = getServerApiBase()
  if (!apiBase) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("service_unavailable"),
      code: "service_unavailable",
    }
  }

  const res = await fetch(`${apiBase}${path}`, {
    ...init,
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
      ...(init.headers ?? {}),
    },
    cache: "no-store",
  })

  if (res.ok) {
    if (res.status === 204) {
      return { ok: true, data: undefined as T }
    }
    const text = await res.text()
    if (!text.trim()) {
      return { ok: true, data: undefined as T }
    }
    return { ok: true, data: JSON.parse(text) as T }
  }

  return {
    ok: false,
    message: await parseErrorMessage(res),
    code: mapHttpStatusToErrorCode(res.status),
  }
}

export function getUserPage(id: number) {
  return authorizedGet<UserPage>(`/api/users/${id}`)
}

export function getUserStars(id: number) {
  return authorizedGet<UserStars>(`/api/users/${id}/stars`)
}

export function getUserGroups(id: number) {
  return authorizedGet<UserGroup[]>(`/api/users/${id}/groups`)
}

export function getUserSubscriptions(id: number) {
  return authorizedGet<UserSubscription[]>(`/api/users/${id}/subscriptions`)
}

export function getUserHistory(id: number) {
  return authorizedGet<UserHistorySection>(`/api/users/${id}/history`)
}

export function getUserSharedObjects() {
  return authorizedGet<UserSharedObjects>("/api/users/me/shared-objects")
}

export function getUserSearchHistory() {
  return authorizedGet<UserSearchHistoryItem[]>("/api/users/me/search-history")
}

export function createUserFolder(
  userId: number,
  isCurrentUser: boolean,
  body: CreateUserFavoriteFolderRequest,
) {
  const path = isCurrentUser ? "/api/users/me/folders" : `/api/users/${userId}/folders`
  return authorizedMutation<UserFavoriteFolder>(path, {
    method: "POST",
    body: JSON.stringify(body),
  })
}

export function updateUserFolder(
  userId: number,
  isCurrentUser: boolean,
  folderId: number,
  body: UpdateUserFavoriteFolderRequest,
) {
  const path = isCurrentUser
    ? `/api/users/me/folders/${folderId}`
    : `/api/users/${userId}/folders/${folderId}`
  return authorizedMutation<UserFavoriteFolder>(path, {
    method: "PUT",
    body: JSON.stringify(body),
  })
}

export function deleteUserFolder(userId: number, isCurrentUser: boolean, folderId: number) {
  const path = isCurrentUser
    ? `/api/users/me/folders/${folderId}`
    : `/api/users/${userId}/folders/${folderId}`
  return authorizedMutation<void>(path, { method: "DELETE" })
}

export function reorderUserFolders(
  userId: number,
  isCurrentUser: boolean,
  body: ReorderUserFavoriteFolderItem[],
) {
  const path = isCurrentUser
    ? "/api/users/me/folders/reorder"
    : `/api/users/${userId}/folders/reorder`
  return authorizedMutation<void>(path, {
    method: "POST",
    body: JSON.stringify(body),
  })
}

export function reorderUserFavorites(
  userId: number,
  isCurrentUser: boolean,
  body: ReorderUserFavoriteItem[],
) {
  const path = isCurrentUser
    ? "/api/users/me/favorites/reorder"
    : `/api/users/${userId}/favorites/reorder`
  return authorizedMutation<void>(path, {
    method: "POST",
    body: JSON.stringify(body),
  })
}

export function updateUserFavoriteFolderAssignment(
  userId: number,
  isCurrentUser: boolean,
  body: UpdateUserFavoriteFolderAssignmentRequest,
) {
  const path = isCurrentUser
    ? "/api/users/me/favorites/folder"
    : `/api/users/${userId}/favorites/folder`
  return authorizedMutation<void>(path, {
    method: "PUT",
    body: JSON.stringify(body),
  })
}

export function toggleUserFavorite(
  userId: number,
  isCurrentUser: boolean,
  body: ToggleUserFavoriteRequest,
) {
  const path = isCurrentUser
    ? "/api/users/me/favorites/toggle"
    : `/api/users/${userId}/favorites/toggle`
  return authorizedMutation<ToggleUserFavoriteResponse>(path, {
    method: "POST",
    body: JSON.stringify(body),
  })
}

export function removeUserSharedObject(id: number) {
  return authorizedMutation<void>(`/api/users/me/shared-objects/${id}`, { method: "DELETE" })
}

export function toggleAdminMode() {
  return authorizedMutation<ToggleAdminModeResponse>("/api/users/me/admin-mode/toggle", {
    method: "POST",
  })
}
