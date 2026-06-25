import { type NextRequest, NextResponse } from "next/server"
import { getAuthRedirectOrigin } from "../redirect-origin"

export function GET(request: NextRequest) {
  const redirectOrigin = getAuthRedirectOrigin(request.url)
  const response = NextResponse.redirect(new URL("/auth/login", redirectOrigin))
  response.cookies.delete("atlas_token")
  return response
}
