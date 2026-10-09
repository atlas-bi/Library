"use server"

import { recordAnalyticsBeacon, recordAnalyticsTraces } from "@/lib/analytics/api"
import type { AnalyticsBeaconPayload, AnalyticsTraceIngestRequest } from "@/lib/analytics/types"

export async function submitAnalyticsBeaconAction(payload: AnalyticsBeaconPayload): Promise<void> {
  await recordAnalyticsBeacon(payload)
}

export async function submitAnalyticsTracesAction(
  request: AnalyticsTraceIngestRequest,
  options?: { userAgent?: string; referer?: string },
): Promise<void> {
  await recordAnalyticsTraces(request, options)
}
