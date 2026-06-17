import { NextResponse } from "next/server"

const tokenCookieName = "atlas_token"
const maxAgeSeconds = 8 * 60 * 60

export function GET(request: Request) {
  const url = new URL(request.url)
  const token = url.searchParams.get("token")

  if (!token) {
    return NextResponse.redirect(new URL("/auth/login", url.origin))
  }

  const response = NextResponse.redirect(new URL("/", url.origin))
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
