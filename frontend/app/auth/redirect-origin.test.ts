import { NextRequest } from "next/server"
import { afterEach, describe, expect, test } from "vitest"
import { GET as callbackGet } from "./callback/route"
import { GET as logoutGet } from "./logout/route"

describe("auth public redirect origin", () => {
  const originalEnv = process.env.AUTH_RETURN_URL_ORIGIN
  const originalDemo = process.env.DEMO
  const originalApiUrl = process.env.NEXT_PUBLIC_API_URL

  afterEach(() => {
    if (originalEnv === undefined) {
      delete process.env.AUTH_RETURN_URL_ORIGIN
    } else {
      process.env.AUTH_RETURN_URL_ORIGIN = originalEnv
    }
    if (originalDemo === undefined) {
      delete process.env.DEMO
    } else {
      process.env.DEMO = originalDemo
    }
    if (originalApiUrl === undefined) {
      delete process.env.NEXT_PUBLIC_API_URL
    } else {
      process.env.NEXT_PUBLIC_API_URL = originalApiUrl
    }
  })

  test("callback redirects to configured public origin after setting token", () => {
    process.env.AUTH_RETURN_URL_ORIGIN = "https://library.atlas.bi"

    const response = callbackGet(new Request("https://0.0.0.0:3000/auth/callback?token=test-token"))

    expect(response.headers.get("location")).toBe("https://library.atlas.bi/")
  })

  test("logout redirects to configured public origin", () => {
    process.env.DEMO = "true"
    process.env.AUTH_RETURN_URL_ORIGIN = "https://library.atlas.bi"

    const response = logoutGet(new NextRequest("https://0.0.0.0:3000/auth/logout"))

    expect(response.headers.get("location")).toBe("https://library.atlas.bi/")
  })

  test("logout ends the SAML session when demo mode is off", () => {
    process.env.DEMO = "false"
    process.env.NEXT_PUBLIC_API_URL = "http://localhost:5000"
    process.env.AUTH_RETURN_URL_ORIGIN = "http://localhost:3001"

    const response = logoutGet(new NextRequest("http://localhost:3001/auth/logout"))

    expect(response.headers.get("location")).toBe(
      "http://localhost:5000/api/auth/logout?returnUrl=http%3A%2F%2Flocalhost%3A3001%2F",
    )
    expect(response.cookies.get("atlas_token")?.value).toBe("")
  })
})
