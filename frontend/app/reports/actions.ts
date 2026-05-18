"use server"

import { revalidatePath } from "next/cache"
import { redirect } from "next/navigation"
import {
  searchReportCollections,
  searchReportTerms,
  searchReportUsers,
  updateReport,
  uploadReportImage,
} from "@/lib/reports/api"
import type { ReportUpdateBody } from "@/lib/reports/types"

export async function searchReportTermsAction(query: string) {
  return searchReportTerms(query)
}

export async function searchReportCollectionsAction(query: string) {
  return searchReportCollections(query)
}

export async function searchReportUsersAction(query: string) {
  return searchReportUsers(query)
}

export async function updateReportAction(
  id: number,
  body: ReportUpdateBody,
): Promise<{ error: string } | undefined> {
  const result = await updateReport(id, body)
  if (!result.ok) {
    return { error: result.message }
  }
  revalidatePath("/reports")
  redirect(`/reports?id=${id}`)
}

export async function uploadReportImageAction(
  id: number,
  formData: FormData,
): Promise<{ error: string } | { ok: true }> {
  const file = formData.get("file")
  if (!(file instanceof File) || file.size === 0) {
    return { error: "Please choose an image file." }
  }
  if (file.size > 1024 * 1024) {
    return { error: "Image must be 1 MB or smaller." }
  }

  const result = await uploadReportImage(id, file)
  if (!result.ok) {
    return { error: result.message }
  }
  revalidatePath("/reports")
  return { ok: true }
}
