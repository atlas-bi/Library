"use server"

import { loadRecentUpdates, type RecentUpdatesLoadResult } from "@/lib/home/recent-updates-api"

export async function loadRecentUpdatesAction(): Promise<RecentUpdatesLoadResult> {
  return loadRecentUpdates()
}
