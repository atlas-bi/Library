import { describe, expect, it } from "vitest"
import {
  ANALYTICS_ONERROR_LOGGER,
  ANALYTICS_TRACE_FATAL_LEVEL,
  buildFatalTraceIngest,
} from "./trace-payload"

describe("buildFatalTraceIngest", () => {
  it("uses JSNLog fatal level and onerror logger name", () => {
    const body = buildFatalTraceIngest('{"msg":"Uncaught Exception"}')
    expect(body.lg).toHaveLength(1)
    expect(body.lg[0].l).toBe(ANALYTICS_TRACE_FATAL_LEVEL)
    expect(body.lg[0].n).toBe(ANALYTICS_ONERROR_LOGGER)
  })
})
