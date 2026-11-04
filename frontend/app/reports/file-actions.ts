"use server"

import {
  fetchReportAttachmentFile,
  fetchReportCubeFile,
  type ReportFileFetchResult,
} from "@/lib/reports/report-files-api"

export async function downloadReportCubeAction(
  reportId: number,
  fallbackFileName: string,
): Promise<ReportFileFetchResult> {
  return fetchReportCubeFile(reportId, fallbackFileName)
}

export async function downloadReportAttachmentAction(
  attachmentId: number,
  fallbackFileName: string,
): Promise<ReportFileFetchResult> {
  return fetchReportAttachmentFile(attachmentId, fallbackFileName)
}
