import { beforeEach, describe, expect, test, vi } from "vitest"

vi.mock("next/cache", () => ({
  revalidatePath: vi.fn(),
}))

vi.mock("@/lib/users/api", () => ({
  updateUserSettings: vi.fn(),
}))

import { revalidatePath } from "next/cache"
import { updateUserSettings } from "@/lib/users/api"
import { updateUserSettingsAction } from "./actions"

describe("updateUserSettingsAction", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  test("returns data and revalidates on success", async () => {
    vi.mocked(updateUserSettings).mockResolvedValueOnce({ ok: true, data: undefined })

    const result = await updateUserSettingsAction({ shareNotificationEnabled: true })

    expect(result).toEqual({ data: {} })
    expect(revalidatePath).toHaveBeenCalledWith("/users/settings")
  })

  test("returns error without revalidating on failure", async () => {
    vi.mocked(updateUserSettings).mockResolvedValueOnce({
      ok: false,
      message: "shareNotificationEnabled is required.",
      code: "bad_request",
    })

    const result = await updateUserSettingsAction({ shareNotificationEnabled: true })

    expect(result).toEqual({ error: "shareNotificationEnabled is required." })
    expect(revalidatePath).not.toHaveBeenCalled()
  })
})
