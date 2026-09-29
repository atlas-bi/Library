import { NextRequest } from "next/server"
import { afterEach, describe, expect, test } from "vitest"
import { GET as callbackGet } from "./callback/route"
import { GET as logoutGet } from "./logout/route"

describe("auth public redirect origin", () => {
  const originalEnv = process.env.AUTH_RETURN_URL_ORIGIN

  afterEach(() => {
    if (originalEnv === undefined) {
      delete process.env.AUTH_RETURN_URL_ORIGIN
    } else {
      process.env.AUTH_RETURN_URL_ORIGIN = originalEnv
    }
  })

  test("callback redirects to configured public origin after setting token", () => {
    process.env.AUTH_RETURN_URL_ORIGIN = "https://library.atlas.bi"

    const response = callbackGet(new Request("https://0.0.0.0:3000/auth/callback?token=test-token"))

    expect(response.headers.get("location")).toBe("https://library.atlas.bi/")
  })

  test("logout redirects to configured public origin", () => {
    process.env.AUTH_RETURN_URL_ORIGIN = "https://library.atlas.bi"

    const response = logoutGet(new NextRequest("https://0.0.0.0:3000/auth/logout"))

    expect(response.headers.get("location")).toBe("https://library.atlas.bi/")
  })
})
