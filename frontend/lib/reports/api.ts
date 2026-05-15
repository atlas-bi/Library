import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { getUserFriendlyErrorMessage, mapHttpStatusToErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type {
  MaintenanceStatus,
  ReportDetail,
  ReportLookupItem,
  ReportsListResponse,
  ReportTypeaheadItem,
  ReportUpdateBody,
} from "./types"

export type ReportDetailResult = {
  data: ReportDetail | null
  error: AppErrorCode | null
}

export type ReportsListResult = {
  data: ReportsListResponse | null
  error: AppErrorCode | null
}

export type ReportMutationOk = { ok: true; data: ReportDetail }
export type ReportMutationErr = { ok: false; message: string; code: AppErrorCode }
export type ReportMutationResult = ReportMutationOk | ReportMutationErr

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

function toErrorCode(status: number): AppErrorCode {
  return mapHttpStatusToErrorCode(status)
}

async function authorizedGet<T>(
  path: string,
): Promise<{ data: T | null; error: AppErrorCode | null }> {
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

export async function getReportsList(page = 1, pageSize = 20): Promise<ReportsListResult> {
  const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
  const result = await authorizedGet<ReportsListResponse>(`/api/reports?${params}`)
  return { data: result.data, error: result.error }
}

export async function getReportDetailById(id: number): Promise<ReportDetailResult> {
  const result = await authorizedGet<ReportDetail>(`/api/reports/${id}`)
  return { data: result.data, error: result.error }
}

export async function getReportTerms(id: number) {
  return authorizedGet<ReportDetail["terms"]>(`/api/reports/${id}/terms`)
}

export async function getReportQueries(id: number) {
  return authorizedGet<{
    queries?: ReportDetail["queries"]
    componentQueries?: ReportDetail["componentQueries"]
  }>(`/api/reports/${id}/queries`)
}

export async function getReportRelationships(id: number) {
  return authorizedGet<{
    parents?: ReportDetail["parents"]
    children?: ReportDetail["children"]
    collections?: ReportDetail["collections"]
    groups?: ReportDetail["groups"]
  }>(`/api/reports/${id}/relationships`)
}

export async function getReportMaintenanceStatus(id: number) {
  return authorizedGet<MaintenanceStatus>(`/api/reports/${id}/maintenance-status`)
}

export async function getReportLookups(lookupArea: string) {
  return authorizedGet<ReportLookupItem[]>(`/api/reports/lookups/${encodeURIComponent(lookupArea)}`)
}

export async function searchReportTerms(q: string): Promise<ReportTypeaheadItem[]> {
  const trimmed = q.trim()
  if (!trimmed) return []
  const params = new URLSearchParams({ q: trimmed })
  const result = await authorizedGet<ReportTypeaheadItem[]>(`/api/reports/search/terms?${params}`)
  return Array.isArray(result.data) ? result.data : []
}

export async function searchReportCollections(q: string): Promise<ReportTypeaheadItem[]> {
  const trimmed = q.trim()
  if (!trimmed) return []
  const params = new URLSearchParams({ q: trimmed })
  const result = await authorizedGet<ReportTypeaheadItem[]>(
    `/api/reports/search/collections?${params}`,
  )
  return Array.isArray(result.data) ? result.data : []
}

export async function searchReportUsers(q: string): Promise<ReportTypeaheadItem[]> {
  const trimmed = q.trim()
  if (!trimmed) return []
  const params = new URLSearchParams({ q: trimmed })
  const result = await authorizedGet<ReportTypeaheadItem[]>(`/api/reports/search/users?${params}`)
  return Array.isArray(result.data) ? result.data : []
}

export async function updateReport(
  id: number,
  body: ReportUpdateBody,
): Promise<ReportMutationResult> {
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

  const res = await fetch(`${apiBase}/api/reports/${id}`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  })

  if (res.ok) {
    return { ok: true, data: (await res.json()) as ReportDetail }
  }

  return { ok: false, message: await parseErrorMessage(res), code: toErrorCode(res.status) }
}

export async function uploadReportImage(id: number, file: File): Promise<ReportMutationResult> {
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

  const formData = new FormData()
  formData.append("file", file)

  const res = await fetch(`${apiBase}/api/reports/${id}/images`, {
    method: "POST",
    headers: { Authorization: `Bearer ${token}` },
    body: formData,
  })

  if (res.ok) {
    return { ok: true, data: (await res.json()) as ReportDetail }
  }

  return { ok: false, message: await parseErrorMessage(res), code: toErrorCode(res.status) }
}
