import { redirect } from "next/navigation"
import type { NextRequest } from "next/server"
import { getPublicApiBase } from "@/lib/api-base"
import { buildLoginCallbackUrl } from "../return-path"

export function GET(request: NextRequest) {
  const authReturnOrigin = process.env.AUTH_RETURN_URL_ORIGIN ?? request.nextUrl.origin
  const returnUrl = buildLoginCallbackUrl(
    authReturnOrigin,
    request.nextUrl.searchParams.get("returnTo"),
  )
  const apiBase = getPublicApiBase(request.nextUrl.origin)
  redirect(`${apiBase}/api/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`)
}
