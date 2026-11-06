"use server"

import { getRecentTerms, type RecentTermsResult } from "@/lib/terms/api"

export async function loadRecentTermsAction(): Promise<RecentTermsResult> {
  return getRecentTerms()
}
