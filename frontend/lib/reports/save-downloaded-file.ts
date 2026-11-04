import type { ReportFilePayload } from "@/lib/reports/report-files-api"

export function saveDownloadedFile(payload: ReportFilePayload): void {
  const binary = atob(payload.base64)
  const bytes = new Uint8Array(binary.length)
  for (let i = 0; i < binary.length; i += 1) {
    bytes[i] = binary.charCodeAt(i)
  }

  const blob = new Blob([bytes], { type: payload.contentType || "application/octet-stream" })
  const url = URL.createObjectURL(blob)
  const anchor = document.createElement("a")
  anchor.href = url
  anchor.download = payload.fileName
  anchor.rel = "noopener"
  document.body.appendChild(anchor)
  anchor.click()
  anchor.remove()
  URL.revokeObjectURL(url)
}
