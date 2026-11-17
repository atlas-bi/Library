import { getServerApiBase } from "@/lib/api-base"
import type { AppErrorCode } from "@/lib/app-error"
import { mapHttpStatusToErrorCode } from "@/lib/app-error"
import { getToken } from "@/lib/auth"

export type ReportFilePayload = {
  base64: string
  fileName: string
  contentType: string
}

export type ReportFileFetchResult =
  | { ok: true; data: ReportFilePayload }
  | { ok: false; code: AppErrorCode; status?: number }

export function parseContentDispositionFileName(header: string | null): string | null {
  if (!header) return null

  const utf8Match = /filename\*=UTF-8''([^;]+)/i.exec(header)
  if (utf8Match?.[1]) {
    try {
      return decodeURIComponent(utf8Match[1].trim())
    } catch {
      return utf8Match[1].trim()
    }
  }

  const quotedMatch = /filename="([^"]+)"/i.exec(header)
  if (quotedMatch?.[1]) return quotedMatch[1]

  const plainMatch = /filename=([^;]+)/i.exec(header)
  if (plainMatch?.[1]) return plainMatch[1].trim().replace(/^"|"$/g, "")

  return null
}

function toError(status: number): ReportFileFetchResult {
  return { ok: false, code: mapHttpStatusToErrorCode(status), status }
}

async function fetchReportFile(path: string, fallbackFileName: string): Promise<ReportFileFetchResult> {
  const token = await getToken()
  if (!token) return { ok: false, code: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { ok: false, code: "service_unavailable" }

  try {
    const response = await fetch(`${apiBase}${path}`, {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    })

    if (!response.ok) {
      return toError(response.status)
    }

    const buffer = await response.arrayBuffer()
    const base64 = Buffer.from(buffer).toString("base64")
    const contentType = response.headers.get("content-type")?.split(";")[0]?.trim() || "application/octet-stream"
    const fileName =
      parseContentDispositionFileName(response.headers.get("content-disposition")) ?? fallbackFileName

    return {
      ok: true,
      data: { base64, fileName, contentType },
    }
  } catch {
    return { ok: false, code: "unknown" }
  }
}

/** `GET /api/reports/{id}/cube` */
export async function fetchReportCubeFile(
  reportId: number,
  fallbackFileName: string,
): Promise<ReportFileFetchResult> {
  return fetchReportFile(`/api/reports/${reportId}/cube`, fallbackFileName)
}

/** `GET /api/reports/attachments/{id}` */
export async function fetchReportAttachmentFile(
  attachmentId: number,
  fallbackFileName: string,
): Promise<ReportFileFetchResult> {
  return fetchReportFile(`/api/reports/attachments/${attachmentId}`, fallbackFileName)
}
