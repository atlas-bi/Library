import { beforeEach, describe, expect, test, vi } from "vitest"

vi.mock("@/lib/auth", () => ({
  getToken: vi.fn(async () => "test-token"),
}))

vi.mock("@/lib/api-base", () => ({
  getServerApiBase: vi.fn(() => "https://api.example.test"),
}))

import { getServerApiBase } from "@/lib/api-base"
import {
  fetchReportAttachmentFile,
  fetchReportCubeFile,
  parseContentDispositionFileName,
} from "./report-files-api"

const fetchMock = vi.fn<(input: RequestInfo | URL, init?: RequestInit) => Promise<Response>>()

describe("parseContentDispositionFileName", () => {
  test("reads quoted filename values", () => {
    expect(parseContentDispositionFileName('attachment; filename="Patient Flow.pdf"')).toBe(
      "Patient Flow.pdf",
    )
  })
})

describe("fetchReportCubeFile", () => {
  beforeEach(() => {
    vi.stubGlobal("fetch", fetchMock)
    vi.mocked(getServerApiBase).mockReturnValue("https://api.example.test")
    fetchMock.mockReset()
  })

  test("returns file bytes on success", async () => {
    fetchMock.mockResolvedValueOnce(
      new Response("cube-bytes", {
        status: 200,
        headers: {
          "content-type": "text/plain",
          "content-disposition": 'attachment; filename="Revenue.odc"',
        },
      }),
    )

    const result = await fetchReportCubeFile(7, "fallback.odc")

    expect(fetchMock).toHaveBeenCalledWith(
      "https://api.example.test/api/reports/7/cube",
      expect.objectContaining({
        headers: { Authorization: "Bearer test-token" },
      }),
    )
    expect(result.ok).toBe(true)
    if (result.ok) {
      expect(result.data.fileName).toBe("Revenue.odc")
      expect(atob(result.data.base64)).toBe("cube-bytes")
    }
  })

  test("returns not_found for 404", async () => {
    fetchMock.mockResolvedValueOnce(new Response("", { status: 404 }))

    const result = await fetchReportCubeFile(7, "fallback.odc")

    expect(result).toEqual({ ok: false, code: "not_found", status: 404 })
  })
})

describe("fetchReportAttachmentFile", () => {
  beforeEach(() => {
    vi.stubGlobal("fetch", fetchMock)
    fetchMock.mockReset()
  })

  test("returns forbidden for 403", async () => {
    fetchMock.mockResolvedValueOnce(new Response("", { status: 403 }))

    const result = await fetchReportAttachmentFile(12, "output.pdf")

    expect(fetchMock).toHaveBeenCalledWith(
      "https://api.example.test/api/reports/attachments/12",
      expect.any(Object),
    )
    expect(result).toEqual({ ok: false, code: "forbidden", status: 403 })
  })
})
