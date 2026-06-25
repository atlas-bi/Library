import { type NextRequest, NextResponse } from "next/server"

export function GET(request: NextRequest) {
  const publicOrigin = process.env.AUTH_RETURN_URL_ORIGIN ?? request.nextUrl.origin
  const response = NextResponse.redirect(new URL("/", publicOrigin))
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
