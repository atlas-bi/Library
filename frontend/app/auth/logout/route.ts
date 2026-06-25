import { type NextRequest, NextResponse } from "next/server"

export function GET(request: NextRequest) {
  const publicOrigin = process.env.AUTH_RETURN_URL_ORIGIN ?? request.nextUrl.origin
  const response = NextResponse.redirect(new URL("/auth/login", publicOrigin))
  response.cookies.delete("atlas_token")
  return response
}
