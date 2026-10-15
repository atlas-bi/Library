import { beforeEach, describe, expect, it, vi } from "vitest"

vi.mock("@/lib/interactions/api", () => ({
  submitAccessRequest: vi.fn(),
}))

vi.mock("next/cache", () => ({
  revalidatePath: vi.fn(),
}))

import { submitAccessRequest } from "@/lib/interactions/api"
import { submitAccessRequestAction } from "./actions"

const VALID_BODY = {
  reportName: "Sales Dashboard",
  reportUrl: "https://example.com/reports/1",
  directorName: "Jane Smith",
}

describe("submitAccessRequestAction", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("delegates to submitAccessRequest and returns data on success", async () => {
    vi.mocked(submitAccessRequest).mockResolvedValueOnce({ ok: true, data: {} })

    const result = await submitAccessRequestAction(VALID_BODY)

    expect(submitAccessRequest).toHaveBeenCalledWith(VALID_BODY)
    expect(result).toEqual({ data: {} })
  })

  it("returns the API error message when submitAccessRequest fails", async () => {
    vi.mocked(submitAccessRequest).mockResolvedValueOnce({
      ok: false,
      message: "Report name is required.",
      code: "bad_request",
    })

    const result = await submitAccessRequestAction(VALID_BODY)

    expect(submitAccessRequest).toHaveBeenCalledWith(VALID_BODY)
    expect(result).toEqual({ error: "Report name is required." })
  })
})
