import type { AnalyticsBeaconPayload } from "./types"

export const ANALYTICS_SESSION_KEY = "_sid"
export const ANALYTICS_PAGE_KEY = "_pid"
export const ANALYTICS_SESSION_TIMEOUT_MS = 2 * 60_000
export const ANALYTICS_BEACON_INTERVAL_MS = 0.5 * 60_000

export function createSessionId(now = new Date()): string {
  return btoa(now.toString())
}

export function readStoredId(storage: Pick<Storage, "getItem">, key: string): string | null {
  return storage.getItem(key)
}

export function writeStoredId(storage: Pick<Storage, "setItem" | "removeItem">, key: string, value: string | null) {
  if (value === null) {
    storage.removeItem(key)
    return
  }
  storage.setItem(key, value)
}

export function getNavigationLoadTime(performance: Performance): number {
  const timing = performance.timing
  if (!timing?.navigationStart || !timing.domContentLoadedEventEnd) {
    return 0
  }
  return timing.domContentLoadedEventEnd - timing.navigationStart
}

type BuildBeaconInput = {
  sessionId: string
  pageId: string
  pageStartedAtMs: number
  loadTime?: number | string
  nowMs?: number
  language: string
  userAgent: string
  location: Pick<Location, "host" | "hostname" | "href" | "protocol" | "search" | "pathname" | "origin">
  referrer: string
  screenHeight: number
  screenWidth: number
  devicePixelRatio: number
}

export function buildAnalyticsBeaconPayload(input: BuildBeaconInput): AnalyticsBeaconPayload {
  const nowMs = input.nowMs ?? Date.now()
  const loadTime = String(input.loadTime ?? 0)

  return {
    language: input.language,
    userAgent: input.userAgent,
    host: input.location.host,
    hostname: input.location.hostname,
    href: input.location.href,
    protocol: input.location.protocol,
    search: input.location.search,
    pathname: input.location.pathname,
    screenHeight: String(input.screenHeight),
    screenWidth: String(input.screenWidth),
    origin: input.location.origin,
    referrer: input.referrer,
    loadTime,
    zoom: input.devicePixelRatio,
    sessionId: input.sessionId,
    pageId: input.pageId,
    pageTime: nowMs - input.pageStartedAtMs,
  }
}
