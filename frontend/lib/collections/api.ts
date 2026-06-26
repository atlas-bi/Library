import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { getUserFriendlyErrorMessage, mapHttpStatusToErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type {
  CollectionDetailDto,
  CollectionsListResponseDto,
  CollectionTypeaheadItemDto,
  CollectionWriteBody,
} from "./types"

export type CollectionsListResult = {
  data: CollectionsListResponseDto | null
  error: AppErrorCode | null
  status?: number
}

export type CollectionDetailResult = {
  data: CollectionDetailDto | null
  error: AppErrorCode | null
  status?: number
}

export type CollectionMutationOk = { ok: true; data: CollectionDetailDto }
export type CollectionMutationErr = { ok: false; message: string; code: AppErrorCode }
export type CollectionMutationResult = CollectionMutationOk | CollectionMutationErr

export type CollectionDeleteResult =
  | { ok: true }
  | { ok: false; message: string; code: AppErrorCode }

async function parseErrorMessage(res: Response): Promise<string> {
  const code = mapHttpStatusToErrorCode(res.status)
  const fallback = getUserFriendlyErrorMessage(code)
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

function toErrorCode(status: number): AppErrorCode {
  return mapHttpStatusToErrorCode(status)
}

export async function getCollectionsList(page = 1, pageSize = 20): Promise<CollectionsListResult> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  })
  const result = await apiFetchJson<CollectionsListResponseDto>(
    `${apiBase}/api/collections?${params}`,
    {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    },
  )

  if (!result.ok) return { data: null, error: result.error.code, status: result.error.status }
  return { data: result.data, error: null }
}

export async function getCollectionById(id: number): Promise<CollectionDetailResult> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<CollectionDetailDto>(`${apiBase}/api/collections/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code, status: result.error.status }
  return { data: result.data, error: null }
}

export async function createCollection(
  body: CollectionWriteBody,
): Promise<CollectionMutationResult> {
  const token = await getToken()
  if (!token)
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("auth_required"),
      code: "auth_required",
    }

  const apiBase = getServerApiBase()
  if (!apiBase) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("service_unavailable"),
      code: "service_unavailable",
    }
  }

  const res = await fetch(`${apiBase}/api/collections`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  })

  if (res.status === 201) {
    const data = (await res.json()) as CollectionDetailDto
    return { ok: true, data }
  }

  const message = await parseErrorMessage(res)
  return { ok: false, message, code: toErrorCode(res.status) }
}

export async function updateCollection(
  id: number,
  body: CollectionWriteBody,
): Promise<CollectionMutationResult> {
  const token = await getToken()
  if (!token)
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("auth_required"),
      code: "auth_required",
    }

  const apiBase = getServerApiBase()
  if (!apiBase) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("service_unavailable"),
      code: "service_unavailable",
    }
  }

  const res = await fetch(`${apiBase}/api/collections/${id}`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  })

  if (res.ok) {
    const data = (await res.json()) as CollectionDetailDto
    return { ok: true, data }
  }

  const message = await parseErrorMessage(res)
  return { ok: false, message, code: toErrorCode(res.status) }
}

export async function deleteCollection(id: number): Promise<CollectionDeleteResult> {
  const token = await getToken()
  if (!token)
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("auth_required"),
      code: "auth_required",
    }

  const apiBase = getServerApiBase()
  if (!apiBase) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("service_unavailable"),
      code: "service_unavailable",
    }
  }

  const res = await fetch(`${apiBase}/api/collections/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  })

  if (res.status === 204) return { ok: true }

  const message = await parseErrorMessage(res)
  return { ok: false, message, code: toErrorCode(res.status) }
}

export async function searchCollectionTerms(q: string): Promise<CollectionTypeaheadItemDto[]> {
  const trimmed = q.trim()
  if (!trimmed) return []

  const token = await getToken()
  if (!token) return []

  const apiBase = getServerApiBase()
  if (!apiBase) return []

  const params = new URLSearchParams({ q: trimmed })
  const result = await apiFetchJson<CollectionTypeaheadItemDto[]>(
    `${apiBase}/api/collections/search/terms?${params}`,
    {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    },
  )

  if (!result.ok) return []
  return Array.isArray(result.data) ? result.data : []
}

export async function searchCollectionReports(q: string): Promise<CollectionTypeaheadItemDto[]> {
  const trimmed = q.trim()
  if (!trimmed) return []

  const token = await getToken()
  if (!token) return []

  const apiBase = getServerApiBase()
  if (!apiBase) return []

  const params = new URLSearchParams({ q: trimmed })
  const result = await apiFetchJson<CollectionTypeaheadItemDto[]>(
    `${apiBase}/api/collections/search/reports?${params}`,
    {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    },
  )

  if (!result.ok) return []
  return Array.isArray(result.data) ? result.data : []
}
