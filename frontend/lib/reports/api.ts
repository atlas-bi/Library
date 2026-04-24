import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"

import { apiFetchJson } from "@/lib/http"
import type { ReportDetail } from "./types"

export type ReportDetailResult = {
  data: ReportDetail | null
  error: AppErrorCode | null
}

export async function getReportDetailById(id: number): Promise<ReportDetailResult> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<ReportDetail>(`${apiBase}/api/reports/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code }
  return { data: result.data, error: null }
}
