import { cookies } from "next/headers"
import { type NextRequest, NextResponse } from "next/server"
import { getServerApiBase } from "@/lib/api-base"

export async function GET(request: NextRequest) {
  const token = (await cookies()).get("atlas_token")?.value
  const returnTo = request.nextUrl.searchParams.get("returnTo") || "/"
  const publicOrigin = process.env.AUTH_RETURN_URL_ORIGIN ?? request.nextUrl.origin
  const parsedReturnTo = new URL(returnTo, publicOrigin)
  const redirectUrl = new URL(
    `${parsedReturnTo.pathname}${parsedReturnTo.search}${parsedReturnTo.hash}` || "/",
    publicOrigin,
  )
  redirectUrl.searchParams.set("_admin", String(Date.now()))

  if (!token) {
    return NextResponse.redirect(redirectUrl)
  }

  const apiBase = getServerApiBase()
  if (!apiBase) {
    return NextResponse.redirect(redirectUrl)
  }

  try {
    const result = await fetch(`${apiBase}/api/users/me/admin-mode/toggle`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    })
    if (!result.ok) {
      redirectUrl.searchParams.set("adminToggle", "failed")
    }
  } catch {
    redirectUrl.searchParams.set("adminToggle", "failed")
    return NextResponse.redirect(redirectUrl)
  }

  return NextResponse.redirect(redirectUrl)
}
