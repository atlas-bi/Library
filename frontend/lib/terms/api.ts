import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { getUserFriendlyErrorMessage, mapHttpStatusToErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type {
  TermDetailDto,
  TermRelatedReportDto,
  TermWriteBody,
  TermsListDto,
} from "./types"

export type TermsListResult = {
  data: TermsListDto | null
  error: AppErrorCode | null
  status?: number
}

export type TermDetailResult = {
  data: TermDetailDto | null
  error: AppErrorCode | null
  status?: number
}

export type TermMutationOk = { ok: true; data: TermDetailDto }
export type TermMutationErr = { ok: false; message: string; code: AppErrorCode }
export type TermMutationResult = TermMutationOk | TermMutationErr

export type TermDeleteResult = { ok: true } | { ok: false; message: string; code: AppErrorCode }

export type TermReportsResult = {
  data: TermRelatedReportDto[] | null
  error: AppErrorCode | null
  status?: number
}

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

export async function getTermsList(): Promise<TermsListResult> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<TermsListDto>(`${apiBase}/api/terms`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code, status: result.error.status }
  return { data: result.data, error: null }
}

export async function getTermById(id: number): Promise<TermDetailResult> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<TermDetailDto>(`${apiBase}/api/terms/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code, status: result.error.status }
  return { data: result.data, error: null }
}

export async function getTermReports(id: number): Promise<TermReportsResult> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<TermRelatedReportDto[]>(`${apiBase}/api/terms/${id}/reports`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code, status: result.error.status }
  return { data: result.data, error: null }
}

export async function createTerm(body: TermWriteBody): Promise<TermMutationResult> {
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

  const res = await fetch(`${apiBase}/api/terms`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  })

  if (res.status === 201) {
    const data = (await res.json()) as TermDetailDto
    return { ok: true, data }
  }

  const message = await parseErrorMessage(res)
  return { ok: false, message, code: toErrorCode(res.status) }
}

export async function updateTerm(id: number, body: TermWriteBody): Promise<TermMutationResult> {
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

  const res = await fetch(`${apiBase}/api/terms/${id}`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  })

  if (res.ok) {
    const data = (await res.json()) as TermDetailDto
    return { ok: true, data }
  }

  const message = await parseErrorMessage(res)
  return { ok: false, message, code: toErrorCode(res.status) }
}

export async function deleteTerm(id: number): Promise<TermDeleteResult> {
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

  const res = await fetch(`${apiBase}/api/terms/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  })

  if (res.status === 204) return { ok: true }

  const message = await parseErrorMessage(res)
  return { ok: false, message, code: toErrorCode(res.status) }
}