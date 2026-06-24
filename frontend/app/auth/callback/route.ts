import { NextResponse } from "next/server"
import { getAuthRedirectOrigin } from "../redirect-origin"

const tokenCookieName = "atlas_token"
const maxAgeSeconds = 8 * 60 * 60

export function GET(request: Request) {
  const url = new URL(request.url)
  const redirectOrigin = getAuthRedirectOrigin(request.url)
  const token = url.searchParams.get("token")

  if (!token) {
    return NextResponse.redirect(new URL("/auth/login", redirectOrigin))
  }

  const response = NextResponse.redirect(new URL("/", redirectOrigin))
  response.cookies.set({
    name: tokenCookieName,
    value: token,
    httpOnly: true,
    sameSite: "lax",
    secure: url.protocol === "https:",
    path: "/",
    maxAge: maxAgeSeconds,
  })

  return response
}
