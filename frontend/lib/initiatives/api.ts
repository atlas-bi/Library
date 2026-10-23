import { getServerApiBase } from "@/lib/api-base"
import type { AppErrorCode } from "@/lib/app-error"
import { getToken } from "@/lib/auth"
import { apiFetchJson } from "@/lib/http"
import type {
  InitiativeCollectionTypeaheadItemDto,
  InitiativeDetailDto,
  InitiativesListResponseDto,
  InitiativeWriteBody,
} from "./types"


export type ApiResult<T> = {
  data: T | null
  error: AppErrorCode | null
}

async function authorizedGet<T>(path: string): Promise<ApiResult<T>> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<T>(`${apiBase}${path}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code }
  return { data: result.data, error: null }
}

async function authorizedMutation<T>(
  path: string,
  method: "POST" | "PUT" | "DELETE",
  body?: unknown,
): Promise<ApiResult<T>> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const init: RequestInit = {
    method,
    headers: { Authorization: `Bearer ${token}` },
  }

  if (body) {
    init.headers = { ...init.headers, "Content-Type": "application/json" }
    init.body = JSON.stringify(body)
  }

  const result = await apiFetchJson<T>(`${apiBase}${path}`, init)

  if (!result.ok) return { data: null, error: result.error.code }
  return { data: result.data, error: null }
}

export async function getInitiatives(): Promise<ApiResult<InitiativesListResponseDto>> {
  return authorizedGet<InitiativesListResponseDto>("/api/initiatives")
}

export async function getInitiative(id: number): Promise<ApiResult<InitiativeDetailDto>> {
  return authorizedGet<InitiativeDetailDto>(`/api/initiatives/${id}`)
}

export async function createInitiative(
  data: InitiativeWriteBody,
): Promise<ApiResult<InitiativeDetailDto>> {
  return authorizedMutation<InitiativeDetailDto>("/api/initiatives", "POST", data)
}

export async function updateInitiative(
  id: number,
  data: InitiativeWriteBody,
): Promise<ApiResult<InitiativeDetailDto>> {
  return authorizedMutation<InitiativeDetailDto>(`/api/initiatives/${id}`, "PUT", data)
}

export async function deleteInitiative(id: number): Promise<ApiResult<void>> {
  return authorizedMutation<void>(`/api/initiatives/${id}`, "DELETE")
}

export async function searchInitiativeCollections(
  query: string,
): Promise<ApiResult<InitiativeCollectionTypeaheadItemDto[]>> {
  const params = new URLSearchParams()
  if (query) {
    params.set("q", query)
  }
  return authorizedGet<InitiativeCollectionTypeaheadItemDto[]>(
    `/api/initiatives/search/collections?${params.toString()}`,
  )
}

export interface ToggleStarResponseDto {
  type: string
  id: number
  isStarred: boolean
  count: number
}

export interface ShareRecipientDto {
  userId?: number | null
  type: string
}

export interface ShareMailRequestDto {
  draftId?: number | null
  to: ShareRecipientDto[]
  subject: string
  message: string
  text?: string | null
  share: boolean
  shareName?: string | null
  shareUrl?: string | null
}

export async function toggleStar(id: number): Promise<ApiResult<ToggleStarResponseDto>> {
  return authorizedMutation<ToggleStarResponseDto>("/api/interactions/stars/toggle", "POST", { type: "Initiative", id })
}

export async function shareMail(data: ShareMailRequestDto): Promise<ApiResult<void>> {
  return authorizedMutation<void>("/api/interactions/share-mail", "POST", data)
}
