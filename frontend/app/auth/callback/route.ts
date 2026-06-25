import { NextResponse } from "next/server"

const tokenCookieName = "atlas_token"
const maxAgeSeconds = 8 * 60 * 60

export function GET(request: Request) {
  const url = new URL(request.url)
  const token = url.searchParams.get("token")
  const publicOrigin = process.env.AUTH_RETURN_URL_ORIGIN ?? url.origin

  if (!token) {
    return NextResponse.redirect(new URL("/auth/login", publicOrigin))
  }

  const response = NextResponse.redirect(new URL("/", publicOrigin))
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
