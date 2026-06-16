import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type { GroupDetail, GroupListResponse, GroupReport, GroupUser } from "./types"

export type GroupListResult = {
  data: GroupListResponse | null
  error: AppErrorCode | null
  status?: number
}

export type GroupDetailResult = {
  data: GroupDetail | null
  error: AppErrorCode | null
  status?: number
}

export type GroupUsersResult = {
  data: GroupUser[] | null
  error: AppErrorCode | null
  status?: number
}

export type GroupReportsResult = {
  data: GroupReport[] | null
  error: AppErrorCode | null
  status?: number
}

async function authorizedGet<T>(path: string): Promise<{
  data: T | null
  error: AppErrorCode | null
  status?: number
}> {
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

export function getGroupsList(): Promise<GroupListResult> {
  return authorizedGet<GroupListResponse>("/api/groups")
}

export function getGroupById(id: number): Promise<GroupDetailResult> {
  return authorizedGet<GroupDetail>(`/api/groups/${id}`)
}

export function getGroupUsers(id: number): Promise<GroupUsersResult> {
  return authorizedGet<GroupUser[]>(`/api/groups/${id}/users`)
}

export function getGroupReports(id: number): Promise<GroupReportsResult> {
  return authorizedGet<GroupReport[]>(`/api/groups/${id}/reports`)
}
