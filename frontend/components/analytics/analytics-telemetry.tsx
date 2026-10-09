"use client"

import { usePathname } from "next/navigation"
import { useEffect, useRef } from "react"
import {
  submitAnalyticsBeaconAction,
  submitAnalyticsTracesAction,
} from "@/app/analytics/telemetry-actions"
import {
  ANALYTICS_BEACON_INTERVAL_MS,
  ANALYTICS_PAGE_KEY,
  ANALYTICS_SESSION_KEY,
  ANALYTICS_SESSION_TIMEOUT_MS,
  buildAnalyticsBeaconPayload,
  createSessionId,
  getNavigationLoadTime,
  readStoredId,
  writeStoredId,
} from "@/lib/analytics/beacon-payload"
import { shouldIgnoreBrowserError } from "@/lib/analytics/browser-error-filter"
import {
  buildFatalTraceIngest,
  serializeBrowserErrorPayload,
} from "@/lib/analytics/trace-payload"

type AnalyticsTelemetryProps = {
  enabled: boolean
}

export function AnalyticsTelemetry({ enabled }: AnalyticsTelemetryProps) {
  const pathname = usePathname()
  const pathnameRef = useRef(pathname)
  const ajaxOnRef = useRef(true)
  const pageStartedAtRef = useRef(0)
  const postBeaconRef = useRef<(loadTime?: number, newPage?: boolean) => void>(() => {})

  useEffect(() => {
    if (!enabled || typeof window === "undefined") {
      return
    }

    const storage = window.sessionStorage
    let sessionTimerId: number | null = null
    let beaconTimerId: number | null = null
    let activityDebounceId: number | null = null

    const resetPageId = () => {
      pageStartedAtRef.current = Date.now()
      const pageId = createSessionId()
      writeStoredId(storage, ANALYTICS_PAGE_KEY, pageId)
      return pageId
    }

    const ensureSessionId = (reset?: "reset" | "clear") => {
      if (reset === "clear") {
        writeStoredId(storage, ANALYTICS_SESSION_KEY, null)
        return null
      }

      let sessionId = readStoredId(storage, ANALYTICS_SESSION_KEY)
      if (reset === "reset" || !sessionId) {
        sessionId = createSessionId()
        writeStoredId(storage, ANALYTICS_SESSION_KEY, sessionId)
        resetPageId()
      }
      return sessionId
    }

    const getPageId = () => {
      const existing = readStoredId(storage, ANALYTICS_PAGE_KEY)
      if (existing) return existing
      return resetPageId()
    }

    const resetBeaconTimer = () => {
      if (beaconTimerId !== null) {
        window.clearTimeout(beaconTimerId)
      }
      beaconTimerId = window.setTimeout(() => {
        postBeacon()
      }, ANALYTICS_BEACON_INTERVAL_MS)
    }

    const postBeacon = (loadTime?: number, newPage?: boolean) => {
      if (!ajaxOnRef.current) return

      if (newPage) {
        resetPageId()
        ajaxOnRef.current = true
      }

      const sessionId = ensureSessionId()
      if (!sessionId) return

      const payload = buildAnalyticsBeaconPayload({
        sessionId,
        pageId: getPageId(),
        pageStartedAtMs: pageStartedAtRef.current,
        loadTime: loadTime ?? 0,
        language: navigator.language,
        userAgent: navigator.userAgent,
        location: window.location,
        referrer: document.referrer,
        screenHeight: document.documentElement.clientHeight,
        screenWidth: document.documentElement.clientWidth,
        devicePixelRatio: window.devicePixelRatio,
      })

      void submitAnalyticsBeaconAction(payload)
      resetBeaconTimer()
    }

    postBeaconRef.current = postBeacon

    const doInactive = () => {
      ajaxOnRef.current = false
      ensureSessionId("clear")
      storage.clear()
    }

    const startSessionTimer = () => {
      if (sessionTimerId !== null) {
        window.clearTimeout(sessionTimerId)
      }
      sessionTimerId = window.setTimeout(doInactive, ANALYTICS_SESSION_TIMEOUT_MS)
      ensureSessionId()
    }

    const onActivity = () => {
      if (activityDebounceId !== null) {
        window.clearTimeout(activityDebounceId)
      }
      activityDebounceId = window.setTimeout(() => {
        startSessionTimer()
        ajaxOnRef.current = true
      }, 500)
    }

    const sendTrace = (payload: Record<string, unknown>) => {
      if (shouldIgnoreBrowserError(payload as { errorMsg?: string; url?: string; trace?: string })) {
        return
      }
      const message = serializeBrowserErrorPayload(payload)
      void submitAnalyticsTracesAction(buildFatalTraceIngest(message), {
        userAgent: navigator.userAgent,
        referer: window.location.href,
      })
    }

    const previousOnError = window.onerror
    const previousOnUnhandledRejection = window.onunhandledrejection

    window.onerror = (errorMsg, url, lineNumber, column, errorObj) => {
      const msgText =
        typeof errorMsg === "object" && errorMsg !== null && "message" in errorMsg
          ? String((errorMsg as Error).message)
          : String(errorMsg ?? "")
      const payload = {
        msg: "Uncaught Exception",
        errorMsg: msgText,
        url: url ?? "",
        "line number": lineNumber,
        column,
        trace: errorObj?.stack ?? "",
      }
      sendTrace(payload)
      if (typeof previousOnError === "function") {
        return previousOnError(errorMsg, url, lineNumber, column, errorObj)
      }
      return false
    }

    window.onunhandledrejection = (event: PromiseRejectionEvent) => {
      const reason = event.reason
      const payload = {
        msg: "unhandledrejection",
        errorMsg:
          reason instanceof Error
            ? reason.message
            : reason != null
              ? String(reason)
              : event.message ?? "",
        trace: reason instanceof Error ? reason.stack ?? "" : "",
      }
      sendTrace(payload)
      if (typeof previousOnUnhandledRejection === "function") {
        previousOnUnhandledRejection.call(window, event)
      }
    }

    document.addEventListener("mousemove", onActivity, false)
    document.addEventListener("mousedown", onActivity, false)
    document.addEventListener("keypress", onActivity, false)
    document.addEventListener("touchmove", onActivity, false)
    document.addEventListener("scroll", onActivity, { passive: true })

    startSessionTimer()
    postBeacon(getNavigationLoadTime(window.performance), true)

    return () => {
      document.removeEventListener("mousemove", onActivity, false)
      document.removeEventListener("mousedown", onActivity, false)
      document.removeEventListener("keypress", onActivity, false)
      document.removeEventListener("touchmove", onActivity, false)
      document.removeEventListener("scroll", onActivity)
      if (sessionTimerId !== null) window.clearTimeout(sessionTimerId)
      if (beaconTimerId !== null) window.clearTimeout(beaconTimerId)
      if (activityDebounceId !== null) window.clearTimeout(activityDebounceId)
      window.onerror = previousOnError
      window.onunhandledrejection = previousOnUnhandledRejection
      postBeaconRef.current = () => {}
    }
  }, [enabled])

  useEffect(() => {
    if (!enabled) return
    if (pathnameRef.current === pathname) return
    pathnameRef.current = pathname
    postBeaconRef.current(0, true)
  }, [enabled, pathname])

  return null
}
