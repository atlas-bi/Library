import { describe, expect, it } from "vitest"
import { mapHttpStatusToErrorCode } from "@/lib/errors"

describe("mapHttpStatusToErrorCode", () => {
  it("maps common statuses", () => {
    expect(mapHttpStatusToErrorCode(400)).toBe("bad_request")
    expect(mapHttpStatusToErrorCode(401)).toBe("auth_required")
    expect(mapHttpStatusToErrorCode(403)).toBe("forbidden")
    expect(mapHttpStatusToErrorCode(404)).toBe("not_found")
    expect(mapHttpStatusToErrorCode(503)).toBe("service_unavailable")
    expect(mapHttpStatusToErrorCode(500)).toBe("server_error")
  })

  it("maps unknown statuses to unknown", () => {
    expect(mapHttpStatusToErrorCode(418)).toBe("unknown")
  })
})
