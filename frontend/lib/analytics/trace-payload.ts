import type { AnalyticsTraceIngestRequest } from "./types"

/** JSNLog fatal level (see Analytics Trace.cshtml.cs). */
export const ANALYTICS_TRACE_FATAL_LEVEL = 6000
export const ANALYTICS_ONERROR_LOGGER = "onerrorLogger"

export function buildFatalTraceIngest(message: string): AnalyticsTraceIngestRequest {
  return {
    lg: [
      {
        l: ANALYTICS_TRACE_FATAL_LEVEL,
        m: message,
        n: ANALYTICS_ONERROR_LOGGER,
      },
    ],
  }
}

export function serializeBrowserErrorPayload(payload: Record<string, unknown>): string {
  return JSON.stringify(payload)
}
