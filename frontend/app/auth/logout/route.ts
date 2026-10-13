import { type NextRequest, NextResponse } from "next/server"
import { getPublicApiBase } from "@/lib/api-base"
import { getAuthRedirectOrigin } from "../redirect-origin"

function samlLoginRequired(): boolean {
  const demo = process.env.DEMO?.trim().toLowerCase()
  return demo === "false" || demo === "0" || demo === "no"
}

export function GET(request: NextRequest) {
  const redirectOrigin = getAuthRedirectOrigin(request.url)
  const destination = samlLoginRequired()
    ? `${getPublicApiBase(request.nextUrl.origin)}/api/auth/logout?returnUrl=${encodeURIComponent(`${redirectOrigin}/`)}`
    : new URL("/", redirectOrigin)
  const response = NextResponse.redirect(destination)
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
