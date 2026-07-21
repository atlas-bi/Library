import { type NextRequest, NextResponse } from "next/server"
import { getAuthRedirectOrigin } from "../redirect-origin"

export function GET(request: NextRequest) {
  const redirectOrigin = getAuthRedirectOrigin(request.url)
  const response = NextResponse.redirect(new URL("/", redirectOrigin))
  response.cookies.set({
    name: "atlas_token",
    value: "",
    httpOnly: true,
    sameSite: "lax",
    secure: request.nextUrl.protocol === "https:",
    path: "/",
    expires: new Date(0),
  })
  return response
}
