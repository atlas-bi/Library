import { beforeEach, describe, expect, test, vi } from "vitest"

vi.mock("@/lib/auth", () => ({
  getToken: vi.fn(async () => "test-token"),
}))

vi.mock("@/lib/api-base", () => ({
  getServerApiBase: vi.fn(() => "https://api.example.test"),
}))

vi.mock("@/lib/http", () => ({
  apiFetchJson: vi.fn(),
}))

import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import { apiFetchJson } from "@/lib/http"
import { getTasks } from "./api"
import type { TasksResponseDto } from "./types"

const BASE = "https://api.example.test"

const emptyTasks: TasksResponseDto = {
  canMakeReports: [],
  recommendRetire: [],
  unused: [],
  maintenanceRequired: [],
  audit: [],
  missingSchedule: [],
  notInAnalytics: [],
  topUndocumented: [],
  newUndocumented: [],
}

describe("getTasks", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getToken).mockResolvedValue("test-token")
    vi.mocked(getServerApiBase).mockReturnValue(BASE)
  })

  test("calls GET /api/tasks with bearer auth", async () => {
    vi.mocked(apiFetchJson).mockResolvedValueOnce({
      ok: true,
      data: emptyTasks,
    })

    const result = await getTasks()

    expect(result).toEqual({ data: emptyTasks, error: null })
    expect(apiFetchJson).toHaveBeenCalledWith(`${BASE}/api/tasks`, {
      headers: { Authorization: "Bearer test-token" },
      cache: "no-store",
    })
  })

  test("returns auth_required when there is no token", async () => {
    vi.mocked(getToken).mockResolvedValue(null)

    const result = await getTasks()

    expect(result).toEqual({ data: null, error: "auth_required" })
    expect(apiFetchJson).not.toHaveBeenCalled()
  })

  test("returns service_unavailable when API base is missing", async () => {
    vi.mocked(getServerApiBase).mockReturnValue("")

    const result = await getTasks()

    expect(result).toEqual({ data: null, error: "service_unavailable" })
    expect(apiFetchJson).not.toHaveBeenCalled()
  })
})
