import { redirect } from "next/navigation"
import type { NextRequest } from "next/server"
import { getPublicApiBase } from "@/lib/api-base"

export function GET(request: NextRequest) {
  const authReturnOrigin = process.env.AUTH_RETURN_URL_ORIGIN ?? request.nextUrl.origin
  const returnUrl = `${authReturnOrigin.replace(/\/$/, "")}/auth/callback`
  const apiBase = getPublicApiBase(request.nextUrl.origin)
  redirect(`${apiBase}/api/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`)
}
