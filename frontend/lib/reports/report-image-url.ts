/**
 * Report image endpoints (GET, session cookie): see ReportImagesApiController.
 * Next.js rewrites forward `atlas_token` to the API for these paths.
 */

function normalizeApiBase(apiBase: string | undefined): string | undefined {
  return apiBase?.replace(/\/+$/, "")
}

function joinApiBase(apiBase: string | undefined, path: string): string {
  const base = normalizeApiBase(apiBase)
  if (!base) return path
  return `${base}${path}`
}

/** Relative path: GET /api/reports/{reportId}/thumbnail?size=&imgId=&type= */
export function getReportThumbnailPath(
  reportId: number,
  options?: { imageId?: number; size?: string; type?: string },
): string {
  const size = options?.size ?? "256x256"
  const params = new URLSearchParams({ size })
  if (options?.imageId != null) {
    params.set("imgId", String(options.imageId))
  }
  if (options?.type) {
    params.set("type", options.type)
  }
  return `/api/reports/${reportId}/thumbnail?${params.toString()}`
}

export function getReportThumbnailUrl(
  reportId: number,
  options?: { apiBase?: string; imageId?: number; size?: string; type?: string },
): string {
  return joinApiBase(options?.apiBase, getReportThumbnailPath(reportId, options))
}

/** Relative path: GET /api/reports/images/placeholder?size= */
export function getReportImagePlaceholderPath(options?: { size?: string }): string {
  const size = options?.size
  if (!size) {
    return "/api/reports/images/placeholder"
  }
  return `/api/reports/images/placeholder?${new URLSearchParams({ size }).toString()}`
}

export function getReportImagePlaceholderUrl(options?: {
  apiBase?: string
  size?: string
}): string {
  return joinApiBase(options?.apiBase, getReportImagePlaceholderPath(options))
}
