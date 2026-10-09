import { render, waitFor } from "@testing-library/react"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"
import { AnalyticsTelemetry } from "./analytics-telemetry"

const submitAnalyticsBeaconActionMock = vi.fn()
const submitAnalyticsTracesActionMock = vi.fn()

vi.mock("@/app/analytics/telemetry-actions", () => ({
  submitAnalyticsBeaconAction: (...args: unknown[]) => submitAnalyticsBeaconActionMock(...args),
  submitAnalyticsTracesAction: (...args: unknown[]) => submitAnalyticsTracesActionMock(...args),
}))

vi.mock("next/navigation", () => ({
  usePathname: () => "/terms",
}))

describe("AnalyticsTelemetry", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    submitAnalyticsBeaconActionMock.mockResolvedValue(undefined)
    submitAnalyticsTracesActionMock.mockResolvedValue(undefined)
    sessionStorage.clear()
    Object.defineProperty(window.performance, "timing", {
      configurable: true,
      value: {
        navigationStart: 0,
        domContentLoadedEventEnd: 100,
      },
    })
  })

  afterEach(() => {
    vi.restoreAllMocks()
    window.onerror = null
    window.onunhandledrejection = null
  })

  it("does not send beacons when telemetry is disabled", async () => {
    render(<AnalyticsTelemetry enabled={false} />)

    await waitFor(() => {
      expect(submitAnalyticsBeaconActionMock).not.toHaveBeenCalled()
    })
  })

  it("sends an initial beacon when enabled", async () => {
    render(<AnalyticsTelemetry enabled={true} />)

    await waitFor(() => {
      expect(submitAnalyticsBeaconActionMock).toHaveBeenCalled()
    })
  })

  it("ignores extension script errors", async () => {
    render(<AnalyticsTelemetry enabled={true} />)

    await waitFor(() => {
      expect(submitAnalyticsBeaconActionMock).toHaveBeenCalled()
    })

    submitAnalyticsTracesActionMock.mockClear()
    window.onerror?.("Extension failed", "chrome-extension://abc/content.js", 1, 1, undefined)

    await waitFor(() => {
      expect(submitAnalyticsTracesActionMock).not.toHaveBeenCalled()
    })
  })
})
